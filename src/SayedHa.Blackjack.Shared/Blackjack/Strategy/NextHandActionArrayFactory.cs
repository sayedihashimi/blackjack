using SayedHa.Blackjack.Shared.Players;
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

        public NextHandActionArray CreateBasicStrategy() {
            var nhaa = new NextHandActionArray();

            // I thought about calling into the BasicStrategyPlayer to dynamically build this, but it didn't work out.
            // I would have to create Hand objects on the fly from the array values and it doesn't make sense to do that.
            var convert = NextHandActionConverter.Instance;
            var s = convert.GetIntFor(HandAction.Stand);
            var h = convert.GetIntFor(HandAction.Hit);
            var d = convert.GetIntFor(HandAction.Double);
            var p = convert.GetIntFor(HandAction.Split);
			nhaa.hardTotalHandActionArray = new int[10, 18]
	        {
                       /*3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 2s */
		        /*2*/  { h, h, h, h, h, h, h, d, d, h, s, s, s, s, s, s, s, s },
		        /*3*/  { h, h, h, h, h, h, d, d, d, h, s, s, s, s, s, s, s, s },
		        /*4*/  { h, h, h, h, h, h, d, d, d, s, s, s, s, s, s, s, s, s },
		        /*5*/  { h, h, h, h, h, h, d, d, d, s, s, s, s, s, s, s, s, s },
		        /*6*/  { h, h, h, h, h, h, d, d, d, s, s, s, s, s, s, s, s, s },
		        /*7*/  { h, h, h, h, h, h, h, d, d, h, h, h, h, h, s, s, s, s },
		        /*8*/  { h, h, h, h, h, h, h, d, d, h, h, h, h, h, s, s, s, s },
		        /*9*/  { h, h, h, h, h, h, h, d, d, h, h, h, h, h, s, s, s, s },
		        /*10*/ { h, h, h, h, h, h, h, h, d, h, h, h, h, h, s, s, s, s },
		        /*A*/  { h, h, h, h, h, h, h, h, d, h, h, h, h, h, s, s, s, s }
	        };
            nhaa.softHandActionArray = new int[10, 8]
	        {
                    /* 2  3  4  5  6  7  8  9  ← player card that's not the Ace */
		      /* 2*/ { h, h, h, h, h, d, s, s },
		      /* 3*/ { h, h, h, h, d, d, s, s },
		      /* 4*/ { h, h, d, d, d, d, s, s },
		      /* 5*/ { d, d, d, d, d, d, s, s },
		      /* 6*/ { d, d, d, d, d, d, d, s },
		      /* 7*/ { h, h, h, h, h, s, s, s },
		      /* 8*/ { h, h, h, h, h, s, s, s },
	          /* 9*/ { h, h, h, h, h, s, s, s },
		      /*10*/ { h, h, h, h, h, s, s, s },
		      /*11*/ { h, h, h, h, h, s, s, s }
	        };

            nhaa.pairHandActionArray = new int[10, 10] {
                    /* 2  3  4  5  6  7  8  9  10 A   ← player pair card */
               /*2*/ { p, p, 0, 0, p, p, p, p, 0, p },
               /*3*/ { p, p, 0, 0, p, p, p, p, 0, p },
               /*4*/ { p, p, 0, 0, p, p, p, p, 0, p },
               /*5*/ { p, p, p, 0, p, p, p, p, 0, p },
               /*6*/ { p, p, p, 0, p, p, p, p, 0, p },
               /*7*/ { p, p, 0, 0, 0, p, p, 0, 0, p },
               /*8*/ { 0, 0, 0, 0, 0, 0, p, p, 0, p },
               /*9*/ { 0, 0, 0, 0, 0, 0, p, p, 0, p },
              /*10*/ { 0, 0, 0, 0, 0, 0, p, 0, 0, p },
               /*A*/ { 0, 0, 0, 0, 0, 0, p, 0, 0, p }
			};

			return nhaa;
        }

		/*
        // using int instead of bool because
        //  1. to be able to tell when the value is not set.
        //  2. to be more consistent with the other item here.
        protected internal int[,] pairHandActionArray = new int[10, 10];
        // soft totals are 2 - 9
        protected internal int[,] softHandActionArray = new int[10, 8];
        // hard totals 3 - 18. probably could actually be 5 - 18 but not currently
        protected internal int[,] hardTotalHandActionArray = new int[10, 18];
         */

		protected internal void SetAllCellsTo(int value, int[,] valueArray) {
            for (var i = 0; i < valueArray.GetLength(0); i++) {
                for (var j = 0; j < valueArray.GetLength(1); j++) {
                    valueArray[i, j] = value;
                }
            }
        }
    }
}
