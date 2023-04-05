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
}
