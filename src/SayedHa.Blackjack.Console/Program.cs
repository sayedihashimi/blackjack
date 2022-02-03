// See https://aka.ms/new-console-template for more information
using SayedHa.Blackjack.Shared;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;

var numGamesToPlay = 10;
string? pathToCsvForGameResult = null;
if(args != null && args.Length > 0) {
    numGamesToPlay = int.Parse(args[0]);
    if (args.Length > 1) {
        pathToCsvForGameResult = args[1];
    }
}

Console.WriteLine($"Playing {numGamesToPlay} games");

try {
    var gameRunner = new GameRunner(6, 1);
    Console.WriteLine("----------------------------------------------------");
    var gameResults = new List<GameResult>();
    var game = gameRunner.CreateNewGame();
    for (int i = 0; i < numGamesToPlay; i++) {
        gameResults.Add(gameRunner.PlayGame(game));        
        Console.WriteLine("----------------------------------------------------");
    }


    if (!string.IsNullOrEmpty(pathToCsvForGameResult)) {
        await CreateCsvFileForResultsAsync(pathToCsvForGameResult, gameResults);
    }
    else {
        Console.WriteLine("Not creating csv file because no argument was passed with the path");
    }
}
catch(Exception ex) {
    Console.WriteLine(ex.ToString());
}

async Task CreateCsvFileForResultsAsync(string pathToCsv,List<GameResult>results) {
    Debug.Assert(pathToCsv != null);
    Debug.Assert(results != null);
    Debug.Assert(results.Count > 0);
    // if the file exists overwrite it
    using StreamWriter streamWriter = new StreamWriter(pathToCsv, false);
    streamWriter.WriteLine("result,dealerCards,opponent-hand,split");
    foreach (var result in results) {
        await streamWriter.WriteLineAsync(GetCsvStringFor(result));
    }
    await streamWriter.FlushAsync();
}
string GetCsvStringFor(GameResult gameResult) {
    // "result"|"dealerCards"|"opp-hand"|"split"
    // "DealerWon"|"J♦ 7♥"|"6♥,2♠,5♥,5♦"|false
    
    // for each hand create a seperate row
    // if op hands > 1 it must have split so set split to true
    var sb = new StringBuilder();    
    foreach(var hand in gameResult.OpponentHands) {
        sb.AppendLine($"\"{hand.HandResult}\",\"{gameResult.DealerHand}\",\"{hand}\",\"{gameResult.OpponentHands.Count > 1}\"");
    }
    return sb.ToString().Trim();
}