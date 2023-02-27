using SayedHa.Blackjack.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Tests {
    public static class CardDeckHelper {
        /// <summary>
        /// This method is for testing purposes and the ordering of these cards will not change.
        /// </summary>
        /// <param name="numDecks">Number of decks to create</param>
        /// <returns>List of cards in a specific sequence</returns>
        public static List<Card> CreateListOfAStandardDeckOfCards(int numDecks) {
            var standardDeck = new List<Card>();

            for(var i = 0; i < numDecks; i++) {
                // preserve order to keep tests passing.
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
            }

            return standardDeck;
        }
    }
}
