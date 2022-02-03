// See https://aka.ms/new-console-template for more information
using SayedHa.Blackjack.Shared;

var numGamesToPlay = 10;
if(args != null && args.Length > 0) {
    numGamesToPlay = int.Parse(args[0]);    
}

Console.WriteLine($"Playing {numGamesToPlay} games");

try {
    var gameRunner = new GameRunner(6, 1);
    Console.WriteLine("----------------------------------------------------");
    var game = gameRunner.CreateNewGame();
    for (int i = 0; i < numGamesToPlay; i++) {
        // TODO: This needs to get updated, every game played uses a new shoe of cards
        // need to reuse the shoe and shuffle (create new) as needed        
        gameRunner.PlayGame(game);
        Console.WriteLine("----------------------------------------------------");
    }    
}
catch(Exception ex) {
    Console.WriteLine(ex.ToString());
}
