using SayedHa.Blackjack.Shared;
using System;
using System.Collections.Generic;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class Test_CardDeck {
        [Fact]
        public void Test_GetNewStandardCardDeck_ReturnsCorrectCards() {
            var cards = new CardDeckFactory().GetDeckStandardDeckOfCards(1, true);

            var allStandardCards = GetStandardDeckOfCards();

            Assert.NotNull(cards);
            foreach(var card in allStandardCards) {
                Assert.True(cards.Contains(card));
            }
        }

        [Fact]
        public void Test_MoveNext_EnumeratesAllCards_For_One_Deck() {
            var cards = new CardDeckFactory().GetDeckStandardDeckOfCards(1, true);
            var cardsAsList = new List<Card>();
            var nextCard = cards.GetCardAndMoveNext();
            do {
                if(nextCard != null) {
                    cardsAsList.Add(nextCard);
                }
                nextCard = cards.GetCardAndMoveNext();
            }while(nextCard != null);

            var allStandardCards = GetStandardDeckOfCards();
            foreach (var card in allStandardCards) {
                Assert.True(cardsAsList?.Contains(card));
            }
            
        }
        [Fact]
        public void Test_Shuffle_For_One_Deck() {
            CardDeck cards = new CardDeckFactory().GetDeckStandardDeckOfCards(1, true);
            // check the first ten cards to ensure they don't shrae the same suit
            var firstSuit = cards.GetCurrentCard().Suit;
            int numToTest = 10;
            bool foundDifferentSuit = false;
            for (int i = 0; i < numToTest; i++) {
                var currentCard = cards.GetCardAndMoveNext();
                if (currentCard != null) { 
                    if (currentCard.Suit != firstSuit) {
                        foundDifferentSuit = true;
                        break;
                    }
                }
                else {
                    throw new ApplicationException($"currentCard is null when it shouldn't");
                }
            }

            Assert.True(foundDifferentSuit);
        }

        // helper methods
        internal static List<Card> GetStandardDeckOfCards() {
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