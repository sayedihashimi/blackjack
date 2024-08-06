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
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using SayedHa.Blackjack;
using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Betting;
using SayedHa.Blackjack.Shared.Blackjack.Strategy;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
// TODO: Get rid of this
using SB = SayedHa.Blackjack.Shared.Blackjack.Strategy.StrategyBuilder;
// TODO: this whole class needs to be replaced, just prototyping currently.

//var summary = BenchmarkRunner.Run<StrategyBuilderBenchmark>();
//return;

//var summary = BenchmarkRunner.Run<RandomNumberBenchmarks>();
//Console.WriteLine(summary);
//return;

//var stopwatch1 = new Stopwatch();
//stopwatch1.Start();
//var rtb = new RandomTreeBenchmarks();
//rtb.CreateRandomTrees();
//stopwatch1.Stop();
//Console.WriteLine($"Elapsed seconds: '{stopwatch1.Elapsed.TotalSeconds}'");
//return;

//var summary = BenchmarkRunner.Run<RandomTreeBenchmarks>(
//    ManualConfig
//        .Create(DefaultConfig.Instance)
//        .WithOptions(ConfigOptions.DisableOptimizationsValidator)
//    );
//Console.WriteLine(summary);

//var summary = BenchmarkRunner.Run<StrategyTreeParallelBenchmarks>();
//Console.WriteLine(" **** Summary below ****");
//Console.WriteLine(summary);

//return;

StartRunningStrategyBuilder2();
return;

