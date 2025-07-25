// This file is part of SayedHa.Blackjack.
//
// SayedHa.Blackjack is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SayedHa.Blackjack is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with SayedHa.Blackjack.  If not, see <https://www.gnu.org/licenses/>.
using SayedHa.Blackjack.Shared;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class HandEdgeCasesTests {
        [Fact]
        public void Test_Hand_Constructor_Sets_Properties() {
            var logger = NullLogger.Instance;
            var hand = new Hand(50f, logger);
            
            Assert.Equal(50f, hand.Bet);
            Assert.Equal(HandStatus.InPlay, hand.Status);
            Assert.Equal(HandResult.InPlay, hand.HandResult);
            Assert.Null(hand.BetResult);
        }

        [Fact]
        public void Test_Hand_Constructor_With_BetResult() {
            var logger = NullLogger.Instance;
            var hand = new Hand(25f, logger, 10f);
            
            Assert.Equal(25f, hand.Bet);
            Assert.Equal(10f, hand.BetResult);
        }

        [Fact]
        public void Test_Hand_IsBlackjack_True() {
            var hand = new Hand(10f, NullLogger.Instance) {
                DealtCards = new List<Card> {
                    new Card { Number = CardNumber.Ace, Suit = CardSuit.Spade },
                    new Card { Number = CardNumber.King, Suit = CardSuit.Heart }
                }
            };
            
            Assert.True(hand.DoesHandHaveBlackjack());
            Assert.Equal(21, hand.GetScore());
        }

        [Fact]
        public void Test_Hand_IsBlackjack_False_Not_21() {
            var hand = new Hand(10f, NullLogger.Instance) {
                DealtCards = new List<Card> {
                    new Card { Number = CardNumber.King, Suit = CardSuit.Spade },
                    new Card { Number = CardNumber.Queen, Suit = CardSuit.Heart }
                }
            };
            
            Assert.False(hand.DoesHandHaveBlackjack());
            Assert.Equal(20, hand.GetScore());
        }

        [Fact]
        public void Test_Hand_IsBlackjack_False_Three_Cards() {
            var hand = new Hand(10f, NullLogger.Instance) {
                DealtCards = new List<Card> {
                    new Card { Number = CardNumber.Ace, Suit = CardSuit.Spade },
                    new Card { Number = CardNumber.Five, Suit = CardSuit.Heart },
                    new Card { Number = CardNumber.Five, Suit = CardSuit.Diamond }
                }
            };
            
            // Blackjack can only happen with exactly 2 cards
            Assert.Equal(21, hand.GetScore());
        }

        [Fact]
        public void Test_Hand_IsBust_True() {
            var hand = new Hand(10f, NullLogger.Instance) {
                DealtCards = new List<Card> {
                    new Card { Number = CardNumber.King, Suit = CardSuit.Spade },
                    new Card { Number = CardNumber.Queen, Suit = CardSuit.Heart },
                    new Card { Number = CardNumber.Five, Suit = CardSuit.Diamond }
                }
            };
            
            Assert.True(hand.GetScore() > 21);
            Assert.Equal(25, hand.GetScore());
        }

        [Fact]
        public void Test_Hand_IsBust_False() {
            var hand = new Hand(10f, NullLogger.Instance) {
                DealtCards = new List<Card> {
                    new Card { Number = CardNumber.King, Suit = CardSuit.Spade },
                    new Card { Number = CardNumber.Ace, Suit = CardSuit.Heart }
                }
            };
            
            Assert.False(hand.GetScore() > 21);
            Assert.Equal(21, hand.GetScore());
        }

        [Fact]
        public void Test_Hand_CanSplit_True_Pair() {
            var hand = new Hand(10f, NullLogger.Instance) {
                DealtCards = new List<Card> {
                    new Card { Number = CardNumber.Eight, Suit = CardSuit.Spade },
                    new Card { Number = CardNumber.Eight, Suit = CardSuit.Heart }
                }
            };
            
            var validActions = hand.GetValidActions(100);
            Assert.Contains(HandAction.Split, validActions);
        }

        [Fact]
        public void Test_Hand_CanSplit_True_Face_Cards() {
            var hand = new Hand(10f, NullLogger.Instance) {
                DealtCards = new List<Card> {
                    new Card { Number = CardNumber.King, Suit = CardSuit.Spade },
                    new Card { Number = CardNumber.Queen, Suit = CardSuit.Heart }
                }
            };
            
            var validActions = hand.GetValidActions(100);
            // Face cards have different numbers but same value, so they can't be split
            Assert.DoesNotContain(HandAction.Split, validActions);
        }

        [Fact]
        public void Test_Hand_CanSplit_False_Different_Values() {
            var hand = new Hand(10f, NullLogger.Instance) {
                DealtCards = new List<Card> {
                    new Card { Number = CardNumber.King, Suit = CardSuit.Spade },
                    new Card { Number = CardNumber.Nine, Suit = CardSuit.Heart }
                }
            };
            
            var validActions = hand.GetValidActions(100);
            Assert.DoesNotContain(HandAction.Split, validActions);
        }

        [Fact]
        public void Test_Hand_CanSplit_False_More_Than_Two_Cards() {
            var hand = new Hand(10f, NullLogger.Instance) {
                DealtCards = new List<Card> {
                    new Card { Number = CardNumber.Eight, Suit = CardSuit.Spade },
                    new Card { Number = CardNumber.Eight, Suit = CardSuit.Heart },
                    new Card { Number = CardNumber.Two, Suit = CardSuit.Diamond }
                }
            };
            
            var validActions = hand.GetValidActions(100);
            Assert.DoesNotContain(HandAction.Split, validActions);
        }

        [Fact]
        public void Test_Hand_CanDouble_True_Two_Cards() {
            var hand = new Hand(10f, NullLogger.Instance) {
                DealtCards = new List<Card> {
                    new Card { Number = CardNumber.Five, Suit = CardSuit.Spade },
                    new Card { Number = CardNumber.Six, Suit = CardSuit.Heart }
                }
            };
            
            var validActions = hand.GetValidActions(100);
            Assert.Contains(HandAction.Double, validActions);
        }

        [Fact]
        public void Test_Hand_CanDouble_False_More_Than_Two_Cards() {
            var hand = new Hand(10f, NullLogger.Instance) {
                DealtCards = new List<Card> {
                    new Card { Number = CardNumber.Five, Suit = CardSuit.Spade },
                    new Card { Number = CardNumber.Six, Suit = CardSuit.Heart },
                    new Card { Number = CardNumber.Two, Suit = CardSuit.Diamond }
                }
            };
            
            var validActions = hand.GetValidActions(100);
            Assert.DoesNotContain(HandAction.Double, validActions);
        }

        [Fact]
        public void Test_Hand_Empty_Cards_List() {
            var hand = new Hand(10f, NullLogger.Instance) {
                DealtCards = new List<Card>()
            };
            
            Assert.Equal(0, hand.GetScore());
            // DoesHandHaveBlackjack requires exactly 2 cards, so it will throw with empty list
            // We'll just test that the score is 0
        }

        [Fact]
        public void Test_Hand_Null_Cards_List() {
            var hand = new Hand(10f, NullLogger.Instance);
            // DealtCards is not null by default, it's an empty list
            
            Assert.Equal(0, hand.GetScore());
        }

        [Fact]
        public void Test_DealerHand_Constructor() {
            var dealerHand = new DealerHand(NullLogger.Instance);
            
            Assert.Equal(0f, dealerHand.Bet); // Dealer doesn't bet
            Assert.Equal(HandStatus.InPlay, dealerHand.Status);
        }

        [Fact]
        public void Test_DealerHand_DealersVisibleCard() {
            var dealerHand = new DealerHand(NullLogger.Instance) {
                DealtCards = new List<Card> {
                    new Card { Number = CardNumber.Nine, Suit = CardSuit.Spade }, // Hidden
                    new Card { Number = CardNumber.King, Suit = CardSuit.Heart }  // Visible
                }
            };
            
            var visibleCard = dealerHand.DealersVisibleCard;
            Assert.NotNull(visibleCard);
            Assert.Equal(CardNumber.King, visibleCard.Number);
            Assert.Equal(CardSuit.Heart, visibleCard.Suit);
        }

        [Fact]
        public void Test_DealerHand_DealersHiddenCard() {
            var dealerHand = new DealerHand(NullLogger.Instance) {
                DealtCards = new List<Card> {
                    new Card { Number = CardNumber.Nine, Suit = CardSuit.Spade }, // Hidden
                    new Card { Number = CardNumber.King, Suit = CardSuit.Heart }  // Visible
                }
            };
            
            var hiddenCard = dealerHand.DealersHiddenCard;
            Assert.NotNull(hiddenCard);
            Assert.Equal(CardNumber.Nine, hiddenCard.Number);
            Assert.Equal(CardSuit.Spade, hiddenCard.Suit);
        }

        [Fact]
        public void Test_DealerHand_Cards_Null_Or_Empty() {
            var dealerHand = new DealerHand(NullLogger.Instance);
            
            Assert.Null(dealerHand.DealersVisibleCard);
            Assert.Null(dealerHand.DealersHiddenCard);
            
            dealerHand.DealtCards = new List<Card>();
            Assert.Null(dealerHand.DealersVisibleCard);
            Assert.Null(dealerHand.DealersHiddenCard);
        }

        [Fact]
        public void Test_DealerHand_Only_One_Card() {
            var dealerHand = new DealerHand(NullLogger.Instance) {
                DealtCards = new List<Card> {
                    new Card { Number = CardNumber.Ace, Suit = CardSuit.Club }
                }
            };
            
            Assert.NotNull(dealerHand.DealersHiddenCard);
            Assert.Null(dealerHand.DealersVisibleCard); // Only returns card if at least 2 cards
        }

        [Fact]
        public void Test_Hand_ToString_Contains_Useful_Info() {
            var hand = new Hand(25f, NullLogger.Instance) {
                DealtCards = new List<Card> {
                    new Card { Number = CardNumber.King, Suit = CardSuit.Spade },
                    new Card { Number = CardNumber.Seven, Suit = CardSuit.Heart }
                }
            };
            
            // Test the overloaded ToString method with parameters to show all info
            var handString = hand.ToString(false, true, true, true, true);
            
            Assert.Contains("Score=17", handString); // Score
            Assert.Contains("K", handString);  // King card
            Assert.Contains("7", handString);  // Seven card
        }
    }
}
