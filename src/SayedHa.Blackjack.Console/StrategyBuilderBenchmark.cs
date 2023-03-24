using BenchmarkDotNet.Attributes;
using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Betting;
using SayedHa.Blackjack.Shared.Blackjack.Strategy;
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
}
