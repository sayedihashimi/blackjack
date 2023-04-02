using SayedHa.Blackjack.Shared.Betting;
using SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree;
using SayedHa.Blackjack.Shared.Players;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    public class StrategyBuilder2 {
        public StrategyBuilder2() : this(new StrategyBuilderSettings()) { }
        public StrategyBuilder2(StrategyBuilderSettings settings) {
            Settings = settings;
        }
        public StrategyBuilderSettings Settings { 
            get; 
            init; 
        }

        public List<NextHandActionArray> FindBestStrategies(int numToReturn) {
            var initialPopulation = NextHandActionArrayFactory.GetInstance().CreateRandomStrategies(Settings.NumStrategiesForFirstGeneration).ToList();

            var gameRunner = new GameRunner(NullReporter.Instance);
            var bankroll = new Bankroll(Settings.InitialBankroll, NullLogger.Instance);
            var bettingStrategy = new FixedBettingStrategy(bankroll, Settings.BetAmount);

            var maxNumGenerations = Settings.MaxNumberOfGenerations;
            var currentGeneration = 1;
            var mutationRate = Settings.InitialMutationRate;
            var stopwatch = new Stopwatch();
            do {
                stopwatch.Restart();
                if (!Settings.AllConsoleOutputDisabled) {
                    Console.Write($"generation: {currentGeneration}");
                }
                bankroll = new Bankroll(Settings.InitialBankroll, NullLogger.Instance);
                bettingStrategy = new FixedBettingStrategy(bankroll, Settings.BetAmount);
                PlayAndEvaluate(Settings.NumHandsToPlayForEachStrategy,
                                initialPopulation,
                                gameRunner,
                                bankroll,
                                bettingStrategy);
                initialPopulation.Sort(NextHandActionArray.NextHandActionArrayComparison);

                // trim down the population to ensure it doesn't get too big
                if(initialPopulation.Count > Settings.NumStrategiesForFirstGeneration) {
                    initialPopulation = initialPopulation.GetRange(0, Settings.NumStrategiesForFirstGeneration);
                }
                if (!Settings.AllConsoleOutputDisabled) {
                    Console.Write(". top scores: ");
                    Console.Write($"{initialPopulation[0].FitnessScore}{initialPopulation[0].Name}, ");
                    Console.Write($"{initialPopulation[1].FitnessScore}{initialPopulation[1].Name}, ");
                    Console.Write($"{initialPopulation[2].FitnessScore}{initialPopulation[2].Name}, ");
                    Console.Write($"{initialPopulation[3].FitnessScore}{initialPopulation[3].Name}, ");
                    Console.Write($"{initialPopulation[4].FitnessScore}{initialPopulation[4].Name}, ");
                    Console.Write($"{initialPopulation[5].FitnessScore}{initialPopulation[5].Name}, ");
                    Console.Write($"{initialPopulation[5].FitnessScore}{initialPopulation[6].Name}, ");
                    Console.Write($"{initialPopulation[5].FitnessScore}{initialPopulation[7].Name}, ");
                    Console.Write($"{initialPopulation[5].FitnessScore}{initialPopulation[8].Name}, ");
                    Console.Write($"{initialPopulation[5].FitnessScore}{initialPopulation[9].Name}");
                }

                var parents = SelectParents(initialPopulation, Settings.NumStrategiesToGoToNextGeneration);
                var children = ProduceOffspring(parents, initialPopulation.Count - parents.Count);
                MutateOffspring(children!, mutationRate);

                // TODO: Look for possible duplicates before adding
                initialPopulation.AddRange(children);

                stopwatch.Stop();
                if (!Settings.AllConsoleOutputDisabled) {
                    Console.WriteLine($" [{stopwatch.Elapsed.ToString("mm\\:ss")}]");
                }

                currentGeneration++;

            } while (currentGeneration < maxNumGenerations);
            // run another PlayAndEvaluate to evaluate the last set of offspring
            bankroll = new Bankroll(Settings.InitialBankroll, NullLogger.Instance);
            bettingStrategy = new FixedBettingStrategy(bankroll, Settings.BetAmount);
            PlayAndEvaluate(Settings.NumHandsToPlayForEachStrategy,
                                initialPopulation,
                                gameRunner,
                                bankroll,
                                bettingStrategy);
            initialPopulation.Sort(NextHandActionArray.NextHandActionArrayComparison);

            var topStrategies = new List<NextHandActionArray>(numToReturn);
            for (int i = 0; i < numToReturn; i++) {
                if (i >= initialPopulation.Count) {
                    break;
                }
                //if (initialPopulation[i].GetTreeIdString() == basicStrategyTree.GetTreeIdString()) {
                //    initialPopulation[i].Name = "=bs";
                //}
                topStrategies.Add(initialPopulation[i]);
            }

            return topStrategies;
        }
        protected internal void MutateOffspring(List<NextHandActionArray>children, int mutationRate) {
            // TODO: implement later
        }
        public void PlayAndEvaluate(int numGamesToPlay, List<NextHandActionArray> strategies, GameRunner gameRunner, Bankroll bankroll, BettingStrategy bettingStrategy) {
            Debug.Assert(numGamesToPlay > 0);
            Debug.Assert(strategies?.Count > 0);
            var pOptions = new ParallelOptions {
                MaxDegreeOfParallelism = Settings.MtMaxNumThreads
            };
            if (Settings.EnableMultiThreads) {
                Parallel.ForEach<NextHandActionArray>(strategies, pOptions, s => ProcessStrategy(s, gameRunner, bankroll, bettingStrategy));
            }
            else {
                foreach (var strategy in strategies) {
                    ProcessStrategy(strategy, gameRunner, bankroll, bettingStrategy);
                }
            }
        }

        protected internal void ProcessStrategy(NextHandActionArray strategy, GameRunner gameRunner, Bankroll bankroll, BettingStrategy bettingStrategy) {
            if(strategy.FitnessScore == null || strategy.FitnessScore.HasValue) {
                var pf = new StrategyBuilder2ParticipantFactory(strategy, bettingStrategy, Settings, NullLogger.Instance);
                var game = gameRunner.CreateNewGame(Settings.NumDecks, 1, pf, true);
                PlayGames(Settings.NumHandsToPlayForEachStrategy, gameRunner, game);
                strategy.FitnessScore = Evaluate(game);
            }
        }

        protected internal void PlayGames(int numGamesToPlay, GameRunner gameRunner, Game game) {
            Debug.Assert(numGamesToPlay > 0);
            Debug.Assert(gameRunner is object);
            Debug.Assert(game is object);

            if (numGamesToPlay <= 0) {
                throw new ArgumentException($"{nameof(numGamesToPlay)} must be greater than zero. Passed in value: '{numGamesToPlay}'");
            }
            for (int i = 0; i < numGamesToPlay; i++) {
                gameRunner.PlayGame(game);
            }
        }
        protected internal float Evaluate(Game game) {
            Debug.Assert(game is not null);
            // TODO: looks like DollarsRemaining is not working correctly, needs investigation
            // workaround for now.
            return game.Opponents[0].AllHandsBetResult;
        }
        protected internal List<NextHandActionArray> SelectParents(List<NextHandActionArray> strategies, int numParents) {
            Debug.Assert(strategies?.Count > 0);
            Debug.Assert(numParents > 0);
            var list = new List<NextHandActionArray>();
            var currentIndex = 0;
            // sort the list
            // strategies.Sort(strategies[0].GetBlackjackTreeComparison());
            foreach (var item in strategies) {
                if (currentIndex++ >= numParents) {
                    break;
                }
                list.Add(item);
            }

            return list;
        }
        protected internal List<NextHandActionArray> ProduceOffspring(List<NextHandActionArray> parents, int numChildren) {
            var offspring = new List<NextHandActionArray>();

            while(offspring.Count < numChildren) {
                (var parent1Index, var parent2Index) = RandomHelper.Instance.GetTwoRandomNumbersBetween(0, parents.Count);
                var parent1 = parents[parent1Index];
                var parent2 = parents[parent2Index];

                var currentOffspring = ProduceOffspring(parents[parent1Index], parents[parent2Index]);
                offspring.Add(currentOffspring.child1);
                if(offspring.Count < numChildren) {
                    offspring.Add(currentOffspring.child2);
                }
            }

            return offspring;
        }
        protected internal (NextHandActionArray child1, NextHandActionArray child2) ProduceOffspring(NextHandActionArray parent1, NextHandActionArray parent2) {
            var child1 = new NextHandActionArray();
            var child2 = new NextHandActionArray();

            // pairs
            (child1.pairHandActionArray, child2.pairHandActionArray) = ProduceOffspringArray(parent1.pairHandActionArray, parent2.pairHandActionArray);
            (child1.softHandActionArray, child2.softHandActionArray) = ProduceOffspringArray(parent1.softHandActionArray, parent2.softHandActionArray);
            (child1.hardTotalHandActionArray, child2.hardTotalHandActionArray) = ProduceOffspringArray(parent1.hardTotalHandActionArray, parent2.hardTotalHandActionArray);

            return (child1, child2);
        }
        protected internal (int[,] child1Array, int[,] child2Array) ProduceOffspringArray(int[,] parent1Array, int[,] parent2Array) {
            var child1 = new int[parent1Array.GetLength(0), parent1Array.GetLength(1)];
            var child2 = new int[parent1Array.GetLength(0), parent1Array.GetLength(1)];

            var numAllCells = parent1Array.Length;
            var numCutoff = (int)Math.Floor(numAllCells / 2F);
            var currentIndex = 0;
            for (int dealerIndex = 0; dealerIndex < parent1Array.GetLength(0); dealerIndex++) {
                for (int pairCardIndex = 0; pairCardIndex < parent1Array.GetLength(1); pairCardIndex++) {
                    var valueParent1 = parent1Array[dealerIndex, pairCardIndex];
                    var valueParent2 = parent2Array[dealerIndex, pairCardIndex];

                    if (currentIndex < numCutoff) {
                        // child1 get the Split value from parent1 and child2 gets the Split value from parent 2
                        child1[dealerIndex, pairCardIndex] = valueParent1;
                        child2[dealerIndex, pairCardIndex] = valueParent2;
                    }
                    else {
                        // child1 get the Split value from parent2 and child2 gets the Split value from parent 1
                        child1[dealerIndex, pairCardIndex] = valueParent2;
                        child2[dealerIndex, pairCardIndex] = valueParent1;
                    }

                    currentIndex++;
                }
            }

            return (child1, child2);
        }
    }
    
    public class StrategyBuilder2Player : Player {
        public StrategyBuilder2Player(NextHandActionArray strategy) {
            Strategy = strategy;
        }
        public NextHandActionArray Strategy { get; init; }

        public override HandActionAndReason GetNextAction(Hand hand, DealerHand dealerHand, int dollarsRemaining) {
            var opCardNumbers = new CardNumber[hand.DealtCards.Count];
            for(int i = 0; i < hand.DealtCards.Count; i++) {
                opCardNumbers[i] = hand.DealtCards[i].Number;
            }
            if(opCardNumbers.Length == 2) {
                return new HandActionAndReason(Strategy.GetHandAction(dealerHand.DealersVisibleCard!.Number, opCardNumbers[0], opCardNumbers[1]));
            }
            else {
                return new HandActionAndReason(Strategy.GetHandAction(dealerHand.DealersVisibleCard!.Number, opCardNumbers));
            }
        }
    }
    public class StrategyBuilder2ParticipantFactory: ParticipantFactory {
        public StrategyBuilder2ParticipantFactory(NextHandActionArray strategy, BettingStrategy bettingStrategy, StrategyBuilderSettings settings, ILogger logger):
            base(bettingStrategy, OpponentPlayStrategy.Custom, logger) {
            Strategy = strategy;
            Settings = settings;
        }
        protected internal NextHandActionArray Strategy { get; init; }
        protected internal StrategyBuilderSettings Settings { get; init; }

        public override Participant CreateNewOpponent(OpponentPlayStrategy strategy, ILogger logger) => new StrategyBuilderParticipant(
            ParticipantRole.Player,
            new StrategyBuilder2Player(Strategy),
            new Bankroll(Settings.InitialBankroll, NullLogger.Instance));
    }
    public class StrategyBuilderParticipant : Participant {
        // TODO: would be better to get the 5 from settings
        public StrategyBuilderParticipant(ParticipantRole role, Player player, Bankroll bankroll) : base(role, player, new FixedBettingStrategy(bankroll, 5)) {

        }
    }
}
