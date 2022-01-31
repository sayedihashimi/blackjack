using SayedHa.Blackjack.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared {
    public class Card {
        public CardSuit Suit { get; init; }
        public CardNumber Number { get; init; }
        public override bool Equals(object? obj) {
            var other = obj as Card;
            if (other == null) return false;

            return other.Suit == this.Suit && other.Number == this.Number;
        }
        override public int GetHashCode() {
            return this.Suit.GetHashCode() * this.Number.GetHashCode();
        }
    }

    public class CardDeck {
        internal LinkedList<Card>? Cards { get; set; }
        internal LinkedListNode<Card>? CurrentCard { get; set; }

        public CardDeck(LinkedList<Card>? cards) {
            Cards = cards;
            CurrentCard = Cards != null ? Cards.First : null;
        }


        public Card? MoveNext() {
            var retValue = CurrentCard?.Value;
            CurrentCard = CurrentCard?.Next;
            return retValue;
        }

        public bool Contains(Card card) {
            if (card == null) {
                throw new ArgumentNullException(nameof(card));
            }

            return Cards != null ? Cards.Contains(card) : throw new ApplicationException("CardDeck hasn't been initalized, Cards is null");
        }
    }
    public class CardDeckFactory {
        public CardDeck GetDeckStandardDeckOfCards(int numDecks = 1, bool shuffle = true) {
            Debug.Assert(numDecks > 0);

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

            return new CardDeck(cards);
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


    public enum CardSuit {
        Heart,
        Diamond,
        Spade,
        Club
    }
    public enum CardNumber {
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }
    public static class CardNumberExtension {
        public static int[] GetValues(this CardNumber cardNumber) {
            switch (cardNumber) {
                case CardNumber.Ace: return new int[] { 1, 11 };
                case CardNumber.Two: return new int[] { 2 };
                case CardNumber.Three: return new int[] { 3 };
                case CardNumber.Four: return new int[] { 4 };
                case CardNumber.Five: return new int[] { 5 };
                case CardNumber.Six: return new int[] { 6 };
                case CardNumber.Seven: return new int[] { 7 };
                case CardNumber.Eight: return new int[] { 8 };
                case CardNumber.Nine: return new int[] { 9 };
                case CardNumber.Ten: return new int[] { 10 };
                case CardNumber.Jack: return new int[] { 10 };
                case CardNumber.Queen: return new int[] { 10 };
                case CardNumber.King: return new int[] { 10 };
                default: throw new ApplicationException($"Unknown card number:'{cardNumber}'");
            }
        }
    }
}
