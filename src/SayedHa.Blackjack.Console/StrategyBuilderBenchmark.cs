using BenchmarkDotNet.Attributes;
using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Betting;
using SayedHa.Blackjack.Shared.Blackjack.Strategy;
using SayedHa.Blackjack.Shared.Extensions;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
// TODO: Get rid of this
using SB = SayedHa.Blackjack.Shared.Blackjack.Strategy.StrategyBuilder;

namespace SayedHa.Blackjack {
    [MemoryDiagnoser(false)]
    public class StrategyBuilderBenchmark {
        [Benchmark]
        public void RunStrategyBuilder01() {
            var result = new SB().FindBestStrategies(5);
        }
        
    }
    [MemoryDiagnoser(false)]
    public class RandomNumberBenchmarks {
        [Benchmark]
        public void ShuffleListDontUseRandomNumberGenerator() {
            for(int j = 0; j < 100; j++) {
                var list = new List<int>();
                for (int i = 0; i < 100; i++) {
                    list.Add(i);
                }
                list.Shuffle(false);
            }
        }
        [Benchmark]
        public void ShuffleListUseRandomNumberGenerator() {
            for (int j = 0; j < 100; j++) {
                var list = new List<int>();
                for (int i = 0; i < 100; i++) {
                    list.Add(i);
                }
                list.Shuffle(false);
            }
        }
    }
    [MemoryDiagnoser(false)]
    public class RandomTreeBenchmarks {
        [Benchmark]
        public void CreateRandomTrees() {
            var num = 100000;
            Console.WriteLine($"Starting for {num}");
            var sb = new StrategyBuilder();
            sb.CreateRandomTrees(num);
        }
    }
    [MemoryDiagnoser(false)]
    public class StrategyTreeParallelBenchmarks {
        [Benchmark]
        public void TestNotParallel() {
            var sb = new StrategyBuilder();
            sb.ExecuteInParallel = false;
            sb.FindBestStrategies(5);
        }
        [Benchmark]
        public void TestParallel() {
            var sb = new StrategyBuilder();
            sb.ExecuteInParallel = true;
            sb.FindBestStrategies(5);
        }
    }
    [MemoryDiagnoser(false)]
    public class StrategyBuilderVersusStrategyBuilder2 {
        private StrategyBuilderSettings Settings { get; } = new StrategyBuilderSettings {
            AllConsoleOutputDisabled = true,
            NumStrategiesForFirstGeneration = 50,
            NumStrategiesToGoToNextGeneration = 20,
            NumHandsToPlayForEachStrategy = 10,
            MaxNumberOfGenerations = 2,
            InitialMutationRate = 25,
            MinMutationRate = 5,
            MutationRateChangePerGeneration = 1,
            EnableMultiThreads = true,
            MtMaxNumThreads = 72,
        };

        [Benchmark]
        public void RunStrategyBuilder() {
            var sb = new StrategyBuilder(Settings);
            _ = sb.FindBestStrategies(5);
        }
        [Benchmark]
        public void RunStrategyBuilder2() {
            var sb2 = new StrategyBuilder2(Settings);
            _ = sb2.FindBestStrategies(5);
        }
    }
    [MemoryDiagnoser(false)]
    public class CreateRandomStrategiesBenchmarks {
        private static readonly int numToCreate = 1000;
        [Benchmark]
        public void RunCreateRandomStrategies() {
            var nhaa = NextHandActionArrayFactory.Instance;
            nhaa.CreateRandomStrategies(numToCreate).ToList();
        }
        [Benchmark]
        public void RunCreateRandomStrategies2() {
            var nhaa = NextHandActionArrayFactory.Instance;
            nhaa.CreateRandomStrategies2(numToCreate);
        }
    }
    [MemoryDiagnoser(false)]
    public class ProduceOffspringBenchmarks {
        public ProduceOffspringBenchmarks() {
            Strategies = NextHandActionArrayFactory.Instance.CreateRandomStrategies(100).ToList();
        }
        private List<NextHandActionArray> Strategies { get; init; }
        [Benchmark]
        public void RunProduceOffspring() {
            var sb = new StrategyBuilder2();
            sb.ProduceOffspring(Strategies, 200);
        }
        [Benchmark]
        public void RunProduceOffspring2() {
            var sb = new StrategyBuilder2();
            sb.ProduceOffspring2(Strategies, 200);
        }
    }
    [MemoryDiagnoser(false)]
    public class MutateOffspringBenchmarks
    {
        [Benchmark]
        public void RunMutateOffspring() {
            var sb = new StrategyBuilder2();
            var strategy = NextHandActionArrayFactory.Instance.CreateStrategyWithAllHits(true);
            sb.MutateOffspring(strategy, 10);
        }
        [Benchmark]
        public void RunCellMutateOffspring() {
            var sb = new StrategyBuilder2();
            var strategy = NextHandActionArrayFactory.Instance.CreateStrategyWithAllHits(true);
            sb.CellMutateOffspring(strategy, 10);
        }
    }
}
