using System.Collections.Concurrent;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    public class NextHandActionArrayFactory {
        private NextHandActionConverter Converter { get; } = NextHandActionConverter.Instance;
        protected internal RandomHelper RandomHelper { get; set; } = new RandomHelper();
        public static NextHandActionArrayFactory Instance { get; } = new NextHandActionArrayFactory();

        public StrategyBuilderSettings Settings { get; set; } = new StrategyBuilderSettings();

        private static readonly int HandActionHitValue = 2;
        private static readonly int HandActionStandValue = 3;
        public NextHandActionArray CreateRandomStrategy(bool smartStrategy = false) {
            var nhaa = new NextHandActionArray();

            RandomHelper.RandomizeArray(nhaa.pairHandActionArray, 1, 2 + 1);
            RandomHelper.RandomizeArray(nhaa.softHandActionArray, 1, 3 + 1);
            RandomHelper.RandomizeArray(nhaa.hardTotalHandActionArray, 1, 3 + 1);

            if (smartStrategy) {
                ApplySmartDefaults(nhaa);
            }

            return nhaa;
        }

        protected internal void ApplySmartDefaults(NextHandActionArray nhaa) {
            for (int dealerIndex = 0; dealerIndex < nhaa.hardTotalHandActionArray.GetLength(0); dealerIndex++) {
                // always hit on 3 - 8
                for (int hardIndex = 0; hardIndex <= 5; hardIndex++) {
                    nhaa.hardTotalHandActionArray[dealerIndex, hardIndex] = HandActionHitValue;
                }
                // always stand on 17 - 20 14 - 17
                for (int hardIndex = 14; hardIndex <= 17; hardIndex++) {
                    nhaa.hardTotalHandActionArray[dealerIndex, hardIndex] = HandActionStandValue;
                }
            }
        }

        public IEnumerable<NextHandActionArray> CreateRandomStrategies(int numStrategies) {
            for(int i = 0; i<numStrategies; i++) {
                yield return CreateRandomStrategy(Settings.CreateSmartRandomStrategies);
            }
        }
        // CreateRandomStrategies2 is slower than calling CreateRandomStrategies and then ToList(). See CreateRandomStrategiesBenchmarks
        protected internal List<NextHandActionArray> CreateRandomStrategies2(int numStrategies) {
            ConcurrentBag<NextHandActionArray> randomStrategies = new();

            var options = new ParallelOptions {
                // TODO: get this from settings
                MaxDegreeOfParallelism = 72
            };

            Parallel.For(0, numStrategies, options, i => {
                randomStrategies.Add(CreateRandomStrategy(Settings.CreateSmartRandomStrategies));
            });

            return randomStrategies.ToList();
        }

        public NextHandActionArray CreateStrategyWithAllHits(bool allSplits) => CreateStrategyWithAll(HandAction.Hit, allSplits);
        public NextHandActionArray CreateStrategyWithAllStands(bool allSplits) => CreateStrategyWithAll(HandAction.Stand, allSplits);
        public NextHandActionArray CreateStrategyWithAll(HandAction handAction, bool allSplits) {
            int hitValue = Converter.GetIntFor(handAction);
            var splitValue = Converter.ConvertBoolToInt(allSplits);

            var nhaa = new NextHandActionArray();
            SetAllCellsTo(hitValue, nhaa.hardTotalHandActionArray);
            SetAllCellsTo(hitValue, nhaa.softHandActionArray);
            SetAllCellsTo(splitValue, nhaa.pairHandActionArray);

            return nhaa;
        }

        protected internal void SetAllCellsTo(int value, int[,] valueArray) {
            for (var i = 0; i < valueArray.GetLength(0); i++) {
                for (var j = 0; j < valueArray.GetLength(1); j++) {
                    valueArray[i, j] = value;
                }
            }
        }
    }
}
