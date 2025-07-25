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
using System;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class CardExtensionsTests {
        [Theory]
        [InlineData(CardNumber.Ace, new int[] { 11, 1 })]
        [InlineData(CardNumber.Two, new int[] { 2 })]
        [InlineData(CardNumber.Three, new int[] { 3 })]
        [InlineData(CardNumber.Four, new int[] { 4 })]
        [InlineData(CardNumber.Five, new int[] { 5 })]
        [InlineData(CardNumber.Six, new int[] { 6 })]
        [InlineData(CardNumber.Seven, new int[] { 7 })]
        [InlineData(CardNumber.Eight, new int[] { 8 })]
        [InlineData(CardNumber.Nine, new int[] { 9 })]
        [InlineData(CardNumber.Ten, new int[] { 10 })]
        [InlineData(CardNumber.Jack, new int[] { 10 })]
        [InlineData(CardNumber.Queen, new int[] { 10 })]
        [InlineData(CardNumber.King, new int[] { 10 })]
        public void Test_GetValues_Returns_Correct_Values(CardNumber cardNumber, int[] expectedValues) {
            var actualValues = cardNumber.GetValues();
            
            Assert.Equal(expectedValues, actualValues);
        }

        [Theory]
        [InlineData(CardNumber.Ace, "A")]
        [InlineData(CardNumber.Two, "2")]
        [InlineData(CardNumber.Three, "3")]
        [InlineData(CardNumber.Four, "4")]
        [InlineData(CardNumber.Five, "5")]
        [InlineData(CardNumber.Six, "6")]
        [InlineData(CardNumber.Seven, "7")]
        [InlineData(CardNumber.Eight, "8")]
        [InlineData(CardNumber.Nine, "9")]
        [InlineData(CardNumber.Ten, "10")]
        [InlineData(CardNumber.Jack, "J")]
        [InlineData(CardNumber.Queen, "Q")]
        [InlineData(CardNumber.King, "K")]
        public void Test_GetFriendlyString_Returns_Correct_String(CardNumber cardNumber, string expected) {
            var actual = cardNumber.GetFriendlyString();
            
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(CardNumber.King, CardNumber.Queen, CardNumber.King, CardNumber.Queen)]
        [InlineData(CardNumber.Five, CardNumber.Ten, CardNumber.Ten, CardNumber.Five)]
        [InlineData(CardNumber.Ace, CardNumber.Two, CardNumber.Two, CardNumber.Ace)]
        [InlineData(CardNumber.Seven, CardNumber.Seven, CardNumber.Seven, CardNumber.Seven)]
        public void Test_Sort_Returns_Higher_Card_First(CardNumber card1, CardNumber card2, CardNumber expectedFirst, CardNumber expectedSecond) {
            var (sortedCard1, sortedCard2) = card1.Sort(card2);
            
            Assert.Equal(expectedFirst, sortedCard1);
            Assert.Equal(expectedSecond, sortedCard2);
        }

        [Theory]
        [InlineData(CardNumber.Ace, CardNumber.Ace, true)]
        [InlineData(CardNumber.King, CardNumber.King, true)]
        [InlineData(CardNumber.Two, CardNumber.Two, true)]
        [InlineData(CardNumber.Ace, CardNumber.King, false)]
        [InlineData(CardNumber.Five, CardNumber.Six, false)]
        public void Test_IsAPairWith_Returns_Correct_Result(CardNumber card1, CardNumber card2, bool expected) {
            var result = card1.IsAPairWith(card2);
            
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(CardNumber.Ace, CardNumber.King, true)]
        [InlineData(CardNumber.King, CardNumber.Ace, true)]
        [InlineData(CardNumber.Ace, CardNumber.Ace, true)]
        [InlineData(CardNumber.King, CardNumber.Queen, false)]
        [InlineData(CardNumber.Two, CardNumber.Three, false)]
        public void Test_ContainsAnAce_TwoCards_Returns_Correct_Result(CardNumber card1, CardNumber card2, bool expected) {
            var result = card1.ContainsAnAce(card2);
            
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Test_ContainsAnAce_MultipleCards_With_Ace() {
            var result = CardNumber.King.ContainsAnAce(CardNumber.Queen, CardNumber.Ace, CardNumber.Two);
            
            Assert.True(result);
        }

        [Fact]
        public void Test_ContainsAnAce_MultipleCards_Without_Ace() {
            var result = CardNumber.King.ContainsAnAce(CardNumber.Queen, CardNumber.Jack, CardNumber.Two);
            
            Assert.False(result);
        }

        [Fact]
        public void Test_ContainsAnAce_FirstCard_Is_Ace() {
            var result = CardNumber.Ace.ContainsAnAce(CardNumber.Queen, CardNumber.King);
            
            Assert.True(result);
        }

        [Fact]
        public void Test_ContainsAnAce_Empty_Array() {
            var result = CardNumber.King.ContainsAnAce();
            
            Assert.False(result);
        }

        [Fact]
        public void Test_ContainsAnAce_Single_Card_Array_With_Ace() {
            var result = CardNumber.King.ContainsAnAce(CardNumber.Ace);
            
            Assert.True(result);
        }

        [Fact]
        public void Test_ContainsAnAce_Single_Card_Array_Without_Ace() {
            var result = CardNumber.King.ContainsAnAce(CardNumber.Queen);
            
            Assert.False(result);
        }
    }

    public class CardSuitExtensionsTests {
        [Theory]
        [InlineData(CardSuit.Club, "(c)")]
        [InlineData(CardSuit.Diamond, "(d)")]
        [InlineData(CardSuit.Heart, "(h)")]
        [InlineData(CardSuit.Spade, "(s)")]
        public void Test_GetFriendlyString_Without_Symbols(CardSuit suit, string expected) {
            var result = suit.GetFriendlyString();
            
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(CardSuit.Club, false, "(c)")]
        [InlineData(CardSuit.Diamond, false, "C")]
        [InlineData(CardSuit.Heart, false, "(h)")]
        [InlineData(CardSuit.Spade, false, "(s)")]
        [InlineData(CardSuit.Club, true, "♣")]
        [InlineData(CardSuit.Diamond, true, "♦")]
        [InlineData(CardSuit.Heart, true, "♥")]
        [InlineData(CardSuit.Spade, true, "♠")]
        public void Test_GetFriendlyString_With_UseSymbols_Parameter(CardSuit suit, bool useSymbols, string expected) {
            var result = suit.GetFriendlyString(useSymbols);
            
            Assert.Equal(expected, result);
        }
    }

    public class CardTests_Additional {
        [Fact]
        public void Test_Card_Equals_Same_Card() {
            var card1 = new Card { Number = CardNumber.Ace, Suit = CardSuit.Spade };
            var card2 = new Card { Number = CardNumber.Ace, Suit = CardSuit.Spade };
            
            Assert.True(card1.Equals(card2));
            Assert.True(card2.Equals(card1));
        }

        [Fact]
        public void Test_Card_Equals_Different_Card() {
            var card1 = new Card { Number = CardNumber.Ace, Suit = CardSuit.Spade };
            var card2 = new Card { Number = CardNumber.King, Suit = CardSuit.Spade };
            
            Assert.False(card1.Equals(card2));
            Assert.False(card2.Equals(card1));
        }

        [Fact]
        public void Test_Card_Equals_Null() {
            var card = new Card { Number = CardNumber.Ace, Suit = CardSuit.Spade };
            
            Assert.False(card.Equals(null));
        }

        [Fact]
        public void Test_Card_Equals_Different_Type() {
            var card = new Card { Number = CardNumber.Ace, Suit = CardSuit.Spade };
            var notACard = "not a card";
            
            Assert.False(card.Equals(notACard));
        }

        [Fact]
        public void Test_Card_GetHashCode_Same_Cards() {
            var card1 = new Card { Number = CardNumber.Ace, Suit = CardSuit.Spade };
            var card2 = new Card { Number = CardNumber.Ace, Suit = CardSuit.Spade };
            
            Assert.Equal(card1.GetHashCode(), card2.GetHashCode());
        }

        [Fact]
        public void Test_Card_ToString_Default() {
            var card = new Card { Number = CardNumber.Ace, Suit = CardSuit.Spade };
            var result = card.ToString();
            
            Assert.Equal("A(s)", result);
        }

        [Fact]
        public void Test_Card_ToString_With_Symbols() {
            var card = new Card { Number = CardNumber.Ace, Suit = CardSuit.Heart };
            var result = card.ToString(true);
            
            Assert.Equal("A♥", result);
        }

        [Fact]
        public void Test_Card_ToString_Without_Symbols() {
            var card = new Card { Number = CardNumber.King, Suit = CardSuit.Club };
            var result = card.ToString(false);
            
            Assert.Equal("K(c)", result);
        }

        [Fact]
        public void Test_Card_WasCardBurned_Default() {
            var card = new Card { Number = CardNumber.Two, Suit = CardSuit.Diamond };
            
            Assert.False(card.WasCardBurned);
        }

        [Fact]
        public void Test_Card_WasCardBurned_Set() {
            var card = new Card { Number = CardNumber.Two, Suit = CardSuit.Diamond };
            card.WasCardBurned = true;
            
            Assert.True(card.WasCardBurned);
        }
    }
}
