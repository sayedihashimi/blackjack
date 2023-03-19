using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Payloads;
using SayedHa.Blackjack.Shared.Blackjack.Strategy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
// TODO: Get rid of this
using SB = SayedHa.Blackjack.Shared.Blackjack.Strategy.StrategyBuilder;

namespace SayedHa.Blackjack.Tests {
    public class StrategyBuilderTests {
        [Fact]
        public void Test_StrategyBuilderSetup() {
            var strategy = new SB();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = strategy.FindBestStrategies(5);
            stopwatch.Stop();
            
            Console.WriteLine($"elapsed time: {stopwatch.Elapsed.ToString(@"mm\:ss")}");
            var sb = new StringBuilder();
            var sWriter = new StringWriter(sb);

            sWriter.WriteLine($"Num generations: {new StrategyBuilderSettings().MaxNumberOfGenerations}");
            sWriter.WriteLine("Top strategies found");
            for(int i = 0; i<result.Count; i++){
                sWriter.WriteLine($" ------------- {i} -------------");
                result[i].WriteTreeStringTo(sWriter);
            }

            sWriter.Flush();
            sWriter.Close();

            Console.WriteLine(sb.ToString());
            Assert.NotNull(strategy);
        }
    }
}
