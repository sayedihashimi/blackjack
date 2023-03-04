using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Blackjack.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SayedHa.Blackjack.Tests.StrategyBuilder {
    public class ResultTreeTests {
        [Fact]
        public void Test_RecordOneResult() {
            var rt = new ResultTree();

            var hand = new Hand(5F, new NullLogger());
            hand.BetResult = 5F;

            var dealerCard = CardNumber.Four;
            var opCard1 = CardNumber.Eight;
            var opCard2 = CardNumber.Nine;
            rt.RecordResult(dealerCard, opCard1, opCard2, new List<Hand> { hand });

            var rootNode = rt.RootNode;
            var allResults = rt.GetAllResults();

            Assert.NotNull(rootNode);
            Assert.NotNull(allResults);
            Assert.Single(allResults);
            Assert.NotNull(allResults[0]);
            Assert.NotNull(allResults[0].Results);
            Assert.Equal(1, allResults[0].Results.NumberOfWins);
            Assert.Equal(5F, allResults[0].TotalGainOrLoss);
            Assert.Equal(dealerCard, allResults[0].DealerVisibleCard);
            Assert.True(allResults[0].OpponentCard1 == opCard1 || allResults[0].OpponentCard2 == opCard1);
            Assert.True(allResults[0].OpponentCard1 == opCard2 || allResults[0].OpponentCard2 == opCard2);
        }
        [Fact]
        public void Test_RecordThreeResults_NoDupes() {
            var rt = new ResultTree();

            var hands = new List<Hand> {
                new Hand(5F, new NullLogger(),5F),
                new Hand(10F, new NullLogger(),-10F),
                new Hand(32F, new NullLogger(),32F)
            };

            var dealerCards = new List<CardNumber> {
                CardNumber.Six,
                CardNumber.Ace,
                CardNumber.Four
            };

            var op1Cards = new List<CardNumber> {
                CardNumber.Four,
                CardNumber.King,
                CardNumber.Two
            };
            var op2Cards = new List<CardNumber> {
                CardNumber.Queen,
                CardNumber.Ten,
                CardNumber.Seven
            };

            for(int i = 0;i< hands.Count; i++) {
                rt.RecordResult(dealerCards[i], op1Cards[i], op2Cards[i], new List<Hand> { hands[i] });
            }

            var rootNode = rt.RootNode;
            var allResults = rt.GetAllResults();
            Assert.Equal(3, allResults.Count);
            Assert.Equal(3, rootNode.DealerVisibleCardNodes.Count);

            Assert.Contains(allResults, r => r.TotalGainOrLoss == 5F);
            Assert.Contains(allResults, r => r.TotalGainOrLoss == -10F);
            Assert.Contains(allResults, r => r.TotalGainOrLoss == 32F);
        }
        [Fact]
        public void Test_RecordThreeResults_Twice() {
            var rt = new ResultTree();

            var hands = new List<Hand> {
                new Hand(5F, new NullLogger(),5F),
                new Hand(10F, new NullLogger(),-10F),
                new Hand(32F, new NullLogger(),32F)
            };

            var dealerCards = new List<CardNumber> {
                CardNumber.Six,
                CardNumber.Ace,
                CardNumber.Four
            };

            var op1Cards = new List<CardNumber> {
                CardNumber.Four,
                CardNumber.King,
                CardNumber.Two
            };
            var op2Cards = new List<CardNumber> {
                CardNumber.Queen,
                CardNumber.Ten,
                CardNumber.Seven
            };

            var numTimesToRepeat = 2;

            for(var j = 0; j < numTimesToRepeat; j++) {
                for (int i = 0; i < hands.Count; i++) {
                    rt.RecordResult(dealerCards[i], op1Cards[i], op2Cards[i], new List<Hand> { hands[i] });
                }
            }

            var rootNode = rt.RootNode;
            var allResults = rt.GetAllResults();
            Assert.Equal(3, allResults.Count);
            Assert.Equal(3, rootNode.DealerVisibleCardNodes.Count);

            Assert.Equal(3, rt.NumSecondCardNodesCreated);
            Assert.Contains(allResults, r => r.TotalGainOrLoss == 5F*numTimesToRepeat);
            Assert.Contains(allResults, r => r.TotalGainOrLoss == -10F*numTimesToRepeat);
            Assert.Contains(allResults, r => r.TotalGainOrLoss == 32F*numTimesToRepeat);
        }
    }
}
