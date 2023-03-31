using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Blackjack.Strategy;
using SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SayedHa.Blackjack.Tests.StrategyBuilder {
    public class NextHandActionArrayTests {
        [Fact]
        public void Test_AddSplit_OnePair() {
            var nhaa = new NextHandActionArray();

            CardNumber dealerCard = CardNumber.Ace;
            CardNumber pairCard = CardNumber.Nine;
            nhaa.SetSplitForPair(true, dealerCard, pairCard);

            var foundHandAction = nhaa.GetHandAction(dealerCard, pairCard, pairCard);
            Assert.Equal(HandAction.Split, foundHandAction);
        }
        [Fact]
        public void Test_AddSplit_ThreePairs() {
            var dealerCardList = new List<CardNumber> {
                CardNumber.Nine,
                CardNumber.Four,
                CardNumber.Ace,
            };
            var pairCardList = new List<CardNumber> {
                CardNumber.Six,
                CardNumber.Ten,
                CardNumber.Nine
            };

            var nhaa = new NextHandActionArray();
            for (int i = 0; i < dealerCardList.Count; i++) {
                nhaa.SetSplitForPair(true, dealerCardList[i], pairCardList[i]);
            }
            // get them back and verify the result
            for (int i = 0; i < dealerCardList.Count; i++) {
                var foundHandAction = nhaa.GetHandAction(dealerCardList[i], pairCardList[i], pairCardList[i]);
                Assert.Equal(HandAction.Split, foundHandAction);
            }
        }
        [Fact]
        public void Test_Tree_AddSoftTotal_OneItem() {
            var nhaa = new NextHandActionArray();
            CardNumber dealerCard = CardNumber.Two;
            CardNumber card1 = CardNumber.Three;
            HandAction expectedHandAction = HandAction.Hit;
            nhaa.SetHandActionForSoftTotal(expectedHandAction, dealerCard, 3);

            var foundHandAction = nhaa.GetHandAction(dealerCard, card1, CardNumber.Ace);
            Assert.Equal(expectedHandAction, foundHandAction);
        }
        [Fact]
        public void Test_Tree_AddSoftTotal_ThreeItems() {
            var dealerCardList = new List<CardNumber> {
                CardNumber.Nine,
                CardNumber.Four,
                CardNumber.Ace,
            };
            var opCards = new List<CardNumber> {
                CardNumber.Nine,
                CardNumber.Eight,
                CardNumber.Three,
            };
            var opCardScore = new List<int> {
                9,
                8,
                3
            };
            var expectedHandActionList = new List<HandAction> {
                HandAction.Hit,
                HandAction.Stand,
                HandAction.Stand,
            };

            var nhaa = new NextHandActionArray();
            for (int i = 0; i < dealerCardList.Count; i++) {
                nhaa.SetHandActionForSoftTotal(expectedHandActionList[i], dealerCardList[i], opCardScore[i]);
            }
            // get them back and verify the result
            for (int i = 0; i < dealerCardList.Count; i++) {
                var result = nhaa.GetHandAction(dealerCardList[i], opCards[i], CardNumber.Ace);
                Assert.Equal(expectedHandActionList[i], result);
            }
        }
        [Fact]
        public void Test_Tree_AddHardTotal_OneItem() {
            var nhaa = new NextHandActionArray();
            CardNumber dealerCard = CardNumber.Two;
            CardNumber card1 = CardNumber.Three;
            CardNumber card2 = CardNumber.Five;
            HandAction expectedHandAction = HandAction.Hit;
            nhaa.SetHandActionForHardTotal(expectedHandAction, dealerCard, CardNumberHelper.GetScoreTotal(card1, card2));
            var foundHandAction = nhaa.GetHandAction(dealerCard, card1, card2);
            Assert.Equal(expectedHandAction, foundHandAction);
        }
        [Fact]
        public void Test_Tree_AddHardTotalThreeItems() {
            var dealerCardList = new List<CardNumber> {
                CardNumber.Six,
                CardNumber.King,
                CardNumber.Ten,
            };
            var opCards = new List<(CardNumber card1, CardNumber card2)> {
                (CardNumber.Three, CardNumber.Eight),
                (CardNumber.Seven, CardNumber.Jack),
                (CardNumber.Four, CardNumber.Nine)
            };
            var expectedHandActionList = new List<HandAction> {
                HandAction.Double,
                HandAction.Stand,
                HandAction.Hit,
            };

            var nhaa = new NextHandActionArray();
            for (int i = 0; i < dealerCardList.Count; i++) {
                nhaa.SetHandActionForHardTotal(
                    expectedHandActionList[i],
                    dealerCardList[i],
                    CardNumberHelper.GetScoreTotal(opCards[i].card1, opCards[i].card2));
            }

            // get and verify
            for (int i = 0; i < dealerCardList.Count; i++) {
                var result = nhaa.GetHandAction(
                    dealerCardList[i],
                    opCards[i].card1,
                    opCards[i].card2);
                Assert.Equal(expectedHandActionList[i], result);
            }
        }
        [Fact]
        public void Test_GetNextHandAction_01() {
            var dealerCardList = new List<CardNumber> {
                CardNumber.Six,
                CardNumber.King,
                CardNumber.Ten,

                CardNumber.Ace,
                CardNumber.Jack,

                CardNumber.Eight,
                CardNumber.Six,
            };
            var opCards = new List<(CardNumber card1, CardNumber card2)> {
                (CardNumber.Three, CardNumber.Eight),
                (CardNumber.Seven, CardNumber.Jack),
                (CardNumber.Four, CardNumber.Nine),
                // add some hands with an ace
                (CardNumber.Ace, CardNumber.Six),
                (CardNumber.Five, CardNumber.Ace),
                // add pairs
                (CardNumber.Nine, CardNumber.Nine),
                (CardNumber.Six, CardNumber.Six),
            };
            var expectedHandActionList = new List<HandAction> {
                HandAction.Double,
                HandAction.Stand,
                HandAction.Hit,

                HandAction.Hit,
                HandAction.Hit,

                HandAction.Split,
                HandAction.Split
            };

            var nhaa = new NextHandActionArray();
            for (int i = 0; i < dealerCardList.Count; i++) {
                nhaa.SetHandActionForHardTotal(
                    expectedHandActionList[i],
                    dealerCardList[i],
                    CardNumberHelper.GetScoreTotal(opCards[i].card1, opCards[i].card2));
            }

            // get and verify
            for (int i = 0; i < dealerCardList.Count; i++) {
                var result = nhaa.GetHandAction(
                    dealerCardList[i],
                    opCards[i].card1,
                    opCards[i].card2);
                Assert.Equal(expectedHandActionList[i], result);
            }
        }
        [Fact]
        public void Test_FullTree_100000_queries() {
            var allCardNumbers = ((CardNumber[])Enum.GetValues(typeof(CardNumber))).ToArray();
            var nhaa = new NextHandActionArray();
            var expectedHandAction = HandAction.Stand;

            for (int i = 0; i < allCardNumbers.Length; i++) {
                var dealerCard = allCardNumbers[i];
                for (int j = 0; j < allCardNumbers.Length; j++) {
                    var op1Card = allCardNumbers[j]!;
                    for (int k = 0; k < allCardNumbers.Length; k++) {
                        var op2Card = allCardNumbers[k]!;

                        int scoreTotal = CardNumberHelper.GetScoreTotal(op1Card, op2Card);
                        // don't set the core for 21, it's blackjack
                        if (scoreTotal < 21) {
                            nhaa.SetHandAction(expectedHandAction, dealerCard, op1Card, op2Card);
                        }
                    }
                }
            }

            // go through them all with a stopwatch to see how long it takes
            var numQueriesToRun = 1000000;
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
                            var foundHandAction = nhaa.GetHandAction(dealerCard, op1Card!, op2Card!);
                            numQueries++;
                            if (numQueries > numQueriesToRun) {
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

            Assert.True(stopwatch.ElapsedMilliseconds < 1000);
            Assert.Equal(expectedHandAction, nhaa.GetHandAction(CardNumber.Ten, CardNumber.Nine, CardNumber.Four));
            Assert.Equal(expectedHandAction, nhaa.GetHandAction(CardNumber.Ace, CardNumber.Nine, CardNumber.Nine));
        }
    }
}
