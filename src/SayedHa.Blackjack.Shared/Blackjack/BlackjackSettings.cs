// This file is part of SayedHa.Blackjack.
//
// SayedHa.Blackjack is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SayedHa.Blackjack is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with SayedHa.Blackjack.  If not, see <https://www.gnu.org/licenses/>.
using Newtonsoft.Json.Linq;
using SayedHa.Blackjack.Shared.Betting;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace SayedHa.Blackjack.Shared {
    public class BlackjackSettings {
        private BlackjackSettings() {
            BetAmount = 1;
            BankrollAmount = 0;
            BlackjackPayoutMultplier = 3F / 2F;
            NumberOfDecks = 6;
            StrategiesToPlay = GetDefaultStrategiesToPlay();

            // CreateBettingStrategy = (bankroll) => { return new FixedBettingStrategy(bankroll); };
            CreateBettingStrategy = (bankroll) => { return new BasicHiLoStrategy(bankroll, 1, 12); };

            DoubleDownEnabled = true;
            SplitEnabled = true;

            MaxScore = 21;
            ShuffleThresholdPercent = 25;
        }
        private static object _instanceLock = new object();
        private static BlackjackSettings _instance = new BlackjackSettings();
        public int BetAmount { get; protected init; }
        public int MaxScore { get; protected init; }
        public int ShuffleThresholdPercent { get; protected init; }
        public int BankrollAmount { get; protected init; }
        public float BlackjackPayoutMultplier { get; protected init; }
        public int NumberOfDecks { get; protected init; }
        public List<OpponentPlayStrategy> StrategiesToPlay { get; protected init; }

        public bool DoubleDownEnabled { get; protected init; }
        public bool SplitEnabled { get; protected init; }

        // TODO: Need to come up with a better way to do this that doesn't require a func hopefully.
        public Func<Bankroll,BettingStrategy> CreateBettingStrategy { get; protected init; }

        private List<OpponentPlayStrategy> GetDefaultStrategiesToPlay() => new List<OpponentPlayStrategy>() {
            OpponentPlayStrategy.BasicStrategy,
            //OpponentPlayStrategy.StandOn12,
            //OpponentPlayStrategy.StandOn13,
            
            //OpponentPlayStrategy.StandOn14,
            //OpponentPlayStrategy.StandOn15,
            //OpponentPlayStrategy.StandOn16,
            //OpponentPlayStrategy.StandOn17,
            
            //OpponentPlayStrategy.StandOn18,
            //OpponentPlayStrategy.StandOn19,
            //OpponentPlayStrategy.StandOn20,
            
            //OpponentPlayStrategy.AlwaysStand,
            //OpponentPlayStrategy.Random
        };

        public static async Task<BlackjackSettings> LoadFromJsonFileAsync(string filepath) {
            Debug.Assert(filepath != null);

            if (!File.Exists(filepath)) {
                throw new FileNotFoundException("settings file not found", filepath);
            }

            return LoadFromJson(await File.ReadAllTextAsync(filepath));
        }

        public static BlackjackSettings LoadFromJson(string jsonString) {
            Debug.Assert(!string.IsNullOrEmpty(jsonString));

            JObject json = JObject.Parse(jsonString);
            var defaultSettings = new BlackjackSettings();
            var strategies = GetStrategiesFromJson(json);
            var settings = new BlackjackSettings {
                BetAmount = json.Property("BetAmount") != null ? (int)json["BetAmount"]! : defaultSettings.BetAmount,
                BankrollAmount = json.Property("BankrollAmount") != null ? (int)json["BankrollAmount"]! : defaultSettings.BankrollAmount,
                BlackjackPayoutMultplier = json.Property("BlackjackPayoutMultplier") != null ? (float)json["BlackjackPayoutMultplier"]! : defaultSettings.BlackjackPayoutMultplier,
                NumberOfDecks = json.Property("NumberOfDecks") != null ? (int)json["NumberOfDecks"]! : defaultSettings.NumberOfDecks,
                ShuffleThresholdPercent = json.Property("ShuffleThresholdPercent") != null ? (int)json["ShuffleThresholdPercent"]! : defaultSettings.ShuffleThresholdPercent,
                SplitEnabled = json.Property("SplitEnabled") != null ? (bool)json["SplitEnabled"]! : defaultSettings.SplitEnabled,
                DoubleDownEnabled = json.Property("DoubleDownEnabled") != null ? (bool)json["DoubleDownEnabled"]! : defaultSettings.DoubleDownEnabled,
                StrategiesToPlay = json!.Property("StrategiesToPlay") != null ? GetStrategiesFromJson(json)! : defaultSettings.StrategiesToPlay,
                CreateBettingStrategy = json.Property("BettingStrategy") != null ? GetBettingStrategyFromJson(json) : defaultSettings.CreateBettingStrategy
            };

            return settings;
        }

        protected static Func<Bankroll, BettingStrategy> GetBettingStrategyFromJson(JObject json) {
            Debug.Assert(json != null);

            if(json.Property("BettingStrategy") == null) {
                return null!;
            }

            Func<Bankroll, BettingStrategy>? result = null;
            var name = ((string)json!["BettingStrategy"]!["type"]!).ToLowerInvariant();
            var nameLower = name.ToLowerInvariant();
            switch (nameLower) {
                case "fixedbettingstrategy":
                    result = (bankroll) => { return new FixedBettingStrategy(bankroll); };
                    break;
                case "basichilostrategy":
                    int betUnitValue = (int)json!["BettingStrategy"]!["betUnitValue"]!;
                    int maxBetSpread = (int)json!["BettingStrategy"]!["maxBetSpread"]!;
                    result = (bankroll) => { return new BasicHiLoStrategy(bankroll, betUnitValue, maxBetSpread); };
                    break;
                case "_1324bettingstrategy":
                    result = (bankroll) => { return new _1324BettingStrategy(bankroll); };
                    break;
                default:
                    throw new ArgumentException($"Unknown value for BettingStrategy.Type: '{name}'");
            }

            return result;
        }
        protected static List<OpponentPlayStrategy>? GetStrategiesFromJson(JObject json) {
            Debug.Assert(json != null);

            if(json.Property("StrategiesToPlay") == null) {
                return null;
            }

            var result = new List<OpponentPlayStrategy>();

            JArray strategiesArray = (JArray)json["StrategiesToPlay"]!;
            if(strategiesArray == null) {
                return null;
            }
            var currentToken = strategiesArray!.First;

            while (currentToken != null) {
                var strValue = currentToken.Value<string>();
                if (!string.IsNullOrEmpty(strValue)) {

                    switch (strValue.ToLowerInvariant()) {
                        case "standon12":
                            result.Add(OpponentPlayStrategy.StandOn12);
                            break;
                        case "standon13":
                            result.Add(OpponentPlayStrategy.StandOn13);
                            break;
                        case "standon14":
                            result.Add(OpponentPlayStrategy.StandOn14);
                            break;
                        case "standon15":
                            result.Add(OpponentPlayStrategy.StandOn12);
                            break;
                        case "standon16":
                            result.Add(OpponentPlayStrategy.StandOn16);
                            break;
                        case "standon17":
                            result.Add(OpponentPlayStrategy.StandOn17);
                            break;
                        case "standon18":
                            result.Add(OpponentPlayStrategy.StandOn18);
                            break;
                        case "standon19":
                            result.Add(OpponentPlayStrategy.StandOn19);
                            break;
                        case "standon20":
                            result.Add(OpponentPlayStrategy.StandOn20);
                            break;
                        case "alwaysstand":
                            result.Add(OpponentPlayStrategy.AlwaysStand);
                            break;
                        case "random":
                            result.Add(OpponentPlayStrategy.Random);
                            break;
                        case "basicstrategy":
                            result.Add(OpponentPlayStrategy.BasicStrategy);
                            break;
                        default:
                            throw new ArgumentException($"Unknown value for OpponentPlayStrategy: {strValue}");
                    }
                }
                currentToken = currentToken.Next;
            }

            return result;
        }

        public static void SetBlackjackSettings(BlackjackSettings settings) {
            if (settings == null) {
                throw new ArgumentNullException(nameof(settings));
            }

            lock (_instanceLock) {
                _instance = settings;
            }
        }
        // replace this with DI later
        public static BlackjackSettings GetBlackjackSettings() {
            return _instance;
        }
    }
}
