using SayedHa.Blackjack.Shared.Roulette;
using System.Diagnostics;

int numSpins = 1000;
bool enableCsvFileOutput = false;
bool enableNumberDetails = true;
bool enableMartingale = true;
bool enableGreen = true;
bool enableBondMartingale = true;

int minimumBet = 20;
int initialBankroll = 5000;

var rouletteType = RouletteType.European;

if(args.Length == 1) {
    numSpins = int.Parse(args[0]);
}

var settings = new GameSettings {
    EnableConsoleLogger = false,
    NumberOfSpins = numSpins
};
// need to change this to support custom if needed later
settings.SetRouletteType(rouletteType);

var recorders = new List<IGameRecorder>();
if (settings.EnableConsoleLogger) {
    recorders.Add(new ConsoleGameRecorder());
}

var timestamp = DateTime.Now.ToString("yyyy.MM.dd-hhmmss.ff");
var outputPath = $@"C:\temp\roulette";
var filenamePrefix = $"r-{numSpins}-{timestamp}-";

if (enableCsvFileOutput) {
    recorders.Add(new CsvGameRecorder(outputPath, $"r-{numSpins}-{timestamp}-"));
}
if (enableNumberDetails) {
    recorders.Add(new NumberDetailsRecorder(settings, outputPath, filenamePrefix));
}
if (enableMartingale) {
    recorders.Add(new MartingaleBettingRecorder(outputPath, filenamePrefix, GameCellColor.Black, 1, initialBankroll, true));
    recorders.Add(new MartingaleBettingRecorder(outputPath, filenamePrefix, GameCellColor.Red, 1, initialBankroll, false));
}
if (enableGreen) {
    recorders.Add(new GreenMethodRecorder(outputPath, filenamePrefix, minimumBet, initialBankroll, true));
    recorders.Add(new GreenAgressiveMethodRecorder(outputPath, filenamePrefix, minimumBet, initialBankroll, true));
}
if (enableBondMartingale) {
    recorders.Add(new BondMartingaleBettingRecorder(outputPath, filenamePrefix, minimumBet, initialBankroll, true));
}

// csv with stats always needs to be added for the summary
var csvWithStatsRecorder = new CsvWithStatsGameRecorder(outputPath, $"r-{numSpins}-{timestamp}-");
csvWithStatsRecorder.EnableWriteCsvFile = enableCsvFileOutput;
recorders.Add(csvWithStatsRecorder);

var watch = Stopwatch.StartNew();

var controller = new RouletteGameController(settings, outputPath, filenamePrefix);
foreach(var recorder in recorders) {
    controller.AddGameRecorder(recorder);
}

// await controller.PlayAll();

await controller.PlayRollup(500);

watch.Stop();
Console.WriteLine($"num spins: {settings.NumberOfSpins:N0}\ntime: {watch.Elapsed.TotalSeconds}\nfilenames: '{filenamePrefix}*'");