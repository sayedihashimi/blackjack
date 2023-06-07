using SayedHa.Blackjack.Shared.Blackjack.Strategy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SayedHa.Blackjack.Tests.StrategyBuilder {
    public class StrategyBuilder2Tests {
        [Fact]
        public void Test_ProduceOffspring() {
            var factory = NextHandActionArrayFactory.Instance;
            var parent1 = factory.CreateStrategyWithAllStands(false);
            var parent2 = factory.CreateStrategyWithAllHits(true);

            var sb = new StrategyBuilder2();
            (var child1, var child2) = sb.ProduceOffspring(parent1, parent2);
            
            Assert.NotNull(child1);
            Assert.NotNull(child2);

            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);
            stringWriter.WriteLine("---- child 2 ----");
            child1.WriteTo(stringWriter);
            stringWriter.WriteLine("---- child 2 ----");
            child2.WriteTo(stringWriter);

            stringWriter.Flush();
            Console.WriteLine(stringBuilder.ToString());
        }
    }
}
