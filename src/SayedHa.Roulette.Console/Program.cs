using SayedHa.Blackjack.Shared.Roulette;
using System.Diagnostics;

RoulettePlayer player = new RoulettePlayer();
var settings = new GameSettings {
    EnableConsoleLogger = true,
    NumberOfSpins = 100
};

var recorders = new List<IGameRecorder>();
if (settings.EnableConsoleLogger) {
    recorders.Add(new ConsoleGameRecorder());
}

var timestamp = DateTime.Now.ToString("yyyy.MM.dd-hhmmss.ff");
var csvRecorder = new CsvGameRecorder($@"C:\temp\roulette\r-{timestamp}.csv");
recorders.Add(csvRecorder);

var watch = Stopwatch.StartNew();
await player.PlayAsync(settings, recorders);
watch.Stop();

foreach (var recorder in recorders) {
    recorder.Dispose();
}

Console.WriteLine($"num spins: {settings.NumberOfSpins:N0}\ntime: {watch.Elapsed.TotalSeconds}");