using SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class BlackjackStrategyTreeFactoryTests {
        [Fact]
        public void Test_Create_NewRandomTree_01() {
            var randomTree = BlackjackStrategyTreeFactory.GetInstance(true).CreateNewRandomTree();

            Assert.NotNull(randomTree);
            Assert.NotNull(randomTree.aceTree);
            Assert.NotNull(randomTree.hardTotalTree);
        }
    }
}
