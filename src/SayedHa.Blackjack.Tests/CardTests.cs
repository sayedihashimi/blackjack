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
using SayedHa.Blackjack.Shared.Betting;
using System;
using System.Collections.Generic;
using Xunit;


namespace SayedHa.Blackjack.Tests {
    public class Test_CardDeck {
        [Fact]
        public void Test_GetNewStandardCardDeck_ReturnsCorrectCards() {
            var cards = new CardDeckFactory().CreateCardDeck(1, true);

            var allStandardCards = CardDeckHelper.CreateListOfAStandardDeckOfCards(4);

            Assert.NotNull(cards);
            foreach(var card in allStandardCards) {
                Assert.True(cards.Contains(card));
            }
        }

        [Fact]
        public void Test_MoveNext_EnumeratesAllCards_For_One_Deck() {
            var cards = new CardDeckFactory().CreateCardDeck(1, true);
            var cardsAsList = new List<Card>();
            var nextCard = cards.GetCardAndMoveNext();
            do {
                if(nextCard != null) {
                    cardsAsList.Add(nextCard);
                }
                nextCard = cards.GetCardAndMoveNext();
            }while(nextCard != null);

            var allStandardCards = CardDeckHelper.CreateListOfAStandardDeckOfCards(4);
            foreach (var card in allStandardCards) {
                Assert.True(cardsAsList?.Contains(card));
            }
            
        }
        [Fact]
        public void Test_Shuffle_For_One_Deck() {
            CardDeck cards = new CardDeckFactory().CreateCardDeck(1, true);
            // check the first ten cards to ensure they don't share the same suit
            var firstSuit = cards?.GetCurrentCard()?.Suit;
            int numToTest = 10;
            bool foundDifferentSuit = false;
            for (int i = 0; i < numToTest; i++) {
                var currentCard = cards?.GetCardAndMoveNext();
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

        [Fact]
        public void Test_ToString() {
            CardDeck cards = new CardDeckFactory().CreateCardDeck(1, true);
            var str = cards.ToString();
            Assert.NotNull(str);
            Assert.NotEmpty(str);
            Console.WriteLine(str);
        }
        [Fact]
        public void Test_GetRemainingCardsAsList() {
            CardDeck deck = new CardDeckFactory().CreateCardDeck(1, false);
            // deal a few cards
            int cardsToDeal = 10;
            for (var i = 0; i < cardsToDeal; i++) {
                deck.GetCardAndMoveNext();
            }

            var remainingCards = deck.GetRemainingCardsAsList();
            Assert.NotNull(remainingCards);
            Assert.Equal(52-cardsToDeal, remainingCards.Count);
            Assert.Equal(52, remainingCards.Count + deck.DiscardedCards.Count);

            // make sure no card in DiscardedCards is in remainingcards
            foreach (var card in deck.DiscardedCards) {
                Assert.DoesNotContain<Card>(card, remainingCards);
            }
        }

        

    }
}