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
using System.Runtime.CompilerServices;

namespace SayedHa.Blackjack.Shared {
    public class CardDeckFactory {
        public CardDeck CreateCardDeck(int numDecks, bool shuffle, ILogger? logger=null) {
            Debug.Assert(numDecks > 0);
            logger = logger ?? NullLogger.Instance;

            var cardList = new List<Card>();
            for (var i = 0; i < numDecks; i++) {
                var temp = GetStandardDeckOfCardsAsList(false);
                cardList.AddRange(temp);
            }

            if (shuffle) {
                cardList.Shuffle();
            }

            return new CardDeck(logger, cardList.ConvertToLinkedList(), numDecks);
        }
        public CardDeck CreateCardDeck(List<Card> cards, int numDecks, ILogger? logger = null) {
            Debug.Assert(cards != null && cards.Count > 0);
            logger = logger ?? NullLogger.Instance;

            return new CardDeck(logger, cards.ConvertToLinkedList(), numDecks);
        }

        internal List<Card> GetStandardDeckOfCardsAsList(bool shuffle) {
            var cards = new List<Card>();
            foreach (CardSuit suit in GetAllSuits()) {
                foreach (CardNumber value in GetAllCardNumbers()) {
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
        private static CardSuit[]? _allSuits;
        public static CardSuit[] GetAllSuits() {
            if (_allSuits is null) {
                _allSuits = (CardSuit[])Enum.GetValues(typeof(CardSuit));
            }
            return _allSuits;
        }
        private static readonly CardNumber[] _allCardNumbers = (CardNumber[])Enum.GetValues(typeof(CardNumber));
        public static CardNumber[] GetAllCardNumbers() {
            return _allCardNumbers;
        }
    }
}
