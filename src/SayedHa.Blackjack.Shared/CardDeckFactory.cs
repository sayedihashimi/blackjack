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
using SayedHa.Blackjack.Shared.Extensions;
using System.Diagnostics;

namespace SayedHa.Blackjack.Shared {
    public class CardDeckFactory {
        public CardDeck CreateCardDeck(int numDecks, bool shuffle, ILogger? logger=null) {
            Debug.Assert(numDecks > 0);
            logger = logger ?? new NullLogger();

            var cardList = new List<Card>();
            for (var i = 0; i < numDecks; i++) {
                var temp = GetStandardDeckOfCardsAsList(false);
                cardList.AddRange(temp);
            }

            if (shuffle) {
                cardList.Shuffle();
            }

            return new CardDeck(logger, cardList.ConvertToLinkedList());
        }
        public CardDeck CreateCardDeck(List<Card> cards, ILogger? logger = null) {
            Debug.Assert(cards != null && cards.Count > 0);
            logger = logger ?? new NullLogger();

            return new CardDeck(logger, cards.ConvertToLinkedList());
        }

        internal List<Card> GetStandardDeckOfCardsAsList(bool shuffle) {
            var cards = new List<Card>();
            foreach (CardSuit suit in (CardSuit[])Enum.GetValues(typeof(CardSuit))) {
                foreach (CardNumber value in (CardNumber[])Enum.GetValues(typeof(CardNumber))) {
                    cards.Add(new Card {
                        Suit = suit,
                        Number = value
                    });
                }
            }

            if (shuffle) {
                cards.Shuffle();
            }

            return cards;
        }
    }
}
