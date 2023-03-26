using SayedHa.Blackjack.Shared.Betting;
using SayedHa.Blackjack.Shared.Blackjack.Exceptions;
using SayedHa.Blackjack.Shared.Blackjack.Players;
using SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree;
using SayedHa.Blackjack.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    /// <summary>
    /// This is what will run the genetic algorithm to try 
    /// and build the best strategy.
    /// </summary>
    public class StrategyBuilder {
        public StrategyBuilder() : this(new StrategyBuilderSettings()) { }
        public StrategyBuilder(StrategyBuilderSettings settings) {
            Settings = settings;
        }

        public StrategyBuilderSettings Settings { get; set; }

        protected internal List<BlackjackStrategyTree> AllStrategiesTested { get; set; } = new List<BlackjackStrategyTree>();

        //public async Task<List<BlackjackStrategyTree>> FindBestStrategiesMtAsync(int numToReturn) {
        //    // 1. setup
        //    //      create many strategies and evaluate them
        //    // 2. Select parents => crossover
        //    // 3. Mutate offspring
        //    // 4. Merge main population and offspring
        //    // 5. Evaluate, sort and select
        //    // 6. Check exit criteria, if not satisfied go to step 2

        //    // 1. Setup
        //    var factory = BlackjackStrategyTreeFactory.GetInstance(Settings.UseRandomNumberGenerator);

        //    var stopwatch = new Stopwatch();
        //    stopwatch.Start();
        //    // var initialPopulationOfStrategiesList = CreateRandomTrees(Settings.NumStrategiesForFirstGeneration);

        //    // TODO: Remove this
        //    // initialPopulationOfStrategiesList.RemoveAt(0);
        //    // initialPopulationOfStrategiesList.Add(BlackjackStrategyTreeFactory.GetInstance(true).GetBasicStrategyTree());
        //    // TODO: end remove

        //    stopwatch.Stop();
        //    var elapsedTimeStr = stopwatch.ElapsedMilliseconds;

        //    // evaluate the strategies by playing a set number of games
        //    var bankroll = new Bankroll(Settings.InitialBankroll, NullLogger.Instance);
        //    var gameRunner = new GameRunner(NullReporter.Instance);
        //    var bettingStrategy = new FixedBettingStrategy(bankroll, Settings.BetAmount);
        //    stopwatch.Restart();
        //    var maxNumGenerations = Settings.MaxNumberOfGenerations;

        //    var currentGeneration = 1;
        //    var mutationRate = Settings.InitialMutationRate;
        //    var mutationRateChange = Settings.MutationRateChangePerGeneration;
        //    //var allStrategies = new List<BlackjackStrategyTree>();
        //    //allStrategies.AddRange(initialPopulationOfStrategiesList);
            
        //    var topStrategies = new List<BlackjackStrategyTree>();
        //    do {
        //        Console.WriteLine($"generation: {currentGeneration}");

        //        var evaluatedStrategies = await PlayAndEvalMtAsync(Settings.NumHandsToPlayForEachStrategy, gameRunner, bankroll, bettingStrategy);
        //        // PlayAndEvaluate(Settings.NumHandsToPlayForEachStrategy, initialPopulationOfStrategiesList, gameRunner, bankroll, bettingStrategy);
        //        // sort the list with highest fitness first
        //        evaluatedStrategies.Sort(evaluatedStrategies[0].GetBlackjackTreeComparison());
        //        var parentStrategiesList = SelectParents(evaluatedStrategies, Settings.NumStrategiesToGoToNextGeneration);
        //        // need to select parents now
        //        var children = ProduceOffspring(parentStrategiesList, evaluatedStrategies.Count - parentStrategiesList.Count);
        //        MutateOffspring(children, mutationRate);

        //        // combine the parents and the children into a list, evaluate, sort and continue
        //        parentStrategiesList.AddRange(children);
        //        evaluatedStrategies = parentStrategiesList;

        //        // add children to allStrategies, sort and select
        //        //allStrategies.AddRange(children);
        //        //allStrategies.Sort(allStrategies[0].GetBlackjackTreeComparison());
        //        //initialPopulationOfStrategiesList = allStrategies.GetRange(0, allStrategies.Count - Settings.NumStrategiesForFirstGeneration);

        //        // trim all strategies as well so it doesn't get too big
        //        //allStrategies = initialPopulationOfStrategiesList;

        //        // update the mutation rate
        //        if(mutationRate != Settings.MinMutationRate) {
        //            mutationRate = (int)Math.Ceiling(mutationRate * (100 - mutationRateChange) / 100F);
        //            if (mutationRate < Settings.MinMutationRate) {
        //                mutationRate = Settings.MinMutationRate;
        //                // Console.WriteLine("mutation rate at 0");
        //            }
        //        }

        //        evaluatedStrategies.AddRange(topStrategies);
        //        evaluatedStrategies.Sort(evaluatedStrategies[0].GetBlackjackTreeComparison());

        //        topStrategies = evaluatedStrategies.GetRange(0, Settings.NumStrategiesToGoToNextGeneration);

        //        currentGeneration++;
        //    } while (currentGeneration < maxNumGenerations);

        //    // run another PlayAndEvaluate to evaluate the last set of offspring
        //    PlayAndEvaluate(Settings.NumHandsToPlayForEachStrategy, initialPopulationOfStrategiesList, gameRunner, bankroll, bettingStrategy);
        //    initialPopulationOfStrategiesList.Sort(initialPopulationOfStrategiesList[0].GetBlackjackTreeComparison());
        //    stopwatch.Stop();

        //    var elapsedTimeStr2 = stopwatch.ElapsedMilliseconds;

        //    // return the top numToReturn items
        //    var topStrategies = new List<BlackjackStrategyTree>(numToReturn);
        //    for (int i = 0; i < numToReturn; i++) {
        //        if (i >= initialPopulationOfStrategiesList.Count) {
        //            break;
        //        }
        //        topStrategies.Add(initialPopulationOfStrategiesList[i]);
        //    }

        //    return topStrategies;
        //}
        public List<BlackjackStrategyTree> FindBestStrategies(int numToReturn) {
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

            // TODO: Remove this
            initialPopulationOfStrategiesList.RemoveAt(0);
            initialPopulationOfStrategiesList.Add(BlackjackStrategyTreeFactory.GetInstance(true).GetBasicStrategyTree());
            // TODO: end remove

            stopwatch.Stop();
            var elapsedTimeStr = stopwatch.ElapsedMilliseconds;

            // evaluate the strategies by playing a set number of games
            var bankroll = new Bankroll(Settings.InitialBankroll, NullLogger.Instance);
            var gameRunner = new GameRunner(NullReporter.Instance);
            var bettingStrategy = new FixedBettingStrategy(bankroll, Settings.BetAmount);
            stopwatch.Restart();
            var maxNumGenerations = Settings.MaxNumberOfGenerations;

            var currentGeneration = 1;
            var mutationRate = Settings.InitialMutationRate;
            var mutationRateChange = Settings.MutationRateChangePerGeneration;
            var allStrategies = new List<BlackjackStrategyTree>();
            allStrategies.AddRange(initialPopulationOfStrategiesList);
            do {
                Console.WriteLine($"generation: {currentGeneration}");
                PlayAndEvaluate(Settings.NumHandsToPlayForEachStrategy, initialPopulationOfStrategiesList, gameRunner, bankroll, bettingStrategy);
                // sort the list with highest fitness first
                initialPopulationOfStrategiesList.Sort(initialPopulationOfStrategiesList[0].GetBlackjackTreeComparison());
                var parentStrategiesList = SelectParents(initialPopulationOfStrategiesList, Settings.NumStrategiesToGoToNextGeneration);
                // need to select parents now
                var children = ProduceOffspring(parentStrategiesList, initialPopulationOfStrategiesList.Count - parentStrategiesList.Count);
                MutateOffspring(children, mutationRate);

                // combine the parents and the children into a list, evaluate, sort and continue
                parentStrategiesList.AddRange(children);
                initialPopulationOfStrategiesList = parentStrategiesList;

                // add children to allStrategies, sort and select
                //allStrategies.AddRange(children);
                //allStrategies.Sort(allStrategies[0].GetBlackjackTreeComparison());
                //initialPopulationOfStrategiesList = allStrategies.GetRange(0, allStrategies.Count - Settings.NumStrategiesForFirstGeneration);

                // trim all strategies as well so it doesn't get too big
                //allStrategies = initialPopulationOfStrategiesList;

                // update the mutation rate
                if (mutationRate != Settings.MinMutationRate) {
                    mutationRate = (int)Math.Ceiling(mutationRate * (100 - mutationRateChange) / 100F);
                    if (mutationRate < Settings.MinMutationRate) {
                        mutationRate = Settings.MinMutationRate;
                        // Console.WriteLine("mutation rate at 0");
                    }
                }

                currentGeneration++;
            } while (currentGeneration < maxNumGenerations);

            // run another PlayAndEvaluate to evaluate the last set of offspring
            PlayAndEvaluate(Settings.NumHandsToPlayForEachStrategy, initialPopulationOfStrategiesList, gameRunner, bankroll, bettingStrategy);
            initialPopulationOfStrategiesList.Sort(initialPopulationOfStrategiesList[0].GetBlackjackTreeComparison());
            stopwatch.Stop();

            var elapsedTimeStr2 = stopwatch.ElapsedMilliseconds;

            // return the top numToReturn items
            var topStrategies = new List<BlackjackStrategyTree>(numToReturn);
            for (int i = 0; i < numToReturn; i++) {
                if (i >= initialPopulationOfStrategiesList.Count) {
                    break;
                }
                topStrategies.Add(initialPopulationOfStrategiesList[i]);
            }

            return topStrategies;
        }
        private readonly HandAction[] _mutateOffspringHandActions = new HandAction[] { HandAction.Hit, HandAction.Stand, HandAction.Double };
        private readonly CardNumber[] _mutateOffspringCardNumbers = ((CardNumber[])Enum.GetValues(typeof(CardNumber))).ToArray();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offspringTree"></param>
        /// <param name="currentGeneration"></param>
        /// <param name="mutationRate">% number that's represented as a number between (inclusive) 0 and 100.</param>
        protected internal void MutateOffspring(List<BlackjackStrategyTree> offspringTree, int mutationRate) {
            if (mutationRate < 0 || mutationRate > 100) {
                throw new UnexpectedValueException($"mutationRate: '{mutationRate}'");
            }

            if(mutationRate == 0) {
                // nothing to do.
                return;
            }

            // TODO: Maybe it's better to clone the offspringTree and then return that as a new list

            // visit each tree in the soft totals and see if it should be updated
            foreach (var off in offspringTree) {
                // soft totals
                foreach (var aceDealer in off.aceTree.Children!) {
                    foreach (var node in aceDealer.Children!) {
                        if (node is LeafNode<CardNumberOrScore, HandAction> leafNode) {
                            // chance to modify the value is mutationRate
                            var value = CardNumberHelper.GetRandomIntBetween(0, 100 + 1);
                            if (value < mutationRate) {
                                // candidate selected for mutation
                                leafNode.Value = CardNumberHelper.GetRandomHandAction(Settings.UseRandomNumberGenerator, _mutateOffspringHandActions);
                            }
                        }
                        else {
                            throw new UnexpectedNodeTypeException($"Expected LeafNode, received type: {node.GetType().FullName}");
                        }
                    }
                }
                // hard total tree
                foreach (var dealerNode in off.hardTotalTree.Children!) {
                    foreach(var node in dealerNode.Children!) {
                        if (node is LeafNode<CardNumberOrScore, HandAction> leafNode) {
                            // chance to modify the value is mutationRate
                            var value = CardNumberHelper.GetRandomIntBetween(0, 100 + 1);
                            if (value < mutationRate) {
                                // get a random action and assign it, if it's the same as the existing action that's ok too.
                                leafNode.Value = CardNumberHelper.GetRandomHandAction(Settings.UseRandomNumberGenerator, _mutateOffspringHandActions);
                            }
                        }
                        else {
                            throw new UnexpectedNodeTypeException($"Expected LeafNode, received type: {node.GetType().FullName}");
                        }
                    }
                }
                // pairs tree
                for (int dealerIndex = 0; dealerIndex < _mutateOffspringCardNumbers.Length; dealerIndex++) {
                    var dealerCard = _mutateOffspringCardNumbers[dealerIndex];
                    var dealerNode = off.pairTree.Get(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard));

                    for (int pairCardIndex = 0; pairCardIndex < _mutateOffspringCardNumbers.Length; pairCardIndex++) {
                        var pairCard = _mutateOffspringCardNumbers[pairCardIndex];
                        var value = CardNumberHelper.GetRandomIntBetween(0, 100 + 1);
                        if (value < mutationRate) {
                            var cnos = CardNumberHelper.ConvertToCardNumberOrScore(pairCard);
                            
                            var foundNode = dealerNode?.Get(cnos);
                            if (foundNode is object) {
                                // remove that node
                                dealerNode!.Children!.Remove(foundNode);
                            }
                            else {
                                // add it
                                off.AddPairSplitNextAction(dealerCard, pairCard);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// This will return the selected parents.
        /// The list provided will be sorted when this method returns.
        /// </summary>
        protected internal List<BlackjackStrategyTree> SelectParents(List<BlackjackStrategyTree> strategies, int numParents) {
            Debug.Assert(strategies?.Count > 0);
            Debug.Assert(numParents > 0);
            var list = new List<BlackjackStrategyTree>();
            var currentIndex = 0;
            // sort the list
            strategies.Sort(strategies[0].GetBlackjackTreeComparison());
            foreach (var item in strategies) {
                if (currentIndex++ >= numParents) {
                    break;
                }
                list.Add(item);
            }

            return list;
        }
        protected internal List<BlackjackStrategyTree> ProduceOffspring(List<BlackjackStrategyTree> parents, int numChildren) {
            var children = new List<BlackjackStrategyTree>();
            var numParents = parents.Count;

            var newParentList = new List<BlackjackStrategyTree>();
            foreach (var parent in parents) {
                newParentList.Add(parent);
            }

            // instead of generating two random index every loop, randomize the parents list
            // and then pick two parents to create offspring.
            // if we get to the end of the list, shuffle and continue.
            newParentList.Shuffle(Settings.UseRandomNumberGenerator);
            var currentIndex = 0;

            while (children.Count < numChildren) {
                if (currentIndex >= newParentList.Count - 2) {
                    // shuffle the list and reset the index
                    newParentList.Shuffle(Settings.UseRandomNumberGenerator);
                    currentIndex = 0;
                }

                var offspring = ProduceOffspring(newParentList[currentIndex], newParentList[currentIndex + 1]);
                children.Add(offspring.child1);
                if (children.Count < numChildren) {
                    children.Add(offspring.child2);
                }
                // move the index forward to get new parents next time
                currentIndex += 2;
            }

            return children;
        }
        /// <summary>
        /// This will produce two offspring from the given parents.
        /// The offspring will have even nodes from parent 1 and odd nodes from parent 2. Node number
        /// is determined by the index when it's being visited.
        /// </summary>
        protected internal (BlackjackStrategyTree child1, BlackjackStrategyTree child2) ProduceOffspring(BlackjackStrategyTree parent1, BlackjackStrategyTree parent2) {
            var child1 = new BlackjackStrategyTree();
            var child2 = new BlackjackStrategyTree();
            var allCardNumbers = CardDeckFactory.GetAllCardNumbers();

            var allPossiblePairs = CardNumberHelper.GetAllPossiblePairCards();
            // first half comes from parent1 and the second half from parent2

            // 1: pairs
            var pairIndexCuttoff = (int)Math.Floor(allCardNumbers.Length / 2F);
            for (var dealerIndex = 0; dealerIndex < allCardNumbers.Length; dealerIndex++) {
                for (int pairIndex = 0; pairIndex < allPossiblePairs.Count; pairIndex++) {
                    var valueParent1 = parent1.GetNextHandAction(allCardNumbers[dealerIndex], allCardNumbers[pairIndex], allCardNumbers[pairIndex]);
                    var valueParent2 = parent2.GetNextHandAction(allCardNumbers[dealerIndex], allCardNumbers[pairIndex], allCardNumbers[pairIndex]);
                    if (pairIndex < pairIndexCuttoff) {
                        // child1 get the Split value from parent1 and child2 gets the Split value from parent 2
                        if (valueParent1 == HandAction.Split) {
                            child1.AddPairSplitNextAction(allCardNumbers[dealerIndex], allCardNumbers[pairIndex]);
                        }
                        if (valueParent2 == HandAction.Split) {
                            child2.AddPairSplitNextAction(allCardNumbers[dealerIndex], allCardNumbers[pairIndex]);
                        }
                    }
                    else {
                        // child1 get the Split value from parent2 and child2 gets the Split value from parent 1
                        if (valueParent2 == HandAction.Split) {
                            child1.AddPairSplitNextAction(allCardNumbers[dealerIndex], allCardNumbers[pairIndex]);
                        }
                        if (valueParent1 == HandAction.Split) {
                            child2.AddPairSplitNextAction(allCardNumbers[dealerIndex], allCardNumbers[pairIndex]);
                        }
                    }
                }
            }
            // 2: soft totals
            var allSoftTotalCards = CardNumberHelper.GetAllPossibleSoftTotalValues();
            var softTotalCardCuttoffIndex = (int)Math.Floor(allSoftTotalCards.Count / 2F);
            for (var dealerIndex = 0; dealerIndex < allCardNumbers.Length; dealerIndex++) {
                for (var stIndex = 0; stIndex < allSoftTotalCards.Count; stIndex++) {
                    var valueParent1 = parent1.GetFromAceTree(allCardNumbers[dealerIndex], allSoftTotalCards[stIndex]);
                    var valueParent2 = parent1.GetFromAceTree(allCardNumbers[dealerIndex], allSoftTotalCards[stIndex]);
                    if (stIndex < softTotalCardCuttoffIndex) {
                        // child1 get the Split value from parent1 and child2 gets the Split value from parent 2
                        if (valueParent1 is not null) {
                            child1.AddSoftTotalNextAction(allCardNumbers[dealerIndex], allSoftTotalCards[stIndex], valueParent1.Value);
                        }
                        if (valueParent2 is not null) {
                            child2.AddSoftTotalNextAction(allCardNumbers[dealerIndex], allSoftTotalCards[stIndex], valueParent2.Value);
                        }
                    }
                    else {
                        // child1 get the Split value from parent2 and child2 gets the Split value from parent 1
                        if (valueParent2 is not null) {
                            child1.AddSoftTotalNextAction(allCardNumbers[dealerIndex], allSoftTotalCards[stIndex], valueParent2.Value);
                        }
                        if (valueParent1 is not null) {
                            child2.AddSoftTotalNextAction(allCardNumbers[dealerIndex], allSoftTotalCards[stIndex], valueParent1.Value);
                        }
                    }
                }
            }
            // 3: hard totals
            var allHardTotalValues = CardNumberHelper.GetAllPossibleHardTotalValues();
            var hardTotalCardCuttoffIndex = (int)Math.Floor(allHardTotalValues.Count / 2F);
            for (var dealerIndex = 0; dealerIndex < allCardNumbers.Length; dealerIndex++) {
                for (var htIndex = 0; htIndex < allCardNumbers.Length; htIndex++) {
                    var dealerCard = allCardNumbers[dealerIndex];
                    var valueParent1 = parent1.GetOrAddFromHardTotalTree(allCardNumbers[dealerIndex], allHardTotalValues[htIndex]);
                    var valueParent2 = parent2.GetOrAddFromHardTotalTree(allCardNumbers[dealerIndex], allHardTotalValues[htIndex]);
                    var currentHardTotal = allHardTotalValues[htIndex];
                    if (htIndex < hardTotalCardCuttoffIndex) {
                        // child1 get the Split value from parent1 and child2 gets the Split value from parent 2
                        child1.AddHardTotalNextAction(dealerCard, currentHardTotal, valueParent1);
                        child2.AddHardTotalNextAction(dealerCard, currentHardTotal, valueParent2);
                    }
                    else {
                        // child1 get the Split value from parent2 and child2 gets the Split value from parent 1
                        child1.AddHardTotalNextAction(dealerCard, currentHardTotal, valueParent2);
                        child2.AddHardTotalNextAction(dealerCard, currentHardTotal, valueParent1);
                    }
                }
            }

            return (child1, child2);
        }
        /// <summary>
        /// This will play the provided number of games for each strategy with the GameRunner provided.
        /// After the games are played, the FitnessScore is recorded. For any strategy which already has
        /// a FitnessScore, those will be skipped.
        /// The provided list will be sorted by the FitnessScore (descending) when the method returns.
        /// </summary>
        public void PlayAndEvaluate(int numGamesToPlay, List<BlackjackStrategyTree> strategies, GameRunner gameRunner, Bankroll bankroll, BettingStrategy bettingStrategy) {
            Debug.Assert(strategies?.Count > 0);
            Debug.Assert(gameRunner is object);

            var discardFirstCard = true;

            if (ExecuteInParallel) {
                var pOptions = new ParallelOptions {
                    MaxDegreeOfParallelism = Settings.MtMaxNumThreads
                };
                Parallel.ForEach<BlackjackStrategyTree>(strategies, pOptions, s => ProcessStrategy(s, gameRunner, bankroll, bettingStrategy));
            }
            else {
                foreach (var strategy in strategies) {
                    // if it already has a score, skip it
                    if (strategy.FitnessScore == null || !strategy.FitnessScore.HasValue) {
                        var pf = new StrategyBuilderParticipantFactory(strategy, bettingStrategy, Settings, NullLogger.Instance);
                        var game = gameRunner.CreateNewGame(Settings.NumDecks, 1, pf, discardFirstCard);
                        PlayGames(Settings.NumHandsToPlayForEachStrategy, gameRunner, game);
                        // DollarsRemaining isn't working correctly, needs investigation
                        // strategy.FitnessScore = game.Opponents[0].BettingStrategy.Bankroll.DollarsRemaining;
                        strategy.FitnessScore = Evaluate(game);
                    }
                }
            }
        }
        protected internal bool ExecuteInParallel { get; set; } = false;
        protected internal void ProcessStrategy(BlackjackStrategyTree strategy, GameRunner gameRunner, Bankroll bankroll, BettingStrategy bettingStrategy) {
            if (strategy.FitnessScore == null || !strategy.FitnessScore.HasValue) {
                var pf = new StrategyBuilderParticipantFactory(strategy, bettingStrategy, Settings, NullLogger.Instance);
                var game = gameRunner.CreateNewGame(Settings.NumDecks, 1, pf, true);
                PlayGames(Settings.NumHandsToPlayForEachStrategy, gameRunner, game);
                // DollarsRemaining isn't working correctly, needs investigation
                // strategy.FitnessScore = game.Opponents[0].BettingStrategy.Bankroll.DollarsRemaining;
                strategy.FitnessScore = Evaluate(game);
            }
        }

        public async Task<List<BlackjackStrategyTree>> PlayAndEvalMtAsync(int numGamesToPlay, GameRunner gameRunner, Bankroll bankroll, BettingStrategy bettingStrategy){
            // Debug.Assert(strategies?.Count > 0);
            Debug.Assert(gameRunner is object);

            var queue = new BufferBlock<BlackjackStrategyTree>(new DataflowBlockOptions {
                BoundedCapacity = Settings.MtMaxNumStrategiesPerBlock
            });
            var consumerOptions = new ExecutionDataflowBlockOptions {
                BoundedCapacity = 1
            };
            //var consumer = new ActionBlock<BlackjackStrategyTree>(strategy => PlayAndEvalMtConsumer(strategy, gameRunner, bankroll, bettingStrategy), consumerOptions);
            //queue.LinkTo(consumer, new DataflowLinkOptions { PropagateCompletion = true });
            var consumers = new List<ActionBlock<BlackjackStrategyTree>>();
            var consumerCompletionTasks = new List<Task>();
            var consumerTreeProcessedList = new List<List<BlackjackStrategyTree>>();
            for (int i = 0; i < Settings.MtMaxNumThreads; i++) {
                var trees = new List<BlackjackStrategyTree>();
                consumerTreeProcessedList.Add(trees);
                var c = new ActionBlock<BlackjackStrategyTree>(strategy => PlayAndEvalMtConsumer(strategy, trees, numGamesToPlay, gameRunner, bankroll, bettingStrategy), consumerOptions);
                consumers.Add(c);
                queue.LinkTo(c, new DataflowLinkOptions { PropagateCompletion = true });
                consumerCompletionTasks.Add(c.Completion);
            }

            // start the producer
            var producer = Produce(queue, Settings.NumStrategiesForFirstGeneration);
            var allTasksList = new List<Task>(consumerCompletionTasks.Count + 1);
            allTasksList.Add(producer);
            allTasksList.AddRange(consumerCompletionTasks);
            
            await Task.WhenAll(allTasksList);

            var allStrategies = new List<BlackjackStrategyTree>();
            foreach(var tree in consumerTreeProcessedList) {
                allStrategies.Concat(tree);
            }

            return allStrategies;
        }

        protected internal void PlayAndEvalMtConsumer(BlackjackStrategyTree strategy, List<BlackjackStrategyTree>resultList, int numGamesToPlay, GameRunner gameRunner, Bankroll bankroll, BettingStrategy bettingStrategy) {
            if(strategy.FitnessScore == null || !strategy.FitnessScore.HasValue) {
                var pf = new StrategyBuilderParticipantFactory(strategy, bettingStrategy, Settings, NullLogger.Instance);
                var game = gameRunner.CreateNewGame(Settings.NumDecks, 1, pf, true);
                PlayGames(numGamesToPlay, gameRunner, game);
                strategy.FitnessScore = Evaluate(game);
                resultList.Add(strategy);
            }
        }

        protected internal async Task Produce(BufferBlock<BlackjackStrategyTree> queue, int numRandomTreesToProduce) {
            var numCreated = 0;
            var values = CreateRandomTrees(numRandomTreesToProduce);
            for(int i = 0;i<numCreated;i++) {
                // await queue.SendAsync(value);
                await queue.SendAsync(CreateRandomTree());
                numCreated++;
            }
            queue.Complete();
        }

        protected internal float Evaluate(Game game) {
            Debug.Assert(game is not null);
            // TODO: looks like DollarsRemaining is not working correctly, needs investigation
            // workaround for now.
            return game.Opponents[0].AllHandsBetResult;
        }
        protected internal List<BlackjackStrategyTree> CreateRandomTrees(int numTreesToCreate) {
            Debug.Assert(numTreesToCreate > 0);
            var initialPopulationOfStrategiesList = new List<BlackjackStrategyTree>();
            for (int i = 0; i < Settings.NumStrategiesForFirstGeneration; i++) {
                initialPopulationOfStrategiesList.Add(CreateRandomTree());
            }

            return initialPopulationOfStrategiesList;
        }
        protected internal BlackjackStrategyTree CreateRandomTree() => 
            BlackjackStrategyTreeFactory.GetInstance(Settings.UseRandomNumberGenerator).CreateNewRandomTree();
        /// <summary>
        /// This method will play the number of games specified on the provided game object.
        /// </summary>
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
    }
}
