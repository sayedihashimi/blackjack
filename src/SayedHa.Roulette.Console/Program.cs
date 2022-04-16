using SayedHa.Blackjack.Shared.Roulette;
using System.Diagnostics;

int numSpins = 100;
bool enableCsvFileOutput = false;
bool enableSummaryFileOutput = true;
bool enableNumberDetails = true;
bool enableMartingale = true;

if(args.Length == 1) {
    numSpins = int.Parse(args[0]);
}

RoulettePlayer player = new RoulettePlayer();
var settings = new GameSettings {
    EnableConsoleLogger = false,
    NumberOfSpins = numSpins
};

var recorders = new List<IGameRecorder>();
if (settings.EnableConsoleLogger) {
    recorders.Add(new ConsoleGameRecorder());
}

var timestamp = DateTime.Now.ToString("yyyy.MM.dd-hhmmss.ff");
var filename = $@"C:\temp\roulette\r-{numSpins}-{timestamp}.csv";
var statsFilename = $@"C:\temp\roulette\r-{numSpins}-{timestamp}-stats.csv";
var summaryFilename = $@"C:\temp\roulette\r-{numSpins}-{timestamp}-summary.txt"; ;
var csvRecorder = new CsvGameRecorder(filename);
var csvWithStatsRecorder = new CsvWithStatsGameRecorder(statsFilename);
var numberDetailsRecorder = new NumberDetailsRecorder(settings);
var numberDetailsFilename = $@"C:\temp\roulette\r-{numSpins}-{timestamp}-number-details.txt";
csvWithStatsRecorder.EnableWriteCsvFile = enableCsvFileOutput;
var martingaleRedFilepath = $@"C:\temp\roulette\r-{numSpins}-{timestamp}-martingale-red.txt"; ;
var martingaleRed = new MartingaleBettingRecorder(martingaleRedFilepath, GameCellColor.Red, 1, 1000);
var martingaleBlackFilepath = $@"C:\temp\roulette\r-{numSpins}-{timestamp}-martingale-black.txt"; ;
var martingaleBlack = new MartingaleBettingRecorder(martingaleBlackFilepath, GameCellColor.Red, 1, 1000);

if (enableCsvFileOutput) {
    recorders.Add(csvRecorder);
}
if (enableNumberDetails) {
    recorders.Add(numberDetailsRecorder);
}
if (enableMartingale) {
    recorders.Add(martingaleBlack);
    recorders.Add(martingaleRed);
}
// csv with stats always needs to be added for the summary
recorders.Add(csvWithStatsRecorder);

var watch = Stopwatch.StartNew();
var board = Board.BuildBoard(settings);
await player.PlayAsync(board, settings, recorders);
watch.Stop();

if (enableSummaryFileOutput) {
    await csvWithStatsRecorder.CreateSummaryFileAsync(summaryFilename);
}
if (enableNumberDetails) {
    await numberDetailsRecorder.WriteReport(numberDetailsFilename);
}

foreach (var recorder in recorders) {
    await recorder.GameCompleted();
    recorder.Dispose();
}

Console.WriteLine($"num spins: {settings.NumberOfSpins:N0}\ntime: {watch.Elapsed.TotalSeconds}\nfilenames: '{filename}','{statsFilename}'");