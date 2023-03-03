using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Blackjack.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SayedHa.Blackjack.Tests.StrategyBuilder {
    public class StrategyBuilderRootNodeTests {

        [Fact]
        public void Test_StrategyBuilderRootNode_GetAddIfNeeded_OneCardNumber() {
            var rootNode = new Shared.Blackjack.Strategy.StrategyBuilderRootNode();

            var cardToAdd = CardNumber.Nine;

            var foundNode = rootNode.GetAddIfNeeded(cardToAdd);

            // verify that the node was found and that the internal list contains only it
            Assert.NotNull(foundNode);
            Assert.NotNull(rootNode.DealerVisibleCardNodes);
            Assert.Single(rootNode.DealerVisibleCardNodes);
            Assert.Single(rootNode.DealerVisibleCardNodes, x => x.DealerVisibleCard == cardToAdd);
        }
        [Fact]
        public void Test_StrategyBuilderRootNode_GetAddIfNeeded_ThreeCardNumbers() {
            var rootNode = new Shared.Blackjack.Strategy.StrategyBuilderRootNode();

            var cardsToAdd = new List<CardNumber> {
                CardNumber.Two,
                CardNumber.Ace,
                CardNumber.Nine
            };

            foreach (var card in cardsToAdd) {
                rootNode.GetAddIfNeeded(card);
            }

            Assert.Equal(3, rootNode.DealerVisibleCardNodes.Count);
            // now verify these are all on the tree
            foreach (var card in cardsToAdd) {
                var foundNode = rootNode.GetAddIfNeeded(card);
                // verify that the node was found and that the internal list contains only it
                Assert.NotNull(foundNode);
                Assert.NotNull(rootNode.DealerVisibleCardNodes);
                Assert.Single(rootNode.DealerVisibleCardNodes, x => x.DealerVisibleCard == card);
            }
        }
        [Fact]
        public void Test_StrategyBuilderRootNode_GetAddIfNeeded_AddACardMultipleTimes_OnlyOneNodeForItExists() {
            var rootNode = new Shared.Blackjack.Strategy.StrategyBuilderRootNode();

            var cardToAdd = CardNumber.Nine;
            var numTimesToAddCard = 10;
            for (var i = 0; i < numTimesToAddCard; i++) {
                rootNode.GetAddIfNeeded(cardToAdd);
            }

            // verify that the node is the only item on the list
            var foundNode = rootNode.GetAddIfNeeded(cardToAdd);

            // verify that the node was found and that the internal list contains only it
            Assert.NotNull(foundNode);
            Assert.NotNull(rootNode.DealerVisibleCardNodes);
            Assert.Single(rootNode.DealerVisibleCardNodes);
            Assert.Single(rootNode.DealerVisibleCardNodes, x => x.DealerVisibleCard == cardToAdd);
        }
        // todo: test where the same cardnumber is added multiple times, but only one node exists in the tree for it
        [Fact]
        public void Test_StrategyBuilderRootNode_GetAddIfNeeded_AddMultipleCardsMultipleTimes_OnlyOneNodeForEachItExists() {
            var rootNode = new Shared.Blackjack.Strategy.StrategyBuilderRootNode();

            var cardsToAdd = new List<CardNumber> {
                CardNumber.Two,
                CardNumber.Ace,
                CardNumber.Nine
            };

            var numTimesToAddEachCard = 10;
            for (var i = 0; i < numTimesToAddEachCard; i++) {
                foreach (var card in cardsToAdd) {
                    rootNode.GetAddIfNeeded(card);
                }
            }

            Assert.Equal(3, rootNode.DealerVisibleCardNodes.Count);
            // now verify these are all on the tree
            foreach (var card in cardsToAdd) {
                var foundNode = rootNode.GetAddIfNeeded(card);
                // verify that the node was found and that the internal list contains only it
                Assert.NotNull(foundNode);
                Assert.NotNull(rootNode.DealerVisibleCardNodes);
                Assert.Single(rootNode.DealerVisibleCardNodes, x => x.DealerVisibleCard == card);
            }
        }
    }
}