void StartRunningStrategyBuilder() {
    var settings = new StrategyBuilderSettings();
    Console.WriteLine(@$"starting test for: 
num generations: {settings.MaxNumberOfGenerations} 
population: {settings.NumStrategiesForFirstGeneration} 
num parents (survivors) each gen: {settings.NumStrategiesToGoToNextGeneration}
num hands to play for each strategy: {settings.NumHandsToPlayForEachStrategy}
initial mutation rate: {settings.InitialMutationRate}
min mutation rate: {settings.MinMutationRate}");

    var strategy1 = new SB(settings);
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    var result = strategy1.FindBestStrategies(5);
    stopwatch.Stop();

    Console.WriteLine(@$"Completed test for: 
num generations: {settings.MaxNumberOfGenerations} 
population: {settings.NumStrategiesForFirstGeneration} 
num parents (survivors) each gen: {settings.NumStrategiesToGoToNextGeneration}
num hands to play for each strategy: {settings.NumHandsToPlayForEachStrategy}
initial mutation rate: {settings.InitialMutationRate}
min mutation rate: {settings.MinMutationRate}");

    var sb = new StringBuilder();
    var sWriter = new StringWriter(sb);
    sWriter.WriteLine($"elapsed time: {stopwatch.Elapsed.ToString(@"hh\:mm\:ss")}");
    sWriter.WriteLine($"Num generations: {settings.MaxNumberOfGenerations}");
    sWriter.WriteLine("Top strategies found");
    for (int i = 0; i < result.Count; i++) {
        sWriter.WriteLine($" ------------- {i} -------------");
        result[i].WriteTreeStringTo(sWriter);
    }

    sWriter.Flush();
    sWriter.Close();

    Console.WriteLine(sb.ToString());

}
void StartRunningStrategyBuilder2() {
    var settings = new StrategyBuilderSettings();
    Console.WriteLine(@$"starting test for: 
num generations: {settings.MaxNumberOfGenerations} 
population: {settings.NumStrategiesForFirstGeneration} 
num parents (survivors) each gen: {settings.NumStrategiesToGoToNextGeneration}
num hands to play for each strategy: {settings.NumHandsToPlayForEachStrategy}
initial mutation rate: {settings.InitialMutationRate}
min mutation rate: {settings.MinMutationRate}
num best clones per gen: {settings.NumOfBestStrategyClonesToMakePerGeneration}
create smart strategies: {settings.CreateSmartRandomStrategies}
tournament size: {settings.TournamentSize}");

    var strategyBuilder = new StrategyBuilder2(settings);
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    var result = strategyBuilder.FindBestStrategies2(5);
    stopwatch.Stop();

    Console.WriteLine(@$"Completed test for: 
num generations: {settings.MaxNumberOfGenerations} 
population: {settings.NumStrategiesForFirstGeneration} 
num parents (survivors) each gen: {settings.NumStrategiesToGoToNextGeneration}
num hands to play for each strategy: {settings.NumHandsToPlayForEachStrategy}
initial mutation rate: {settings.InitialMutationRate}
min mutation rate: {settings.MinMutationRate}
num best clones per gen: {settings.NumOfBestStrategyClonesToMakePerGeneration}
create smart strategies: {settings.CreateSmartRandomStrategies}
tournament size: {settings.TournamentSize}");

    var sb = new StringBuilder();
    var sWriter = new StringWriter(sb);
    sWriter.WriteLine($"elapsed time: {stopwatch.Elapsed.ToString(@"hh\:mm\:ss")}");
    sWriter.WriteLine($"Num generations: {settings.MaxNumberOfGenerations}");
    sWriter.WriteLine("Top strategies found");
    for (int i = 0; i < result.Count; i++) {
        sWriter.WriteLine($"Num diff from Basic Strategy: {result[i].NumDifferencesFromBasicStrategy}");
        sWriter.WriteLine($" ------------- {i} Score=({result[i].FitnessScore}) -------------");
        // result[i].WriteTreeStringTo(sWriter);
        result[i].WriteTo(sWriter);
    }

    sWriter.Flush();
    sWriter.Close();

    Console.WriteLine(sb.ToString());
}
void StartStrategyBuilder2ProduceOffspring() {
    var factory = NextHandActionArrayFactory.Instance;
    var parent1 = factory.CreateStrategyWithAllStands(false);
    var parent2 = factory.CreateStrategyWithAllHits(true);

    var sb = new StrategyBuilder2();
    (var child1, var child2) = sb.ProduceOffspring(parent1, parent2);

    var stringBuilder = new StringBuilder();
    var stringWriter = new StringWriter(stringBuilder);
    
    stringWriter.WriteLine("---- parent 1 ----");
    parent1.WriteTo(stringWriter);

    stringWriter.WriteLine("---- parent 2 ----");
    parent2.WriteTo(stringWriter);

    stringWriter.WriteLine("---- child 1 ----");
    child1.WriteTo(stringWriter);
    
    stringWriter.WriteLine("---- child 2 ----");
    child2.WriteTo(stringWriter);

    stringWriter.Flush();
    Console.WriteLine(stringBuilder.ToString());
}
void StartBenchmarkStrategyBuilderVersusStrategyBuilder2() {
    BenchmarkRunner.Run<StrategyBuilderVersusStrategyBuilder2>();
}
void StartBenchmarkCreateRandomStrategies() {
    BenchmarkRunner.Run<CreateRandomStrategiesBenchmarks>();
}
void StartBenchmarkProduceOffspring() {
    BenchmarkRunner.Run<ProduceOffspringBenchmarks>();
}
void StartBenchmarkMutateOffspringBenchmarks() {
    BenchmarkRunner.Run<MutateOffspringBenchmarks>();
}
void StartBenchmarkTournamentSize() {
    BenchmarkRunner.Run<TournamentSizeBenchmarks>();
}
void StartForProfilingStrategyBuilder2() {
    var settings = new StrategyBuilderSettings{
        AllConsoleOutputDisabled = true,
        NumStrategiesForFirstGeneration = 1000,
        NumStrategiesToGoToNextGeneration = 500,
        NumHandsToPlayForEachStrategy = 1000,
        MaxNumberOfGenerations = 20,
        InitialMutationRate = 25,
        MinMutationRate = 5,
        MutationRateChangePerGeneration = 1,
        EnableMultiThreads = true,
        MtMaxNumThreads = 72,
    };
    //Console.WriteLine("Press any key to start");
    //Console.ReadKey();
    var sb = new StrategyBuilder2(settings);
    _ = sb.FindBestStrategies(5);
}

// usage
// [numGamesToPlay] [outputPath] [enableConsoleLogger] [enableFileLogger] [enableMultiThread]

var numGamesToPlay = 10;
string? outputPath = null;
bool enableConsoleLogger = true;
bool enableFileLogger = false;
bool multiThreads = true;
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
    // should be last
    if (args.Length > 4) {
        multiThreads = bool.Parse(args[4]);
    }
}

// var logger = new Logger(addLogger);
int numDecks = BlackjackSettings.GetBlackjackSettings().NumberOfDecks;

var strategiesToPlay = BlackjackSettings.GetBlackjackSettings().StrategiesToPlay;

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

// still not sure which is faster
if (multiThreads) {
    var semaphore = new SemaphoreSlim(strategiesToPlay.Count, strategiesToPlay.Count);
    var tasks = new List<Task>();
    foreach (var strategy in strategiesToPlay) {
        await semaphore.WaitAsync();
        try {
            var task = Task.Run(async () => { await StartForStrategyAsync(strategy, numGamesToPlay, enableConsoleLogger, enableFileLogger, outputPathFull); });
            tasks.Add(task);
        }
        finally {
            semaphore.Release();
        }
    }

    await Task.WhenAll(tasks);
}
else {
    foreach (var strategy in strategiesToPlay) {
        await StartForStrategyAsync(strategy, numGamesToPlay, enableConsoleLogger, enableFileLogger, outputPathFull);
    }
}

async Task StartForStrategyAsync(OpponentPlayStrategy strategy, int numGamesToPlay, bool enableConsoleLogger, bool enableFileLogger, string? outputPathFull) {
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
        logger.LogLine($"cards: [{game.Cards}]");
        for (int i = 0; i < numGamesToPlay; i++) {
            logger.LogLine($"Game {i+1}");
            var gameResult = gameRunner.PlayGame(game);
            gameResults.Add(gameResult);
            logger.Log($"Bankroll: dealer ${gameResult.DealerRemainingCash.remaining:F0}(${gameResult.DealerRemainingCash.diff:F0})");
            foreach (var (remaining, diff) in gameResult.OpponentRemaining) {
                logger.Log($", op: ${remaining:F0}(${diff:F0}){Environment.NewLine}");
            }
        }

        if (!string.IsNullOrEmpty(outputFolderPath)) {
            var fullpathtofile = Path.Combine(outputFolderPath, $"{opponentPlayStrategy}.csv");
            await CreateCsvFileForResultsAsync(fullpathtofile, gameResults);
        }
        else {
            logger.LogLine("Not creating csv file because no argument was passed with the path");
        }

        var summaryString = GetSummaryString(numGamesToPlay, game);

        if (enableFileLogger) {
            logger.LogLine(summaryString);
            logger.CloseFileLogger();
        }

        Console.WriteLine(summaryString);

    }
    catch (Exception ex) {
        logger.LogLine(ex.ToString());
    }
}

string GetSummaryString(int numGames,Game game) {
    var sb = new StringBuilder();

    sb.AppendLine($"Num games: {numGames}");
    var (maxOpponentConWins, maxDealerConWins) = GetNumConsecutiveWins((Opponent)game.Opponents[0]);
    sb.AppendLine($"Max consecutive wins, opponent={maxOpponentConWins},dealer={maxDealerConWins}");
    sb.AppendLine($"Bankroll: dealer {game.Dealer.BettingStrategy.Bankroll.DollarsRemaining}, op: {game.Opponents[0].BettingStrategy.Bankroll.DollarsRemaining}");

    return sb.ToString();
}
(int maxOpponentConWins, int maxDealerConWins) GetNumConsecutiveWins(Opponent opponent) {
    Debug.Assert(opponent != null);
    var maxNumOpponentConWins = 0;
    var maxNumDealerConWins = 0;

    var currentNumOpponentConWins = 0;
    var currentNumDealerConWins = 0;

    var currentNode = opponent.GetAllHands().First;
    while (currentNode != null) {
        switch (currentNode.Value.HandResult) {
            case HandResult.DealerWon:
                currentNumDealerConWins++;
                // reset opponentwin
                maxNumOpponentConWins = currentNumOpponentConWins > maxNumOpponentConWins ? currentNumOpponentConWins : maxNumOpponentConWins;
                currentNumOpponentConWins = 0;
                break;
            case HandResult.OpponentWon:
                currentNumOpponentConWins++;
                // reset dealer con wins
                if (currentNumDealerConWins > maxNumDealerConWins) {
                    maxNumDealerConWins = currentNumDealerConWins;
                }
                currentNumDealerConWins = currentNumDealerConWins > maxNumDealerConWins?currentNumDealerConWins : maxNumDealerConWins;
                currentNumDealerConWins = 0;
                break;
            default:
                break;
        }
        currentNode = currentNode.Next;
    }

    if (currentNumOpponentConWins > maxNumOpponentConWins) {
        maxNumOpponentConWins = currentNumOpponentConWins;
    }
    if (currentNumDealerConWins > maxNumDealerConWins) {
        maxNumDealerConWins = currentNumDealerConWins;
    }

    return (maxNumOpponentConWins, maxNumDealerConWins);
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