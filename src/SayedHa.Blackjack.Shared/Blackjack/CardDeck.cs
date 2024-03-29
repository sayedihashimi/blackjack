﻿// This file is part of SayedHa.Blackjack.
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
        public CardDeck(ILogger logger, LinkedList<Card>? cards, int numDecks) {
            _logger = logger;
            Cards = cards;
            CurrentCard = Cards != null ? Cards.First : null;
            NumDecks = numDecks;
        }

        private ILogger _logger;
        internal LinkedList<Card>? Cards { get; set; }
        internal LinkedListNode<Card>? CurrentCard { get; set; }
        public List<Card> DiscardedCards { get; internal set; } = new List<Card>();
        public int NumDecks { get; protected init; }

        public List<Card> GetRemainingCardsAsList() {
            var remainingCards = new List<Card> ();

            var currentCard = CurrentCard;
            while (currentCard != null) {
                remainingCards.Add(currentCard.Value);
                currentCard = currentCard.Next;
            }

            return remainingCards;
        }

        List<Card>? _cardList;
        public void ShuffleCards() {
            // clear the discarded cards
            // rebuild the linkedlist
            // DiscardedCards.Clear();
            DiscardedCards = new List<Card>();
            if(_cardList == null) {
                _cardList = Cards!.ToList();
            }
            _cardList.Shuffle();
            //var cardList = Cards!.ToList();
            //cardList.Shuffle();
            Cards = new LinkedList<Card>(_cardList);
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
        /// <summary>
        /// This will take the next card and move it to
        /// the discard pile and mark the card as burned.
        /// </summary>
        public void DiscardACard() {
            var card = GetCardAndMoveNext();
            if(card != null) {
                card.WasCardBurned = true;
            }
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
                sb.Append(card.ToString());
                sb.Append(" ");
            }
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }
    }
}
