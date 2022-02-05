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
