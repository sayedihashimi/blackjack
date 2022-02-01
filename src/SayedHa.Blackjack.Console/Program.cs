// See https://aka.ms/new-console-template for more information
using SayedHa.Blackjack.Shared;

Console.WriteLine("Hello, World!");

try {
    var gameRunner = new GameRunner(6, 1);
    var game = gameRunner.CreateNewGame();
    gameRunner.PlayGame(game);
}
catch(Exception ex) {
    Console.WriteLine(ex.ToString());
}
