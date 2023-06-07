using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree;
using SayedHa.Blackjack.Shared.Roulette;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SayedHa.Blackjack.Tests.StrategyBuilder {
    public class NextHandActionTreeTests {
        [Fact]
        public void Test_AddOneItem() {
            var naTree = new NextHandActionTree();

            var dealerCard = CardNumber.Ten;
            var opCard1 = CardNumber.Five;
            var opCard2 = CardNumber.Queen;
            var expectedHandAction = HandAction.Hit;

            naTree.AddNextHandActionFor(dealerCard, opCard1, opCard2, expectedHandAction);

            var resultHandAction1 = naTree.GetNextHandActionFor(dealerCard, opCard1, opCard2);
            var resultHandAction2 = naTree.GetNextHandActionFor(dealerCard, opCard2, opCard1);

            Assert.Equal(expectedHandAction, resultHandAction1);
            Assert.Equal(resultHandAction1, resultHandAction2);
            Assert.Equal(1, naTree.NumSecondCardNodesCreated);
        }
        [Fact]
        public void Test_AddTwoItems() {
            var dealerCards = new List<CardNumber> {
                CardNumber.Ace,
                CardNumber.Four
            };
            var op1Cards = new List<CardNumber> {
                CardNumber.Six,
                CardNumber.Eight
            };
            var op2Cards = new List<CardNumber> {
                CardNumber.Jack,
                CardNumber.King
            };
            var nextHandActions = new List<HandAction> {
                HandAction.Hit,
                HandAction.Stand
            };
            var naTree = new NextHandActionTree();
            for(int i = 0;i<dealerCards.Count;i++) {
                naTree.AddNextHandActionFor(dealerCards[i], op1Cards[i], op2Cards[i], nextHandActions[i]);
            }
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            
            for (int i = 0; i < dealerCards.Count; i++) {
                var expectedHandAction = nextHandActions[i];
                var foundHandAction = naTree.GetNextHandActionFor(dealerCards[i], op1Cards[i], op2Cards[i]);
                Assert.Equal(expectedHandAction, foundHandAction);
            }
        }
        [Fact]
        public void Test_FullTree_100000_queries() {
            var allCardNumbers = ((CardNumber[])Enum.GetValues(typeof(CardNumber))).ToArray();

            var naTree = new NextHandActionTree();
            var expectedHandAction = HandAction.Stand;
            for (int i = 0; i < allCardNumbers.Length; i++) {
                var dealerCard = allCardNumbers[i];
                for (int j = 0; j < allCardNumbers.Length; j++) {
                    var op1Card = allCardNumbers[j]!;
                    for (int k = 0; k < allCardNumbers.Length; k++) {
                        var op2Card = allCardNumbers[k]!;
                        naTree.AddNextHandActionFor(dealerCard, op1Card!, op2Card!, expectedHandAction);
                    }
                }
            }

            // go through them all with a stopwatch to see how long it takes to
            var numQueriesToRun = 100000;
            var numQueries = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (numQueries < numQueriesToRun) {
                for (int i = 0; i < allCardNumbers.Length; i++) {
                    var dealerCard = allCardNumbers[i];
                    for (int j = 0; j < allCardNumbers.Length; j++) {
                        var op1Card = allCardNumbers[j]!;
                        for (int k = 0; k < allCardNumbers.Length; k++) {
                            var op2Card = allCardNumbers[k]!;
                            var foundHandAction = naTree.GetNextHandActionFor(dealerCard, op1Card!, op2Card!);
                            numQueries++;
                            if(numQueries > numQueriesToRun) {
                                break;
                            }
                        }
                        if (numQueries > numQueriesToRun) {
                            break;
                        }
                    }
                    if (numQueries > numQueriesToRun) {
                        break;
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"Elapsed time: {stopwatch.Elapsed.TotalMilliseconds} ms");

            // even on a very slow machine it should run much faster
            Assert.Equal(1183, naTree.NumSecondCardNodesCreated);
            Assert.True( stopwatch.ElapsedMilliseconds < 1000 );
            Assert.Equal(expectedHandAction, naTree.GetNextHandActionFor(CardNumber.Ten, CardNumber.Nine, CardNumber.Four));
        }
    }
}
