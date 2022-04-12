using SayedHa.Blackjack.Shared.Roulette;
using System.Diagnostics;

int numSpins = 100;

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
var csvRecorder = new CsvGameRecorder(filename);
var csvWithStatsRecorder = new CsvWithStatsGameRecorder(statsFilename);

recorders.Add(new CsvGameRecorder(filename));
recorders.Add(new CsvWithStatsGameRecorder(statsFilename));

var watch = Stopwatch.StartNew();
await player.PlayAsync(settings, recorders);
watch.Stop();

foreach (var recorder in recorders) {
    recorder.Dispose();
}

Console.WriteLine($"num spins: {settings.NumberOfSpins:N0}\ntime: {watch.Elapsed.TotalSeconds}\nfilenames: '{filename}','{statsFilename}'");