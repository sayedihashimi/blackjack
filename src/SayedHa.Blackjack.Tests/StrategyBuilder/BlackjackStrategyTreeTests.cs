using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SayedHa.Blackjack.Tests.StrategyBuilder {
    public class BlackjackStrategyTreeTests {
        [Fact]
        public void Test_Tree_AddAndGetOnePair() {
            var tree = new BlackjackStrategyTree();

            CardNumber dealerCard = CardNumber.Ace;
            CardNumber pairCard = CardNumber.Nine;
            HandAction expectedHandAction = HandAction.Stand;
            tree.AddPairNextAction(dealerCard, pairCard, expectedHandAction);

            var foundHandAction = tree.GetNextHandAction(dealerCard, pairCard, pairCard);
            Assert.Equal(expectedHandAction, foundHandAction);
        }
        [Fact]
        public void Test_Tree_AddAndGetThreePairs() {
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
            var expectedHandActionList = new List<HandAction> {
                HandAction.Hit,
                HandAction.Stand,
                HandAction.Stand
            };

            var tree = new BlackjackStrategyTree();
            for (int i = 0;i< dealerCardList.Count;i++) {
                tree.AddPairNextAction(dealerCardList[i], pairCardList[i], expectedHandActionList[i]);
            }
            // get them back and verify the result
            for (int i = 0; i < dealerCardList.Count; i++) {
                var foundHandAction = tree.GetNextHandAction(dealerCardList[i], pairCardList[i], pairCardList[i]);
                Assert.Equal(expectedHandActionList[i], foundHandAction);
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

        // TODO: Needs investigation
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
    }
}
