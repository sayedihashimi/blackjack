using SayedHa.Blackjack.Shared;
using System.Collections.Generic;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class Test_CardDeck {
        [Fact]
        public void Test_GetNewStandardCardDeck_ReturnsCorrectCards() {
            var result = CardDeck.GetNewStandardCardDeck(false);

            Assert.NotNull(result);
            Assert.Equal(52, result.Cards?.Count);

            var cardsToTest = new List<Card>();
            cardsToTest.Add(new Card { Suit = CardSuit.Heart, Value = CardValue.Ace });
            cardsToTest.Add(new Card { Suit = CardSuit.Heart, Value = CardValue.Two });
            cardsToTest.Add(new Card { Suit = CardSuit.Heart, Value = CardValue.Three });
            cardsToTest.Add(new Card { Suit = CardSuit.Heart, Value = CardValue.Four });
            cardsToTest.Add(new Card { Suit = CardSuit.Heart, Value = CardValue.Five });
            cardsToTest.Add(new Card { Suit = CardSuit.Heart, Value = CardValue.Six });
            cardsToTest.Add(new Card { Suit = CardSuit.Heart, Value = CardValue.Seven });
            cardsToTest.Add(new Card { Suit = CardSuit.Heart, Value = CardValue.Eight });
            cardsToTest.Add(new Card { Suit = CardSuit.Heart, Value = CardValue.Nine });
            cardsToTest.Add(new Card { Suit = CardSuit.Heart, Value = CardValue.Ten });
            cardsToTest.Add(new Card { Suit = CardSuit.Heart, Value = CardValue.Jack });
            cardsToTest.Add(new Card { Suit = CardSuit.Heart, Value = CardValue.Queen });
            cardsToTest.Add(new Card { Suit = CardSuit.Heart, Value = CardValue.King });
            cardsToTest.Add(new Card { Suit = CardSuit.Diamond, Value = CardValue.Ace });
            cardsToTest.Add(new Card { Suit = CardSuit.Diamond, Value = CardValue.Two });
            cardsToTest.Add(new Card { Suit = CardSuit.Diamond, Value = CardValue.Three });
            cardsToTest.Add(new Card { Suit = CardSuit.Diamond, Value = CardValue.Four });
            cardsToTest.Add(new Card { Suit = CardSuit.Diamond, Value = CardValue.Five });
            cardsToTest.Add(new Card { Suit = CardSuit.Diamond, Value = CardValue.Six });
            cardsToTest.Add(new Card { Suit = CardSuit.Diamond, Value = CardValue.Seven });
            cardsToTest.Add(new Card { Suit = CardSuit.Diamond, Value = CardValue.Eight });
            cardsToTest.Add(new Card { Suit = CardSuit.Diamond, Value = CardValue.Nine });
            cardsToTest.Add(new Card { Suit = CardSuit.Diamond, Value = CardValue.Ten });
            cardsToTest.Add(new Card { Suit = CardSuit.Diamond, Value = CardValue.Jack });
            cardsToTest.Add(new Card { Suit = CardSuit.Diamond, Value = CardValue.Queen });
            cardsToTest.Add(new Card { Suit = CardSuit.Diamond, Value = CardValue.King });
            cardsToTest.Add(new Card { Suit = CardSuit.Spade, Value = CardValue.Ace });
            cardsToTest.Add(new Card { Suit = CardSuit.Spade, Value = CardValue.Two });
            cardsToTest.Add(new Card { Suit = CardSuit.Spade, Value = CardValue.Three });
            cardsToTest.Add(new Card { Suit = CardSuit.Spade, Value = CardValue.Four });
            cardsToTest.Add(new Card { Suit = CardSuit.Spade, Value = CardValue.Five });
            cardsToTest.Add(new Card { Suit = CardSuit.Spade, Value = CardValue.Six });
            cardsToTest.Add(new Card { Suit = CardSuit.Spade, Value = CardValue.Seven });
            cardsToTest.Add(new Card { Suit = CardSuit.Spade, Value = CardValue.Eight });
            cardsToTest.Add(new Card { Suit = CardSuit.Spade, Value = CardValue.Nine });
            cardsToTest.Add(new Card { Suit = CardSuit.Spade, Value = CardValue.Ten });
            cardsToTest.Add(new Card { Suit = CardSuit.Spade, Value = CardValue.Jack });
            cardsToTest.Add(new Card { Suit = CardSuit.Spade, Value = CardValue.Queen });
            cardsToTest.Add(new Card { Suit = CardSuit.Spade, Value = CardValue.King });
            cardsToTest.Add(new Card { Suit = CardSuit.Club, Value = CardValue.Ace });
            cardsToTest.Add(new Card { Suit = CardSuit.Club, Value = CardValue.Two });
            cardsToTest.Add(new Card { Suit = CardSuit.Club, Value = CardValue.Three });
            cardsToTest.Add(new Card { Suit = CardSuit.Club, Value = CardValue.Four });
            cardsToTest.Add(new Card { Suit = CardSuit.Club, Value = CardValue.Five });
            cardsToTest.Add(new Card { Suit = CardSuit.Club, Value = CardValue.Six });
            cardsToTest.Add(new Card { Suit = CardSuit.Club, Value = CardValue.Seven });
            cardsToTest.Add(new Card { Suit = CardSuit.Club, Value = CardValue.Eight });
            cardsToTest.Add(new Card { Suit = CardSuit.Club, Value = CardValue.Nine });
            cardsToTest.Add(new Card { Suit = CardSuit.Club, Value = CardValue.Ten });
            cardsToTest.Add(new Card { Suit = CardSuit.Club, Value = CardValue.Jack });
            cardsToTest.Add(new Card { Suit = CardSuit.Club, Value = CardValue.Queen });
            cardsToTest.Add(new Card { Suit = CardSuit.Club, Value = CardValue.King });
            
            foreach(var ct in cardsToTest) {
                Assert.True(result.Cards?.Contains(ct));
            }

        }
    }
}