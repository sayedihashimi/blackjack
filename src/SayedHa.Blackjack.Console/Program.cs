// This file is part of SayedHa.Blackjack.
//
// SayedHa.Blackjack is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SayedHa.Blackjack is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with SayedHa.Blackjack.  If not, see <https://www.gnu.org/licenses/>.
using SayedHa.Blackjack.Shared;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;

// usage
// [numGamesToPlay] [pathToCsvFileToWriteResults] [enableConsoleLogger] [enableFileLogger]

var numGamesToPlay = 10;
string? outputPath = null;
bool enableConsoleLogger = true;
bool enableFileLogger = false;
if (args != null && args.Length > 0) {
    numGamesToPlay = int.Parse(args[0]);
    if (args.Length > 1) {
        outputPath = args[1];
    }

    if (args.Length > 2) {
        enableConsoleLogger = bool.Parse(args[2]);
    }
    if (args.Length > 3) {
        enableFileLogger = bool.Parse(args[3]);
    }
}

// var logger = new Logger(addLogger);
int numDecks = 6;

var strategiesToPlay = new List<OpponentPlayStrategy>() {
    OpponentPlayStrategy.BasicStrategy,
    //OpponentPlayStrategy.StandOn12,
    //OpponentPlayStrategy.StandOn13,
    //OpponentPlayStrategy.StandOn14,
    //OpponentPlayStrategy.StandOn15,
    //OpponentPlayStrategy.StandOn16,
    OpponentPlayStrategy.StandOn17,
    //OpponentPlayStrategy.StandOn18,
    //OpponentPlayStrategy.StandOn19,
    //OpponentPlayStrategy.StandOn20,
    OpponentPlayStrategy.AlwaysStand,
    OpponentPlayStrategy.Random
};

var timestamp = DateTime.Now.ToString("yyyy.MM.dd-hhmmss.ff");
string? outputPathFull = null;
if (!string.IsNullOrEmpty(outputPath)) {
    if (!Directory.Exists(outputPath)) {
        Directory.CreateDirectory(outputPath);
    }
    // create the timestampe folder now
    outputPathFull = Path.Combine(outputPath, timestamp);
    if (!Directory.Exists(outputPathFull)) {
        Directory.CreateDirectory(outputPathFull);
    }
}

foreach (var strategy in strategiesToPlay) {
    var logger = new Logger(enableConsoleLogger);
    if (enableFileLogger && !string.IsNullOrEmpty(outputPathFull)) {
        var logfilepath = Path.Combine(outputPathFull, $"game.{strategy}.log");        
        logger.ConfigureFileLogger(logfilepath);
    }
    
    logger.LogLine($"Playing {numGamesToPlay} games with strategy '{strategy}'");
    await PlayGameWithStrategyAsync(strategy, outputPathFull, logger);

    if (enableFileLogger) {
        logger.CloseFileLogger();
    }
}

async Task PlayGameWithStrategyAsync(OpponentPlayStrategy opponentPlayStrategy, string? outputFolderPath, ILogger logger) {
    try {
        var gameRunner = new GameRunner(logger);
        logger.LogLine("----------------------------------------------------");
        var gameResults = new List<GameResult>();

        var bettingStrategy = BettingStrategy.CreateNewDefaultBettingStrategy(logger);
        var pf = new ParticipantFactory(bettingStrategy, opponentPlayStrategy, logger);

        var game = gameRunner.CreateNewGame(numDecks, 1, pf, true);
        for (int i = 0; i < numGamesToPlay; i++) {
            logger.LogLine($"Game {i+1}");
            var gameResult = gameRunner.PlayGame(game);
            gameResults.Add(gameResult);
            logger.Log($"Bankroll: dealer ${gameResult.DealerRemainingCash.remaining:F0}(${gameResult.DealerRemainingCash.diff:F0})");
            foreach (var opr in gameResult.OpponentRemaining) {
                logger.Log($", op: ${opr.remaining:F0}(${opr.diff:F0}){Environment.NewLine}");
            }
            logger.LogLine(string.Empty);
        }

        if (!string.IsNullOrEmpty(outputFolderPath)) {
            var fullpathtofile = Path.Combine(outputFolderPath, $"{opponentPlayStrategy}.csv");
            await CreateCsvFileForResultsAsync(fullpathtofile, gameResults);
        }
        else {
            logger.LogLine("Not creating csv file because no argument was passed with the path");
        }


        if (enableFileLogger) {
            logger.CloseFileLogger();
        }
    }
    catch (Exception ex) {
        logger.LogLine(ex.ToString());
    }
}

async Task CreateCsvFileForResultsAsync(string pathToCsv, List<GameResult> results) {
    Debug.Assert(pathToCsv != null);
    Debug.Assert(results != null);
    Debug.Assert(results.Count > 0);
    // if the file exists overwrite it
    using StreamWriter streamWriter = new StreamWriter(pathToCsv, false);
    streamWriter.WriteLine("result,dealerCards,opponent-hand,split,playerRemaining,dealerRemaining");
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
    foreach (var hand in gameResult.OpponentHands) {
        sb.AppendLine($"\"{hand.HandResult}\",\"{gameResult.DealerHand}\",\"{hand}\",\"{gameResult.OpponentHands.Count > 1}\",\"{gameResult.OpponentRemaining[0].remaining}\",\"{gameResult.DealerRemainingCash.remaining}\"");
    }
    return sb.ToString().Trim();
}