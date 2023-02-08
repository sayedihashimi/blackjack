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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace SayedHa.Blackjack.Shared.Roulette {
    public class GameSettings {
        public GameSettings() {
            SetRouletteType(RouletteType.American);
        }
        // array because some games have 0,0 + 00 and 0 + 00 + OTHER
        [JsonIgnore]
        public string[] SpecialCells { get; set; } = new string[2] { "0", "00" };
        public int NumberOfSpins { get; set; } = 100;
        public bool EnableConsoleLogger { get; set; } = false;
        public bool EnableCsvFileOutput { get; set; } = false;
        public bool EnableNumberDetails { get; set; } = false;
        public bool EnableMartingale { get; set; } = false;
        public bool EnableBondMartingale { get; set; } = false;
        public bool EnableGreen { get; set; }
        private RouletteType _rouletteType = RouletteType.American;
        // [JsonConverter(typeof(StringEnumConverter))]
        public RouletteType RouletteType {
            get => _rouletteType;
            set => SetRouletteType(value);
        }
        public bool StopWhenBankrupt { get; set; } = true;
        public long InitialBankroll { get; set; }
        public int MinimumBet { get; set; } = 1;
        public long MaximumBet { get; set; } = 50000;
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
                    Console.Error.WriteLine($"For RouletteType.Custom call SetCustomRouletteType");
                    break;
                // throw new ArgumentOutOfRangeException($"For RouletteType.Custom call SetCustomRouletteType");
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

        private JsonSerializerOptions _options = new JsonSerializerOptions {
            Converters ={
                new JsonStringEnumConverter()
            }
        };
        
        public async Task SaveSettingsToJsonFileAsync(string filepath, GameSettings settings) {
            using FileStream createStream = File.Create(filepath);
            await System.Text.Json.JsonSerializer.SerializeAsync<GameSettings>(createStream, settings, _options);
            await createStream.FlushAsync();
            await createStream.DisposeAsync();
        }
        public async Task<GameSettings?> ReadFromJsonFileAsync(string filepath) {
            Debug.Assert(!string.IsNullOrEmpty(filepath));
            using FileStream readStream = File.OpenRead(filepath);
            try
            {
                Console.WriteLine($"Reading settings file from '{filepath}'");
                var settings = await System.Text.Json.JsonSerializer.DeserializeAsync<GameSettings>(readStream, _options);
                await readStream.DisposeAsync();
                return settings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading settings file at '{filepath}'. Error:\n{ex}");
                return null;
            }
        }

        public string GetJsonFor(GameSettings settings) =>
           System.Text.Json.JsonSerializer.Serialize(settings);
    }
}
