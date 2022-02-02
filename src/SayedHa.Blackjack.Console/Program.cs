// See https://aka.ms/new-console-template for more information
using SayedHa.Blackjack.Shared;

int numGamesToPlay = 1;

try {
    var gameRunner = new GameRunner(6, 1);
    for (int i = 0; i < numGamesToPlay; i++) {
        var game = gameRunner.CreateNewGame();
        gameRunner.PlayGame(game);
    }    
}
catch(Exception ex) {
    Console.WriteLine(ex.ToString());
}
