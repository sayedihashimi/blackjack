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
using System.Diagnostics;

var settings = new GameSettings {
    NumberOfSpins = 1000,
    InitialBankroll = 5000000,
    MaximumBet = 5000,
    MinimumBet = 25,

    AllowNegativeBankroll = false,
    EnableConsoleLogger = false,
    EnableCsvFileOutput = true,
    EnableNumberDetails = true,
    EnableMartingale = false,
    EnableGreen = true,
    EnableBondMartingale = false
};
// need to change this to support custom if needed later
settings.SetRouletteType(RouletteType.American);

//try {
//    await new GameSettingsFactory().SaveSettingsToJsonFileAsync(@"c:\temp\roulette-settings.json", settings);
//}
//catch (Exception ex) {
//    Console.WriteLine(ex.ToString());
//}


if (args.Length == 1) {
    settings.NumberOfSpins = int.Parse(args[0]);
}

var timestamp = DateTime.Now.ToString("yyyy.MM.dd-hhmmss.ff");
var outputPath = $@"C:\temp\roulette";
var filenamePrefix = $"r-{settings.NumberOfSpins}-{timestamp}-";

var watch = Stopwatch.StartNew();

var controller = new RouletteGameController(settings, outputPath, filenamePrefix);

await controller.PlayAll();
// await controller.PlayRollup(50);

watch.Stop();
Console.WriteLine($"num spins: {settings.NumberOfSpins:N0}\ntime: {watch.Elapsed.TotalSeconds}\nfilenames: '{filenamePrefix}*'");