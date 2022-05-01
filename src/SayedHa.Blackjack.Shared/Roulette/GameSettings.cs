using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Roulette {
    public class GameSettings {
        // array because some games have 0,0 + 00 and 0 + 00 + OTHER
        [JsonIgnore]
        public string[] SpecialCells { get; protected set; } = new string[2] { "0", "00" };

        public int NumberOfSpins { get; set; } = 100;
        public bool EnableConsoleLogger { get; set; } = true;
        public bool EnableCsvFileOutput { get; set; } = false;
        public bool EnableNumberDetails { get; set; } = false;
        public bool EnableMartingale { get; set; } = false;
        public bool EnableBondMartingale { get; set; } = false;
        public bool EnableGreen { get; set; }
        private RouletteType _rouletteType;
        [JsonConverter(typeof(StringEnumConverter))]
        public RouletteType RouletteType {
            get => _rouletteType;
            set => SetRouletteType(value);
        }
        public bool StopWhenBankrupt { get; set; } = true;
        public long InitialBankroll { get; set; }
        public int MinimumBet { get; set; } = 1;
        public long MaximumBet { get; set; } = long.MaxValue;
        public bool AllowNegativeBankroll { get; set; }

        public void SetRouletteType(RouletteType rouletteType) {
            _rouletteType = rouletteType;
            switch (rouletteType) {
                case RouletteType.American:
                    SpecialCells = new string[2] { "0", "00" };
                    break;
                case RouletteType.European:
                    SpecialCells = new string[1] { "0" };
                    break;
                case RouletteType.Custom:
                    throw new ArgumentOutOfRangeException($"For RouletteType.Custom call SetCustomRouletteType");
                default:
                    throw new ArgumentOutOfRangeException(nameof(rouletteType));
            }
        }
        public void SetCustomRouletteType(string[] specialCells) {
            RouletteType = RouletteType.Custom;
            SpecialCells = specialCells;
        }
    }
    public class GameSettingsFactory {
        public async Task SaveSettingsToJsonFileAsync(string filepath,GameSettings settings) =>
            await File.WriteAllTextAsync(filepath, JsonConvert.SerializeObject(settings, Formatting.Indented));

        public GameSettings? ReadFromJsonFile(string filepath) =>
            JsonConvert.DeserializeObject<GameSettings>(filepath);

        public string GetJsonFor(GameSettings settings) =>
            JsonConvert.SerializeObject(settings, Formatting.Indented);
    }
}
