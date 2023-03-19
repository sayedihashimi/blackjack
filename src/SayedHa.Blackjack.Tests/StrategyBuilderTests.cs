using System;
using System.Collections.Generic;
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

            var result = strategy.FindBestStrategies(5);

            var sb = new StringBuilder();
            var sWriter = new StringWriter(sb);

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
