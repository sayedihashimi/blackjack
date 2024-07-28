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
        public void Test_AllHits() {
            var allHits = NextHandActionArrayFactory.Instance.CreateStrategyWithAllHits(true);

            var splitIntValue = NextHandActionConverter.Instance.ConvertBoolToInt(true);
            for(int i =0; i< allHits.pairHandActionArray.GetLength(0); i++) {
                for(int j = 0; j < allHits.pairHandActionArray.GetLength(1); j++) {
                    var action = allHits.pairHandActionArray[i, j];
                    Assert.Equal(splitIntValue, action);
                }
            }

            var hitIntValue = NextHandActionConverter.Instance.GetIntFor(HandAction.Hit);
            for (int i = 0; i < allHits.hardTotalHandActionArray.GetLength(0); i++) {
                for (int j = 0; j < allHits.hardTotalHandActionArray.GetLength(1); j++) {
                    var action = allHits.hardTotalHandActionArray[i, j];
                    Assert.Equal(hitIntValue, action);
                }
            }
            for (int i = 0; i < allHits.softHandActionArray.GetLength(0); i++) {
                for (int j = 0; j < allHits.softHandActionArray.GetLength(1); j++) {
                    var action = allHits.softHandActionArray[i, j];
                    Assert.Equal(hitIntValue, action);
                }
            }
        }
        //[Fact]
        //public void Test_AllStands() {
        //    var hitIntValue = NextHandActionConverter.Instance.GetIntFor(HandAction.Hit);
        //    var allHits = NextHandActionArrayFactory.GetInstance().CreateStrategyWithAllHits(true);
        //    for (int i = 0; i < allHits.pairHandActionArray.GetLength(0); i++) {
        //        for (int j = 0; j < allHits.pairHandActionArray.GetLength(1); j++) {
        //            var action = allHits.pairHandActionArray[i, j];
        //            Assert.Equal(hitIntValue, action);
        //        }
        //    }
        //}

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
                            // don't ask about Ace 10 it's blackjack and not in the array
                            if (IsBlackJack(op1Card, op2Card)) {
                                continue;
                            }

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

        [Fact]
        public void Test_Equals() {
			// build 3 nhaa objects with the same content
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

			var nhaa1 = new NextHandActionArray();
            var nhaa2 = new NextHandActionArray();
            var nhaa3 = new NextHandActionArray();
			for (int i = 0; i < dealerCardList.Count; i++) {
				nhaa1.SetSplitForPair(true, dealerCardList[i], pairCardList[i]);
				nhaa2.SetSplitForPair(true, dealerCardList[i], pairCardList[i]);
				nhaa3.SetSplitForPair(true, dealerCardList[i], pairCardList[i]);
			}

            Assert.Equal(nhaa1, nhaa2);
            Assert.Equal(nhaa2, nhaa3);
		}
        private bool IsBlackJack(CardNumber cardNumber1, CardNumber cardNumber2) =>
            (cardNumber1, cardNumber2) switch{
                (CardNumber.Ace,CardNumber.Ten) => true,
                (CardNumber.Ace,CardNumber.Jack) => true,
                (CardNumber.Ace, CardNumber.Queen) => true,
                (CardNumber.Ace, CardNumber.King) => true,
                (CardNumber.Ten, CardNumber.Ace) => true,
                (CardNumber.Jack, CardNumber.Ace) => true,
                (CardNumber.Queen, CardNumber.Ace) => true,
                (CardNumber.King, CardNumber.Ace) => true,
                _ => false
            };
    }
}
