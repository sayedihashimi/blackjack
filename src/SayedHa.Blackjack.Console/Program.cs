// See https://aka.ms/new-console-template for more information
using SayedHa.Blackjack.Shared;

int numGamesToPlay = 10;

try {
    var gameRunner = new GameRunner(6, 1);
    Console.WriteLine("----------------------------------------------------");
    for (int i = 0; i < numGamesToPlay; i++) {
        var game = gameRunner.CreateNewGame();
        gameRunner.PlayGame(game);
        Console.WriteLine("----------------------------------------------------");
    }    
}
catch(Exception ex) {
    Console.WriteLine(ex.ToString());
}
