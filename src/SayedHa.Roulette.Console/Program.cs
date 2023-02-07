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