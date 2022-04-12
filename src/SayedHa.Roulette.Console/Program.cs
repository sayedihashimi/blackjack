using SayedHa.Blackjack.Shared.Roulette;
using System.Diagnostics;

int numSpins = 100;
bool enableCsvFileOutput = false;
bool enableSummaryFileOutput = true;

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
var summaryFilename = $@"C:\temp\roulette\r-{numSpins}-{timestamp}-summary.csv"; ;
var csvRecorder = new CsvGameRecorder(filename);
var csvWithStatsRecorder = new CsvWithStatsGameRecorder(statsFilename);

csvWithStatsRecorder.EnableWriteCsvFile = enableCsvFileOutput;

if (enableCsvFileOutput) {
    recorders.Add(csvRecorder);
}

recorders.Add(csvWithStatsRecorder);

var watch = Stopwatch.StartNew();
await player.PlayAsync(settings, recorders);
watch.Stop();

if (enableSummaryFileOutput) {
    await csvWithStatsRecorder.CreateSummaryFileAsync(summaryFilename);
}

foreach (var recorder in recorders) {
    recorder.Dispose();
}

Console.WriteLine($"num spins: {settings.NumberOfSpins:N0}\ntime: {watch.Elapsed.TotalSeconds}\nfilenames: '{filename}','{statsFilename}'");