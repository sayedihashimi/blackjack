using SayedHa.Blackjack.Shared.Roulette;
using System.Diagnostics;

RoulettePlayer player = new RoulettePlayer();
var settings = new GameSettings {
    EnableConsoleLogger = false,
    NumberOfSpins = 1000000
};

var watch = Stopwatch.StartNew();
player.Play(settings);
watch.Stop();

Console.WriteLine($"num spins: {settings.NumberOfSpins:N0}\ntime: {watch.Elapsed.TotalSeconds}");