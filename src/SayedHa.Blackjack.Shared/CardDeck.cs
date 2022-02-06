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
using System.Text;

namespace SayedHa.Blackjack.Shared {
    public class CardDeck {
        private ILogger _logger;
        internal LinkedList<Card>? Cards { get; set; }
        internal LinkedListNode<Card>? CurrentCard { get; set; }
        public List<Card> DiscardedCards { get; internal set; } = new List<Card>();

        public List<Card> GetRemainingCardsAsList() {
            var remainingCards = new List<Card> ();

            var currentCard = CurrentCard;
            while (currentCard != null) {
                remainingCards.Add(currentCard.Value);
                currentCard = currentCard.Next;
            }

            return remainingCards;
        }

        public CardDeck(ILogger logger,LinkedList<Card>? cards) {
            _logger = logger;
            Cards = cards;
            CurrentCard = Cards != null ? Cards.First : null;
        }
        public void ShuffleCards() {
            // clear the discarded cards
            // rebuild the linkedlist
            DiscardedCards.Clear();
            var cardList = Cards!.ToList();
            cardList.Shuffle();
            Cards = new LinkedList<Card>(cardList);
            CurrentCard = Cards!.First;
        }

        public Card? GetCardAndMoveNext() {
            var retValue = CurrentCard?.Value;
            if(retValue != null) {
                DiscardedCards.Add(retValue);
            }
            CurrentCard = CurrentCard?.Next;
            return retValue;
        }

        public Card? GetCurrentCard() {
            return CurrentCard?.Value;
        }

        public bool Contains(Card card) {
            if (card == null) {
                throw new ArgumentNullException(nameof(card));
            }

            return Cards != null ? Cards.Contains(card) : throw new ApplicationException("CardDeck hasn't been initalized, Cards is null");
        }

        public int GetNumRemainingCards() => 
            Cards != null && DiscardedCards != null ? Cards.Count() - DiscardedCards.Count : 0;
        public int GetTotalNumCards() =>
            Cards != null ? Cards.Count() : 0;
        
        override public string ToString() {
            if(Cards == null) {
                return "(empty)";
            }
            var sb = new StringBuilder();
            foreach(var card in Cards) {
                sb.AppendLine(card.ToString());
            }
            return sb.ToString();
        }
    }
}
