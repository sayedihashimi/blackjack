using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SayedHa.Blackjack.Tests.StrategyBuilder {
    public class BlackjackStrategyTreeTests {
        [Fact]
        public void Test_AddSplit_OnePair() {
            var tree = new BlackjackStrategyTree();

            CardNumber dealerCard = CardNumber.Ace;
            CardNumber pairCard = CardNumber.Nine;
            tree.AddNextHandAction(dealerCard, HandAction.Split, pairCard, pairCard);

            var foundHandAction = tree.GetNextHandAction(dealerCard, pairCard, pairCard);
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

            var tree = new BlackjackStrategyTree();
            for (int i = 0; i < dealerCardList.Count; i++) {
                tree.AddNextHandAction(dealerCardList[i], pairCardList[i], pairCardList[i], HandAction.Split);
            }
            // get them back and verify the result
            for (int i = 0; i < dealerCardList.Count; i++) {
                var foundHandAction = tree.GetNextHandAction(dealerCardList[i], pairCardList[i], pairCardList[i]);
                Assert.Equal(HandAction.Split, foundHandAction);
            }
        }
        [Fact]
        public void Test_Tree_AddSoftTotal_OneItem() {
            var tree = new BlackjackStrategyTree();
            CardNumber dealerCard = CardNumber.Two;
            CardNumber card1 = CardNumber.Three;
            HandAction expectedHandAction = HandAction.Hit;
            tree.AddSoftTotalNextAction(dealerCard, 3, expectedHandAction);

            var foundHandAction = tree.GetNextHandAction(dealerCard, card1, CardNumber.Ace);
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
                CardNumber.Queen,
                CardNumber.Eight,
                CardNumber.Three,
            };
            var opCardScore = new List<int> { 
                10,
                8,
                3
            };
            var expectedHandActionList = new List<HandAction> {
                HandAction.Hit,
                HandAction.Stand,
                HandAction.Stand,
            };

            var tree = new BlackjackStrategyTree();
            for (int i = 0;i<dealerCardList.Count;i++) {
                // int softTotal = CardNumberHelper.GetScoreTotal(opCards[i], CardNumber.Ace);
                tree.AddSoftTotalNextAction(dealerCardList[i], opCardScore[i], expectedHandActionList[i]);
            }
            // get them back and verify the result
            for(int i = 0; i < dealerCardList.Count; i++) {
                var result = tree.GetNextHandAction(dealerCardList[i], opCards[i], CardNumber.Ace );
                Assert.Equal(expectedHandActionList[i], result);
            }
        }
        [Fact]
        public void Test_Tree_AddHardTotal_OneItem() {
            var tree = new BlackjackStrategyTree();
            CardNumber dealerCard = CardNumber.Two;
            CardNumber card1 = CardNumber.Three;
            CardNumber card2 = CardNumber.Five;
            HandAction expectedHandAction = HandAction.Hit;
            tree.AddHardTotalNextAction(dealerCard, CardNumberHelper.GetScoreTotal(card1, card2), expectedHandAction);
            var foundHandAction = tree.GetNextHandAction(dealerCard, card1, CardNumber.Ace);
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
            var tree = new BlackjackStrategyTree();
            for(int i = 0;i<dealerCardList.Count;i++) {
                tree.AddHardTotalNextAction(
                    dealerCardList[i],
                    CardNumberHelper.GetScoreTotal(opCards[i].card1, opCards[i].card2),
                    expectedHandActionList[i]);
            }

            // get and verify
            for (int i = 0; i < dealerCardList.Count; i++) {
                var result = tree.GetNextHandAction(
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
            var tree = new BlackjackStrategyTree();
            for (int i = 0; i < dealerCardList.Count; i++) {
                tree.AddNextHandAction(
                    dealerCardList[i],
                    expectedHandActionList[i],
                    opCards[i].card1, opCards[i].card2);
            }

            // get and verify
            for (int i = 0; i < dealerCardList.Count; i++) {
                var result = tree.GetNextHandAction(
                    dealerCardList[i],
                    opCards[i].card1,
                    opCards[i].card2);
                Assert.Equal(expectedHandActionList[i], result);
            }
        }
        [Fact]
        public void Test_FullTree_100000_queries() {
            var allCardNumbers = ((CardNumber[])Enum.GetValues(typeof(CardNumber))).ToArray();
            var tree = new BlackjackStrategyTree();
            var expectedHandAction = HandAction.Stand;

            for (int i = 0; i < allCardNumbers.Length; i++) {
                var dealerCard = allCardNumbers[i];
                for (int j = 0; j < allCardNumbers.Length; j++) {
                    var op1Card = allCardNumbers[j]!;
                    for (int k = 0; k < allCardNumbers.Length; k++) {
                        var op2Card = allCardNumbers[k]!;
                        tree.AddNextHandAction(dealerCard, expectedHandAction, op1Card, op2Card);
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
                            var foundHandAction = tree.GetNextHandAction(dealerCard, op1Card!, op2Card!);
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
            Assert.Equal(expectedHandAction, tree.GetNextHandAction(CardNumber.Ten, CardNumber.Nine, CardNumber.Four));
            Assert.Equal(expectedHandAction, tree.GetNextHandAction(CardNumber.Ace, CardNumber.Nine, CardNumber.Nine));
        }
        [Fact]
        public void Test_WriteTreeStringTo_01() {
            var allCardNumbers = ((CardNumber[])Enum.GetValues(typeof(CardNumber))).ToArray();
            var tree = BlackjackStrategyTreeFactory.GetInstance(true).CreateNewRandomTree();
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            tree.WriteTreeStringTo(writer);
            writer.Flush();
            writer.Close();

            var str = sb.ToString();
            Assert.NotNull(str);
        }
        [Fact]
        public void Test_BasicStrategy_DoubleDown() {
            var bsTree = BlackjackStrategyTreeFactory.GetInstance(true).GetBasicStrategyTree();
            var allCardNumbers = CardDeckFactory.GetAllCardNumbers();
            // always double down on 11, always
            foreach(var dealerCard in allCardNumbers) {
                Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Two, CardNumber.Nine));
                Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Three, CardNumber.Eight));
                Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Four, CardNumber.Seven));
                Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Five, CardNumber.Six));
                Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Six, CardNumber.Five));
                Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Seven, CardNumber.Four));
                Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Eight, CardNumber.Three));
                Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Nine, CardNumber.Two));
            }
            // 10 will double on everything but 10 and Ace
            // always double down on 11, always
            foreach (var dealerCard in allCardNumbers) {
                if(dealerCard == CardNumber.Ten || 
                    dealerCard == CardNumber.Jack ||
                    dealerCard == CardNumber.Queen ||
                    dealerCard == CardNumber.King ||
                    dealerCard == CardNumber.Ace) {
                    continue;
                }
                Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Two, CardNumber.Eight));
                Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Three, CardNumber.Seven));
                Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Four, CardNumber.Six));
                Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Five, CardNumber.Five));
                Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Six, CardNumber.Four));
                Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Seven, CardNumber.Three));
                Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Eight, CardNumber.Two));
            }
            // 9 will double on 3 - 6
            foreach(var dealerCard in allCardNumbers) {
                if(dealerCard == CardNumber.Three ||
                    dealerCard == CardNumber.Four ||
                    dealerCard == CardNumber.Five ||
                    dealerCard == CardNumber.Six) {

                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Two, CardNumber.Seven));
                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Three, CardNumber.Six));
                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Four, CardNumber.Five));
                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Five, CardNumber.Four));
                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Six, CardNumber.Three));
                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Seven, CardNumber.Two));
                }
            }
            // now check hands with an Ace (soft totals)
            // Ace + 6
            Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(CardNumber.Three, CardNumber.Ace, CardNumber.Six));
            Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(CardNumber.Four, CardNumber.Ace, CardNumber.Six));
            Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(CardNumber.Five, CardNumber.Ace, CardNumber.Six));
            Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(CardNumber.Six, CardNumber.Ace, CardNumber.Six));
            // Ace + 5
            Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(CardNumber.Four, CardNumber.Ace, CardNumber.Five));
            Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(CardNumber.Five, CardNumber.Ace, CardNumber.Five));
            Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(CardNumber.Six, CardNumber.Ace, CardNumber.Five));
            // Ace + 4
            Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(CardNumber.Four, CardNumber.Ace, CardNumber.Four));
            Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(CardNumber.Five, CardNumber.Ace, CardNumber.Four));
            Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(CardNumber.Six, CardNumber.Ace, CardNumber.Four));
            // Ace + 3
            Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(CardNumber.Five, CardNumber.Ace, CardNumber.Three));
            Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(CardNumber.Six, CardNumber.Ace, CardNumber.Three));
            // Ace + 2
            Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(CardNumber.Five, CardNumber.Ace, CardNumber.Two));
            Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(CardNumber.Six, CardNumber.Ace, CardNumber.Two));
        }
        [Fact]
        public void Test_BasicStrategy_HardTotals() {
            var bsTree = BlackjackStrategyTreeFactory.GetInstance(true).GetBasicStrategyTree();
            var allCardNumbers = CardDeckFactory.GetAllCardNumbers();
            // stand on all cards 17 and above
            foreach(var dealerCard in allCardNumbers) {
                // 17
                Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Seven));
                Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Five, CardNumber.Two));
                Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Four, CardNumber.Three));
                // 18
                Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Eight));
                Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Three, CardNumber.Two, CardNumber.Three));
                // 19
                Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Nine));
                Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Four, CardNumber.Two, CardNumber.Three));
                // 20
                Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Ten));
                Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Four, CardNumber.Two, CardNumber.Two));
                // 21
                Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Ace));
                Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Four, CardNumber.Four, CardNumber.Three));
            }
            // 16
            foreach(var dealerCard in allCardNumbers) {
                if( dealerCard == CardNumber.Two ||
                    dealerCard == CardNumber.Three ||
                    dealerCard == CardNumber.Four ||
                    dealerCard == CardNumber.Five ||
                    dealerCard == CardNumber.Six) {

                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Six, CardNumber.Ten));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Seven, CardNumber.Nine));
                    // skip 8 because that's a pair
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Nine, CardNumber.Seven));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Six));
                }
                else {
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Six, CardNumber.Ten));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Seven, CardNumber.Nine));
                    // skip 8 because that's a pair
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Nine, CardNumber.Seven));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Six));
                }
            }
            // 15
            foreach(var dealerCard in allCardNumbers) {
                if (dealerCard == CardNumber.Two ||
                    dealerCard == CardNumber.Three ||
                    dealerCard == CardNumber.Four ||
                    dealerCard == CardNumber.Five ||
                    dealerCard == CardNumber.Six) {

                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Five, CardNumber.Ten));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Six, CardNumber.Nine));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Seven, CardNumber.Eight));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Eight, CardNumber.Seven));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Nine, CardNumber.Six));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Five));
                }
                else {
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Five, CardNumber.Ten));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Six, CardNumber.Nine));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Seven, CardNumber.Eight));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Eight, CardNumber.Seven));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Nine, CardNumber.Six));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Five));
                }
            }
            // 14
            foreach (var dealerCard in allCardNumbers) {
                if (dealerCard == CardNumber.Two ||
                    dealerCard == CardNumber.Three ||
                    dealerCard == CardNumber.Four ||
                    dealerCard == CardNumber.Five ||
                    dealerCard == CardNumber.Six) {

                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Four, CardNumber.Ten));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Five, CardNumber.Nine));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Six, CardNumber.Eight));
                    // skip 7, pair
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Eight, CardNumber.Six));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Nine, CardNumber.Five));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Four));
                }
                else {
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Four, CardNumber.Ten));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Five, CardNumber.Nine));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Six, CardNumber.Eight));
                    // skip 7, pair
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Eight, CardNumber.Six));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Nine, CardNumber.Five));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Four));
                }
            }
            // 13
            foreach (var dealerCard in allCardNumbers) {
                if (dealerCard == CardNumber.Two ||
                    dealerCard == CardNumber.Three ||
                    dealerCard == CardNumber.Four ||
                    dealerCard == CardNumber.Five ||
                    dealerCard == CardNumber.Six) {

                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Three, CardNumber.Ten));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Four, CardNumber.Nine));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Five, CardNumber.Eight));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Six, CardNumber.Seven));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Seven, CardNumber.Six));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Eight, CardNumber.Five));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Nine, CardNumber.Four));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Three));
                }
                else {
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Three, CardNumber.Ten));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Four, CardNumber.Nine));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Five, CardNumber.Eight));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Six, CardNumber.Seven));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Seven, CardNumber.Six));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Eight, CardNumber.Five));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Nine, CardNumber.Four));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Three));
                }
            }
            // 12
            foreach(var dealerCard in allCardNumbers) {
                if(dealerCard == CardNumber.Four ||
                    dealerCard == CardNumber.Five ||
                    dealerCard == CardNumber.Six) {

                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Two, CardNumber.Ten));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Three, CardNumber.Nine));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Four, CardNumber.Eight));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Five, CardNumber.Seven));
                    // skip 6, pair
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Seven, CardNumber.Five));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Eight, CardNumber.Four));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Nine, CardNumber.Three));
                    Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Two));
                }
                else {
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Two, CardNumber.Ten));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Three, CardNumber.Nine));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Four, CardNumber.Eight));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Five, CardNumber.Seven));
                    // skip 6, pair
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Seven, CardNumber.Five));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Eight, CardNumber.Four));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Nine, CardNumber.Three));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Ten, CardNumber.Two));
                }
            }
            // 11 is covered by double down test
            // 10
            foreach(var dealerCard in allCardNumbers) {
                if( dealerCard == CardNumber.Ten || 
                    dealerCard == CardNumber.Jack ||
                    dealerCard == CardNumber.Queen ||
                    dealerCard == CardNumber.King ||
                    dealerCard == CardNumber.Ace) {
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Eight, CardNumber.Two));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Seven, CardNumber.Three));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Six, CardNumber.Four));
                    // skip 5, pair
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Four, CardNumber.Six));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Three, CardNumber.Seven));
                    Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Two, CardNumber.Eight));
                }
                else {
                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Eight, CardNumber.Two));
                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Seven, CardNumber.Three));
                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Six, CardNumber.Four));
                    // skip 5, pair
                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Four, CardNumber.Six));
                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Three, CardNumber.Seven));
                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Two, CardNumber.Eight));
                }
            }
            // 9
            foreach(var dealerCard in allCardNumbers) {
                if(dealerCard == CardNumber.Three ||
                    dealerCard == CardNumber.Four ||
                    dealerCard == CardNumber.Five ||
                    dealerCard == CardNumber.Six) {

                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Seven, CardNumber.Two));
                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Six, CardNumber.Three));
                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Five, CardNumber.Four));
                    // skip 4, pair
                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Three, CardNumber.Six));
                    Assert.Equal(HandAction.Double, bsTree.GetNextHandAction(dealerCard, CardNumber.Two, CardNumber.Seven));
                }
            }
            // 8 or lower is a hit
            foreach(var dealerCard in allCardNumbers) {
                Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Six, CardNumber.Two));
                Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Five, CardNumber.Three));
                // skip 4, pair
                Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Three, CardNumber.Five));
                Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Two, CardNumber.Six));

                // 7
                Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Five, CardNumber.Two));
                Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Four, CardNumber.Three));
                Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Three, CardNumber.Four));
                Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Two, CardNumber.Five));
                
                // 6
                Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Four, CardNumber.Two));
                // skip 3, pair
                Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Two, CardNumber.Four));

                // 5
                Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Three, CardNumber.Two));
                Assert.Equal(HandAction.Hit, bsTree.GetNextHandAction(dealerCard, CardNumber.Two, CardNumber.Three));
            }

            Assert.Equal(HandAction.Stand, bsTree.GetNextHandAction(CardNumber.Queen, CardNumber.Ten, CardNumber.Four, CardNumber.Four));
        }
    }
}
