using SayedHa.Blackjack.Shared.Betting;
using SayedHa.Blackjack.Shared.Blackjack.Players;
using SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree;
using SayedHa.Blackjack.Shared.Extensions;
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
            
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var initialPopulationOfStrategiesList = CreateRandomTrees(Settings.NumStrategiesForFirstGeneration);
            stopwatch.Stop();
            var elapsedTimeStr = stopwatch.ElapsedMilliseconds;

            // evaluate the strategies by playing a set number of games
            var bankroll = new Bankroll(Settings.InitialBankroll, NullLogger.Instance);
            var gameRunner = new GameRunner(NullReporter.Instance);
            var bettingStrategy = new FixedBettingStrategy(bankroll, Settings.BetAmount);
            stopwatch.Restart();

            PlayAndEvaluate(Settings.NumHandsToPlayForEachStrategy, initialPopulationOfStrategiesList, gameRunner, bankroll, bettingStrategy);
            // sort the list with highest fitness first
            initialPopulationOfStrategiesList.Sort(initialPopulationOfStrategiesList[0].GetBlackjackTreeComparison());
            var parentStrategiesList = SelectParents(initialPopulationOfStrategiesList, Settings.NumStrategiesToGoToNextGeneration);
            // need to select parents now

            stopwatch.Stop();
            var elapsedTimeStr2 = stopwatch.ElapsedMilliseconds;

            return null;
        }
        /// <summary>
        /// This will return the selected parents.
        /// The list provided will be sorted when this method returns.
        /// </summary>
        protected internal List<BlackjackStrategyTree> SelectParents(List<BlackjackStrategyTree>strategies, int numParents){
            Debug.Assert(strategies?.Count > 0);
            Debug.Assert(numParents > 0);
            var list = new List<BlackjackStrategyTree>();
            var currentIndex = 0;
            // sort the list
            strategies.Sort(strategies[0].GetBlackjackTreeComparison());
            foreach(var item in strategies){
                if(currentIndex++ >= numParents){
                    break;
                }
                list.Add(item);
            }

            return list;
        }
        protected internal List<BlackjackStrategyTree> ProduceOffspring(List<BlackjackStrategyTree> parents, int numChildren){
            var children = new List<BlackjackStrategyTree>();
            var numParents = parents.Count;

            var newParentList = new List<BlackjackStrategyTree>();
            foreach(var parent in parents){
                newParentList.Add(parent);
            }

            // instead of generating two random index every loop, randomize the parents list
            // and then pick two parents to create offspring.
            // if we get to the end of the list, shuffle and continue.
            newParentList.Shuffle(Settings.UseRandomNumberGenerator);
            var currentIndex = 0;

            while(children.Count < numChildren){
                if(currentIndex >= newParentList.Count - 2){
                    // shuffle the list and reset the index
                    newParentList.Shuffle(Settings.UseRandomNumberGenerator);
                    currentIndex = 0;
                }

                var offspring = ProduceOffspring(newParentList[currentIndex],newParentList[currentIndex + 1]);
                children.Add(offspring.child1);
                if(children.Count < numChildren){
                    children.Add(offspring.child2);
                }
                // move the index forward to get new parents next time
                currentIndex += 2;
            }

            return children;
        }
        protected internal (BlackjackStrategyTree child1, BlackjackStrategyTree child2) ProduceOffspring(BlackjackStrategyTree parent1, BlackjackStrategyTree parent2){
            throw new NotImplementedException();
        }
        /// <summary>
        /// This will play the provided number of games for each strategy with the GameRunner provided.
        /// After the games are played, the FitnessScore is recorded. For any strategy which already has
        /// a FitnessScore, those will be skipped.
        /// The provided list will be sorted by the FitnessScore (descending) when the method returns.
        /// </summary>
        public void PlayAndEvaluate(int numGamesToPlay, List<BlackjackStrategyTree> strategies, GameRunner gameRunner, Bankroll bankroll,BettingStrategy bettingStrategy) {
            Debug.Assert(strategies?.Count > 0);
            Debug.Assert(gameRunner is object);
            // Debug.Assert(game is object);

            var discardFirstCard = true;

            foreach (var strategy in strategies) {
                var pf = new StrategyBuilderParticipantFactory(strategy, bettingStrategy, Settings, NullLogger.Instance);
                var game = gameRunner.CreateNewGame(Settings.NumDecks, 1, pf, discardFirstCard);

                // if it already has a score, skip it
                if(strategy.FitnessScore == null || !strategy.FitnessScore.HasValue) {
                    PlayGames(Settings.NumHandsToPlayForEachStrategy, gameRunner, game);
                    strategy.FitnessScore = game.Opponents[0].BettingStrategy.Bankroll.DollarsRemaining;
                }
            }
        }

        protected internal List<BlackjackStrategyTree> CreateRandomTrees(int numTreesToCreate) {
            Debug.Assert(numTreesToCreate > 0);
            var factory = BlackjackStrategyTreeFactory.GetInstance(Settings.UseRandomNumberGenerator);
            var initialPopulationOfStrategiesList = new List<BlackjackStrategyTree>();
            for (int i = 0; i < Settings.NumStrategiesForFirstGeneration; i++) {
                initialPopulationOfStrategiesList.Add(factory.CreateNewRandomTree());
            }

            return initialPopulationOfStrategiesList;
        }
        /// <summary>
        /// This method will play the number of games specified on the provided game object.
        /// </summary>
        protected internal void PlayGames(int numGamesToPlay, GameRunner gameRunner, Game game) {
            Debug.Assert(numGamesToPlay > 0);
            Debug.Assert(gameRunner is object);
            Debug.Assert(game is object);

            if(numGamesToPlay <= 0) {
                throw new ArgumentException($"{nameof(numGamesToPlay)} must be greater than zero. Passed in value: '{numGamesToPlay}'");
            }
            for(int i = 0; i < numGamesToPlay; i++) {
                gameRunner.PlayGame(game);
            }
        }
    }

    public class StrategyBuilderSettings
    {
        public int NumDecks { get; set; } = 4;
        // TODO: Get this from somewhere.
        public bool UseRandomNumberGenerator { get; set; } = true;
        public int NumStrategiesForFirstGeneration { get; set; } = 1000;
        // half the population, the other half will be offspring
        public int NumStrategiesToGoToNextGeneration {get;set;} = 500;
        public int NumHandsToPlayForEachStrategy { get; set; } = 1000;
        public int InitialBankroll { get; set; } = 10000;
        public int BetAmount { get; set; } = 5;
    }
}
