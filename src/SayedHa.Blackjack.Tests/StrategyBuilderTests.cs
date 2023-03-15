using System;
using System.Collections.Generic;
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

            var result = strategy.FindBestStrategies();

            Assert.NotNull(strategy);
        }
    }
}
