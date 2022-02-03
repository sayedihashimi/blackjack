using SayedHa.Blackjack.Shared.Extensions;
using System.Diagnostics;

namespace SayedHa.Blackjack.Shared {
    public class CardDeckFactory {
        public CardDeck GetDeckStandardDeckOfCards(int numDecks, bool shuffle, ILogger? logger=null) {
            Debug.Assert(numDecks > 0);
            if (logger == null) {
                logger = new NullLogger();
            }
            var cardList = new List<Card>();
            for (var i = 0; i < numDecks; i++) {
                var temp = GetStandardDeckOfCardsAsList(false);
                cardList.AddRange(temp);
            }

            if (shuffle) {
                cardList.Shuffle();
            }

            // convert into a linkedlist
            LinkedList<Card> cards = new LinkedList<Card>();
            foreach (var card in cardList) {
                cards.AddLast(card);
            }

            return new CardDeck(logger, cards);
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
