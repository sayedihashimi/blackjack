﻿// See https://aka.ms/new-console-template for more information
using SayedHa.Blackjack.Shared;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;

// usage
// [numGamesToPlay] [pathToCsvFileToWriteResults] [enableConsoleLogger]

var numGamesToPlay = 10;
string? pathToCsvForGameResult = null;
bool addLogger = true;
if(args != null && args.Length > 0) {
    numGamesToPlay = int.Parse(args[0]);
    if (args.Length > 1) {
        pathToCsvForGameResult = args[1];
    }

    if (args.Length > 2) {
        addLogger = bool.Parse(args[2]);
    }
}

var logger = new Logger(addLogger);
int numDecks = 6;

var strategiesToPlay = new List<OpponentPlayStrategy>() {
    OpponentPlayStrategy.BasicStrategy,
    OpponentPlayStrategy.StandOn12,
    OpponentPlayStrategy.StandOn13,
    OpponentPlayStrategy.StandOn14,
    OpponentPlayStrategy.StandOn15,
    OpponentPlayStrategy.StandOn16,
    OpponentPlayStrategy.StandOn17,
    OpponentPlayStrategy.StandOn18,
    OpponentPlayStrategy.StandOn19,
    OpponentPlayStrategy.StandOn20,
    OpponentPlayStrategy.AlwaysStand
};

var timestamp = DateTime.Now.ToString("yyyy.MM.dd-hhmmss.ff");

foreach (var strategy in strategiesToPlay) {
    logger.LogLine($"Playing {numGamesToPlay} games with strategy '{strategy}'");
    await PlayGameWithStrategyAsync(strategy, pathToCsvForGameResult, timestamp, logger);
}

async Task PlayGameWithStrategyAsync(OpponentPlayStrategy opponentPlayStrategy, string? outputFolderPath, string? outputFilePrefix, ILogger logger) {
    try {
        var gameRunner = new GameRunner(logger, 6, 1);
        logger.LogLine("----------------------------------------------------");
        var gameResults = new List<GameResult>();
        var game = gameRunner.CreateNewGame(numDecks, 1, opponentPlayStrategy);
        for (int i = 0; i < numGamesToPlay; i++) {
            gameResults.Add(gameRunner.PlayGame(game));
            logger.LogLine("----------------------------------------------------");
        }

        if (!string.IsNullOrEmpty(outputFolderPath)) {
            // if the folder doesn't exist create it
            if (!Directory.Exists(outputFolderPath)) {
                Directory.CreateDirectory(outputFolderPath);
            }
            outputFilePrefix = outputFilePrefix ?? DateTime.Now.ToString("yyyy.MM.dd-hhmmss.ff");
            var newdirpath = Path.Combine(outputFolderPath, outputFilePrefix);
            if (!Directory.Exists(newdirpath)) {
                Directory.CreateDirectory(newdirpath);
            }
            
            var fullpathtofile = Path.Combine(newdirpath, $"{opponentPlayStrategy}.csv");
            await CreateCsvFileForResultsAsync(fullpathtofile, gameResults);
        }
        else {
            logger.LogLine("Not creating csv file because no argument was passed with the path");
        }
    }
    catch (Exception ex) {
        logger.LogLine(ex.ToString());
    }
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