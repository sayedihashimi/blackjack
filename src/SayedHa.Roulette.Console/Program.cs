using SayedHa.Blackjack.Shared.Roulette;
using System.Diagnostics;

var settings = new GameSettings {
    NumberOfSpins = 1000,
    InitialBankroll = 5000000,
    MaximumBet = 5000,
    MinimumBet = 25,

    AllowNegativeBankroll = false,
    EnableConsoleLogger = false,
    EnableCsvFileOutput = false,
    EnableNumberDetails = false,
    EnableMartingale = false,
    EnableGreen = false,
    EnableBondMartingale = false
};
// need to change this to support custom if needed later
settings.SetRouletteType(RouletteType.European);

try {
    
    await new GameSettingsFactory().SaveSettingsToJsonFileAsync(@"c:\temp\roulette-settings.json", settings);
}
catch (Exception ex) {
    Console.WriteLine(ex.ToString());
}


if (args.Length == 1) {
    settings.NumberOfSpins = int.Parse(args[0]);
}
var recorders = new List<IGameRecorder>();
if (settings.EnableConsoleLogger) {
    recorders.Add(new ConsoleGameRecorder());
}

var timestamp = DateTime.Now.ToString("yyyy.MM.dd-hhmmss.ff");
var outputPath = $@"C:\temp\roulette";
var filenamePrefix = $"r-{settings.NumberOfSpins}-{timestamp}-";

if (settings.EnableCsvFileOutput) {
    recorders.Add(new CsvGameRecorder(outputPath, $"r-{settings.NumberOfSpins}-{timestamp}-"));
}
if (settings.EnableNumberDetails) {
    recorders.Add(new NumberDetailsRecorder(settings, outputPath, filenamePrefix));
}

if (settings.EnableMartingale) {
    recorders.Add(new MartingaleBettingRecorder(outputPath, filenamePrefix, GameCellColor.Black, settings.MinimumBet, settings.InitialBankroll, true) {
        MaximumBet = settings.MaximumBet,
        AllowNegativeBankroll = settings.AllowNegativeBankroll
    });
    recorders.Add(new MartingaleBettingRecorder(outputPath, filenamePrefix, GameCellColor.Red, settings.MinimumBet, settings.InitialBankroll, false) { 
        MaximumBet = settings.MaximumBet,
        AllowNegativeBankroll = settings.AllowNegativeBankroll
    });
}
if (settings.EnableGreen) {
    recorders.Add(new GreenMethodRecorder(outputPath, filenamePrefix, settings.MinimumBet, settings.InitialBankroll, true) { 
        MaximumBet = settings.MaximumBet,
        AllowNegativeBankroll = settings.AllowNegativeBankroll
    });
    recorders.Add(new GreenAgressiveMethodRecorder(outputPath, filenamePrefix, settings.MinimumBet, settings.InitialBankroll, true) { 
        MaximumBet = settings.MaximumBet,
        AllowNegativeBankroll = settings.AllowNegativeBankroll
    });
}
if (settings.EnableBondMartingale) {
    recorders.Add(new BondMartingaleBettingRecorder(outputPath, filenamePrefix, settings.MinimumBet, settings.InitialBankroll, true) { 
        MaximumBet = settings.MaximumBet,
        AllowNegativeBankroll = settings.AllowNegativeBankroll
    });
}

// csv with stats always needs to be added for the summary
var csvWithStatsRecorder = new CsvWithStatsGameRecorder(outputPath, $"r-{settings.NumberOfSpins}-{timestamp}-");
csvWithStatsRecorder.EnableWriteCsvFile = settings.EnableCsvFileOutput;
recorders.Add(csvWithStatsRecorder);

var watch = Stopwatch.StartNew();

var controller = new RouletteGameController(settings, outputPath, filenamePrefix);
foreach(var recorder in recorders) {
    controller.AddGameRecorder(recorder);
}

// await controller.PlayAll();

await controller.PlayRollup(50);

watch.Stop();
Console.WriteLine($"num spins: {settings.NumberOfSpins:N0}\ntime: {watch.Elapsed.TotalSeconds}\nfilenames: '{filenamePrefix}*'");