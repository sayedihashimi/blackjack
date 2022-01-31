using SayedHa.Blackjack.Shared;
using System.Collections.Generic;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class Test_CardDeck {
        [Fact]
        public void Test_GetNewStandardCardDeck_ReturnsCorrectCards() {
            var cards = new CardDeckFactory().GetDeckStandardDeckOfCards(1, true);

            var cardsToTest = GetStandardDeckOfCards();

            Assert.NotNull(cards);
            foreach(var card in cardsToTest) {
                Assert.True(cards.Contains(card));
            }
        }

        public static List<Card> GetStandardDeckOfCards() {
            var standardDeck = new List<Card>();
            standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Ace });
            standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Two });
            standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Three });
            standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Four });
            standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Five });
            standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Six });
            standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Seven });
            standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Eight });
            standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Nine });
            standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Ten });
            standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Jack });
            standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Queen });
            standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.King });
            standardDeck.Add(new Card { Suit = CardSuit.Diamond, Number = CardNumber.Ace });
            standardDeck.Add(new Card { Suit = CardSuit.Diamond, Number = CardNumber.Two });
            standardDeck.Add(new Card { Suit = CardSuit.Diamond, Number = CardNumber.Three });
            standardDeck.Add(new Card { Suit = CardSuit.Diamond, Number = CardNumber.Four });
            standardDeck.Add(new Card { Suit = CardSuit.Diamond, Number = CardNumber.Five });
            standardDeck.Add(new Card { Suit = CardSuit.Diamond, Number = CardNumber.Six });
            standardDeck.Add(new Card { Suit = CardSuit.Diamond, Number = CardNumber.Seven });
            standardDeck.Add(new Card { Suit = CardSuit.Diamond, Number = CardNumber.Eight });
            standardDeck.Add(new Card { Suit = CardSuit.Diamond, Number = CardNumber.Nine });
            standardDeck.Add(new Card { Suit = CardSuit.Diamond, Number = CardNumber.Ten });
            standardDeck.Add(new Card { Suit = CardSuit.Diamond, Number = CardNumber.Jack });
            standardDeck.Add(new Card { Suit = CardSuit.Diamond, Number = CardNumber.Queen });
            standardDeck.Add(new Card { Suit = CardSuit.Diamond, Number = CardNumber.King });
            standardDeck.Add(new Card { Suit = CardSuit.Spade, Number = CardNumber.Ace });
            standardDeck.Add(new Card { Suit = CardSuit.Spade, Number = CardNumber.Two });
            standardDeck.Add(new Card { Suit = CardSuit.Spade, Number = CardNumber.Three });
            standardDeck.Add(new Card { Suit = CardSuit.Spade, Number = CardNumber.Four });
            standardDeck.Add(new Card { Suit = CardSuit.Spade, Number = CardNumber.Five });
            standardDeck.Add(new Card { Suit = CardSuit.Spade, Number = CardNumber.Six });
            standardDeck.Add(new Card { Suit = CardSuit.Spade, Number = CardNumber.Seven });
            standardDeck.Add(new Card { Suit = CardSuit.Spade, Number = CardNumber.Eight });
            standardDeck.Add(new Card { Suit = CardSuit.Spade, Number = CardNumber.Nine });
            standardDeck.Add(new Card { Suit = CardSuit.Spade, Number = CardNumber.Ten });
            standardDeck.Add(new Card { Suit = CardSuit.Spade, Number = CardNumber.Jack });
            standardDeck.Add(new Card { Suit = CardSuit.Spade, Number = CardNumber.Queen });
            standardDeck.Add(new Card { Suit = CardSuit.Spade, Number = CardNumber.King });
            standardDeck.Add(new Card { Suit = CardSuit.Club, Number = CardNumber.Ace });
            standardDeck.Add(new Card { Suit = CardSuit.Club, Number = CardNumber.Two });
            standardDeck.Add(new Card { Suit = CardSuit.Club, Number = CardNumber.Three });
            standardDeck.Add(new Card { Suit = CardSuit.Club, Number = CardNumber.Four });
            standardDeck.Add(new Card { Suit = CardSuit.Club, Number = CardNumber.Five });
            standardDeck.Add(new Card { Suit = CardSuit.Club, Number = CardNumber.Six });
            standardDeck.Add(new Card { Suit = CardSuit.Club, Number = CardNumber.Seven });
            standardDeck.Add(new Card { Suit = CardSuit.Club, Number = CardNumber.Eight });
            standardDeck.Add(new Card { Suit = CardSuit.Club, Number = CardNumber.Nine });
            standardDeck.Add(new Card { Suit = CardSuit.Club, Number = CardNumber.Ten });
            standardDeck.Add(new Card { Suit = CardSuit.Club, Number = CardNumber.Jack });
            standardDeck.Add(new Card { Suit = CardSuit.Club, Number = CardNumber.Queen });
            standardDeck.Add(new Card { Suit = CardSuit.Club, Number = CardNumber.King });
            return standardDeck;
        }
    }
}