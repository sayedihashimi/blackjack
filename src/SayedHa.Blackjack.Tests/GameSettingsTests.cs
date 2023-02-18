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
using SayedHa.Blackjack.Shared.Roulette;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class GameSettingsTests {
        [Fact]
        public void TestReadGameSettingsFromJsonString() {
            var gs = new GameSettings();
            gs.MaximumBet = 123456;
            gs.InitialBankroll = 145669874;

            // write the settings to a JSON string and then deserialize
            var jsonStr = System.Text.Json.JsonSerializer.Serialize<GameSettings>(gs);

            var newGameSettingsFromJson = System.Text.Json.JsonSerializer.Deserialize<GameSettings>(jsonStr);

            Assert.NotNull(newGameSettingsFromJson);
            Assert.Equal(gs.MaximumBet, newGameSettingsFromJson!.MaximumBet);
            Assert.Equal(gs.InitialBankroll, newGameSettingsFromJson!.InitialBankroll);
        }
        [Fact]
        public async void TestReadGameSettingsFromJsonFile() {
            var gs = new GameSettings();
            gs.MaximumBet = 123456;
            gs.InitialBankroll = 145669874;

            var filepath = System.IO.Path.GetTempFileName();
            using FileStream createStream = File.Create(filepath);
            await System.Text.Json.JsonSerializer.SerializeAsync<GameSettings>(createStream, gs);
            await createStream.DisposeAsync();

            await Task.Delay(500);
            using FileStream readStream = File.OpenRead(filepath);
            // read that same file
            var newGs = System.Text.Json.JsonSerializer.Deserialize<GameSettings>(readStream);
            await readStream.DisposeAsync();

            Assert.NotNull(newGs);
            Assert.Equal(gs.MaximumBet, newGs!.MaximumBet);
            Assert.Equal(gs.InitialBankroll, newGs.InitialBankroll);

            try {
                File.Delete(filepath);
            }
            catch(Exception ex) {
                Console.WriteLine($"Unable to delete the temp file at '{filepath}'. Error:");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
