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
var csvRecorder = new CsvGameRecorder($@"C:\temp\roulette\r-{timestamp}.csv");
var csvWithStatsRecorder = new CsvWithStatsGameRecorder($@"C:\temp\roulette\r-{timestamp}-stats.csv");

recorders.Add(new CsvGameRecorder($@"C:\temp\roulette\r-{timestamp}.csv"));
recorders.Add(new CsvWithStatsGameRecorder($@"C:\temp\roulette\r-{timestamp}-stats.csv"));

var watch = Stopwatch.StartNew();
await player.PlayAsync(settings, recorders);
watch.Stop();

foreach (var recorder in recorders) {
    recorder.Dispose();
}

Console.WriteLine($"num spins: {settings.NumberOfSpins:N0}\ntime: {watch.Elapsed.TotalSeconds}\nfilename: 'r-{timestamp}*.csv'");