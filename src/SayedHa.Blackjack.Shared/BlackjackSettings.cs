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
using SayedHa.Blackjack.Shared.Betting;
using System.Diagnostics;

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
            ShuffleThresholdPercent = 20;
        }
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

        // replace this with DI later
        public static BlackjackSettings GetBlackjackSettings() {
            return _instance;
        }
    }
}
