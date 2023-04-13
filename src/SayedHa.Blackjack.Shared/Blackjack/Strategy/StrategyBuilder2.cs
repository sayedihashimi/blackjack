using SayedHa.Blackjack.Shared.Betting;
using SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree;
using SayedHa.Blackjack.Shared.Players;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    public class StrategyBuilder2 {
        public StrategyBuilder2() : this(new StrategyBuilderSettings()) { }
        public StrategyBuilder2(StrategyBuilderSettings settings) {
            Settings = settings;
            Console.CancelKeyPress += (sender, eventArgs) => {
                CancelSearch = true;
                Console.WriteLine("stopping operation");
                eventArgs.Cancel = true;
            };
        }
        public StrategyBuilderSettings Settings { 
            get; 
            init; 
        }

        protected internal bool CancelSearch { get; set; } = false;

        public List<NextHandActionArray> FindBestStrategies2(int numToReturn) {
            CancelSearch = false;
            // ignoring numToReturn for now, will only return the top strategy

            float bestOverallFitness = float.MinValue;
            NextHandActionArray bestOverallStrategy = null;
            float bestAverageGenerationFitness = float.MinValue;

            // first generation is all random strategies
            List<NextHandActionArray> currentGeneration = NextHandActionArrayFactory.Instance.CreateRandomStrategies(Settings.NumStrategiesForFirstGeneration).ToList(); ;
            List<NextHandActionArray> nextGeneration = new ();

            var gameRunner = new GameRunner(NullReporter.Instance);
            
            var maxNumGenerations = Settings.MaxNumberOfGenerations;
            int numGeneration = 0;

            // var mutationRate = Settings.InitialMutationRate;
            // var mutationRateChange = Settings.MutationRateChangePerGeneration;
            var cellMutationNumCellsToChange = Settings.CellMutationNumCellsToChangePerChart;
            var cellMutationRateChangePerGen = Settings.CellMutationRateChangePerGeneration;
            var stopwatch = new Stopwatch();
            while (!CancelSearch) {
                stopwatch.Restart();
                // evaluate all the strategies
                var bankroll = new Bankroll(Settings.InitialBankroll, NullLogger.Instance);
                var bettingStrategy = new FixedBettingStrategy(bankroll, Settings.BetAmount);
                PlayAndEvaluate(Settings.NumHandsToPlayForEachStrategy,
                                currentGeneration,
                                gameRunner,
                                bankroll,
                                bettingStrategy);

                // loop through all the strategies to record the best strategy
                float bestGenerationFitnessScore = float.MinValue;
                NextHandActionArray bestGenerationStrategy;
                foreach (var strategy in currentGeneration) {
                    if(strategy.FitnessScore > bestGenerationFitnessScore) {
                        bestGenerationFitnessScore = strategy.FitnessScore.Value;
                        bestGenerationStrategy = strategy;

                        if(strategy.FitnessScore > bestOverallFitness) {
                            bestOverallFitness = strategy.FitnessScore.Value;
                            bestOverallStrategy = strategy;
                        }
                    }
                }

                if (!Settings.AllConsoleOutputDisabled) {
                    Console.Write($"Gen: {numGeneration}. best this generation: {bestGenerationFitnessScore}. Best overall: {bestOverallFitness}. Mutation rate: '{cellMutationNumCellsToChange}'");
                }

                // check to see if we are done
                if (numGeneration >= maxNumGenerations) {
                    if (!Settings.AllConsoleOutputDisabled) {
                        Console.WriteLine($" [{stopwatch.Elapsed.ToString("mm\\:ss")}]");
                    }
                    break;
                }

                // get the nextGeneration setup
                nextGeneration.Clear();

                // add the bestOverall
                nextGeneration.Add(bestOverallStrategy!);

                // clone best strategy, change a single cell and add it to the next generation
                var bestClone = bestOverallStrategy!.Clone();
                CellMutateOffspring(bestClone, 1);
                nextGeneration.Add(bestClone);

                // TODO: later add the top X strategies to carry on to the next generation


                while (nextGeneration.Count < currentGeneration.Count) {
                    var parents = GetTwoParentsTournament(currentGeneration);
                    var children = ProduceOffspring(parents.parent1, parents.parent2);
                    // MutateOffspring(children.child1, mutationRate);
                    // MutateOffspring(children.child2, mutationRate);
                    CellMutateOffspring(children.child1, (int)Math.Round(cellMutationNumCellsToChange));
                    CellMutateOffspring(children.child2, (int)Math.Round(cellMutationNumCellsToChange));
                    nextGeneration.Add(children.child1);
                    nextGeneration.Add(children.child2);
                }

                if(cellMutationNumCellsToChange != Settings.CellMutationMinNumCellsToChangePerChart) {
                    cellMutationNumCellsToChange -= cellMutationRateChangePerGen;

                    if(cellMutationNumCellsToChange < Settings.CellMutationMinNumCellsToChangePerChart) {
                        cellMutationNumCellsToChange = Settings.CellMutationMinNumCellsToChangePerChart;
                    }
                }

                if (!Settings.AllConsoleOutputDisabled) {
                    Console.WriteLine($" [{stopwatch.Elapsed.ToString("mm\\:ss")}]");
                }

                currentGeneration.Clear();
                foreach(var strategy in nextGeneration) {
                    currentGeneration.Add(strategy);
                }

                numGeneration++;
            }

            return new List<NextHandActionArray> { bestOverallStrategy! };
        }

        public List<NextHandActionArray> FindBestStrategies(int numToReturn) {
            var initialPopulation = NextHandActionArrayFactory.Instance.CreateRandomStrategies(Settings.NumStrategiesForFirstGeneration).ToList();

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
        protected internal void CellMutateOffspring(NextHandActionArray child, int numCellsToMutatePerChart) {
            if(numCellsToMutatePerChart < 1) { return; }

            for(int i  = 0; i < numCellsToMutatePerChart; i++) {
                // splits
                var splitDealerIndex = RandomHelper.Instance.GetRandomIntBetween(0, child.pairHandActionArray.GetLength(0));
                var splitPairIndex = RandomHelper.Instance.GetRandomIntBetween(0, child.pairHandActionArray.GetLength(1));
                child.pairHandActionArray[splitDealerIndex, splitPairIndex] = 
                    RandomHelper.Instance.GetRandomBool() ? 1 : 2;
                // hard totals
                var hardTotalDealerIndex = RandomHelper.Instance.GetRandomIntBetween(0, child.hardTotalHandActionArray.GetLength(0));
                var hardTotalIndex = RandomHelper.Instance.GetRandomIntBetween(0, child.hardTotalHandActionArray.GetLength(1));
                child.hardTotalHandActionArray[hardTotalDealerIndex, hardTotalIndex] = 
                    RandomHelper.Instance.GetRandomIntBetween(1, 3 + 1);
                // soft totals
                var softTotalDealerIndex = RandomHelper.Instance.GetRandomIntBetween(0, child.softHandActionArray.GetLength(0));
                var softTotalIndex = RandomHelper.Instance.GetRandomIntBetween(0, child.softHandActionArray.GetLength(1));
                child.hardTotalHandActionArray[softTotalDealerIndex, softTotalIndex] =
                    RandomHelper.Instance.GetRandomIntBetween(1, 3 + 1);
            }
        }

        protected internal void MutateOffspring(List<NextHandActionArray>children, int mutationRate) {
            if(mutationRate <= 0) {
                return;
            }

            foreach(var child in children) {
                MutateOffspring(child, mutationRate);
            }
        }

        protected internal void MutateOffspring(NextHandActionArray child, int mutationRate) {
            // splits
            for (int dealerSplitIndex = 0; dealerSplitIndex < child.pairHandActionArray.GetLength(0); dealerSplitIndex++) {
                for (int pairIndex = 0; pairIndex < child.pairHandActionArray.GetLength(1); pairIndex++) {
                    // should be assigned?
                    var value = RandomHelper.Instance.GetRandomIntBetween(0, 100 + 1);
                    if (value < mutationRate) {
                        child.pairHandActionArray[dealerSplitIndex, pairIndex] = RandomHelper.Instance.GetRandomBool() ? 1 : 2;
                    }
                }
            }
            // soft totals
            for (int dealerIndex = 0; dealerIndex < child.softHandActionArray.GetLength(0); dealerIndex++) {
                for (int softIndex = 0; softIndex < child.softHandActionArray.GetLength(1); softIndex++) {
                    var value = RandomHelper.Instance.GetRandomIntBetween(0, 100 + 1);
                    if (value < mutationRate) {
                        child.softHandActionArray[dealerIndex, softIndex] = RandomHelper.Instance.GetRandomIntBetween(1, 3 + 1);
                    }
                }
            }
            // hard totals
            for (int dealerIndex = 0; dealerIndex < child.hardTotalHandActionArray.GetLength(0); dealerIndex++) {
                for (int softIndex = 0; softIndex < child.hardTotalHandActionArray.GetLength(1); softIndex++) {
                    var value = RandomHelper.Instance.GetRandomIntBetween(0, 100 + 1);
                    if (value < mutationRate) {
                        child.hardTotalHandActionArray[dealerIndex, softIndex] = RandomHelper.Instance.GetRandomIntBetween(1, 3 + 1);
                    }
                }
            }
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
            if(strategy.FitnessScore == null || !strategy.FitnessScore.HasValue) {
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

            //if (numGamesToPlay <= 0) {
            //    throw new ArgumentException($"{nameof(numGamesToPlay)} must be greater than zero. Passed in value: '{numGamesToPlay}'");
            //}
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
        protected internal (NextHandActionArray parent1, NextHandActionArray parent2) GetTwoParentsTournament(List<NextHandActionArray>strategies) {    
            (int index, NextHandActionArray strategy) GetAParent(List<NextHandActionArray> strategies) {
                NextHandActionArray bestStrategy = null;
                float bestStrategyFitness = float.MinValue;
                int bestStrategyIndex = int.MinValue;
                for (int i = 0; i < Settings.TournamentSize; i++) {
                    int randomIndex = RandomHelper.Instance.GetRandomIntBetween(0, strategies.Count);
                    if (randomIndex == bestStrategyIndex) {
                        continue;
                    }
                    var randomStrategy = strategies[randomIndex];
                    if (randomStrategy.FitnessScore!.Value > bestStrategyFitness) {
                        bestStrategy = randomStrategy;
                        bestStrategyIndex = randomIndex;
                        bestStrategyFitness = randomStrategy.FitnessScore!.Value;
                    }
                }
                return (bestStrategyIndex, bestStrategy!);
            }

            var parent1 = GetAParent(strategies);
            var parent2 = GetAParent(strategies);
            while(parent2.index == parent1.index) {
                parent2 = GetAParent(strategies);
            }

            return (parent1.strategy, parent2.strategy);
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
        // this is slower than ProduceOffspring
        protected internal ConcurrentBag<NextHandActionArray> ProduceOffspring2(List<NextHandActionArray> parents, int numChildren) {
            ConcurrentBag<NextHandActionArray> result = new();

            Parallel.For(result.Count, numChildren, i => {
                (var parent1Index, var parent2Index) = RandomHelper.Instance.GetTwoRandomNumbersBetween(0, parents.Count);
                var parent1 = parents[parent1Index];
                var parent2 = parents[parent2Index];

                var currentOffspring = ProduceOffspring(parents[parent1Index], parents[parent2Index]);
                result.Add(currentOffspring.child1);
                if (result.Count < numChildren) {
                    result.Add(currentOffspring.child2);
                }
            });

            return result;
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
