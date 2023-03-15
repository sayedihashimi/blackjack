using SayedHa.Blackjack.Shared.Betting;
using SayedHa.Blackjack.Shared.Blackjack.Players;
using SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    /// <summary>
    /// This is what will run the genetic algorithm to try 
    /// and build the best strategy.
    /// </summary>
    public class StrategyBuilder {
        public StrategyBuilder() : this (new StrategyBuilderSettings()) { }
        public StrategyBuilder(StrategyBuilderSettings settings) {
            Settings = settings;
        }

        public StrategyBuilderSettings Settings { get; set; }

        protected internal List<BlackjackStrategyTree> AllStrategiesTested { get; set; } = new List<BlackjackStrategyTree>();

        public List<BlackjackStrategyTree> FindBestStrategies() {
            // 1. setup
            //      create many strategies and evaluate them
            // 2. Select parents => crossover
            // 3. Mutate offspring
            // 4. Merge main population and offspring
            // 5. Evaluate, sort and select
            // 6. Check exit criteria, if not satisfied go to step 2

            // 1. Setup
            var factory = BlackjackStrategyTreeFactory.GetInstance(Settings.UseRandomNumberGenerator);
            var initialPopulationOfStrategiesList = new List<BlackjackStrategyTree>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for(int i = 0; i < Settings.NumStrategiesForFirstGeneration; i++) {
                initialPopulationOfStrategiesList.Add(factory.CreateNewRandomTree());
            }
            stopwatch.Stop();
            var elapsedTimeStr = stopwatch.ElapsedMilliseconds;

            // evaluate the strategies by playing a set number of games
            var bankroll = new Bankroll(Settings.InitialBankroll, NullLogger.Instance);
            var gameRunner = new GameRunner(NullReporter.Instance);
            var bettingStrategy = new FixedBettingStrategy(bankroll, Settings.BetAmount);
            var discardFirstCard = true;
            stopwatch.Restart();
            foreach (var strategy in initialPopulationOfStrategiesList) {
                var pf = new StrategyBuilderParticipantFactory(strategy, bettingStrategy, Settings, NullLogger.Instance);
                var game = gameRunner.CreateNewGame(Settings.NumDecks, 1, pf, discardFirstCard);
                for(int i = 0; i < Settings.NumHandsToPlayForEachStrategy; i++) {
                    gameRunner.PlayGame(game);
                }

                // record the fitness score
                strategy.FitnessScore = game.Opponents[0].BettingStrategy.Bankroll.DollarsRemaining;
            }
            // var pf = new StrategyBuilderParticipantFactory()
            // AllStrategiesTested.AddRange(initialPopulationOfStrategiesList);
            stopwatch.Stop();
            var elapsedTimeStr2 = stopwatch.ElapsedMilliseconds;


            return null;
        }

    }
    public class StrategyBuilderSettings
    {
        public int NumDecks { get; set; } = 4;
        // TODO: Get this from somewhere.
        public bool UseRandomNumberGenerator { get; set; } = true;
        public int NumStrategiesForFirstGeneration { get; set; } = 750;
        public int NumHandsToPlayForEachStrategy { get; set; } = 1000;
        public int InitialBankroll { get; set; } = 10000;
        public int BetAmount { get; set; } = 5;
    }
}
