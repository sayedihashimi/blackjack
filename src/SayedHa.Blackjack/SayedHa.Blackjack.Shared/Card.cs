using SayedHa.Blackjack.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared {
    public class Card {
        public CardSuit Suit { get; init; }
        public CardValue Value { get; init; }
        public override bool Equals(object? obj) {
            var other = obj as Card;
            if (other == null) return false;

            return other.Suit == this.Suit && other.Value == this.Value;
        }
        override public int GetHashCode() {
            return this.Suit.GetHashCode() * this.Value.GetHashCode();
        }
    }

    public class CardDeck {
        public List<Card>? Cards { get; set; }

        public static CardDeck GetNewStandardCardDeck(bool shuffle=false) {

            var cards = new List<Card>();
            foreach(CardSuit suit in (CardSuit[])Enum.GetValues(typeof(CardSuit))) {
                foreach(CardValue value in (CardValue[])Enum.GetValues(typeof(CardValue))) {
                    cards.Add(new Card {
                        Suit = suit,
                        Value = value
                    });
                }
            }

            if (shuffle) {
                cards.Shuffle();
            }

            return new CardDeck { Cards = cards };
        }
    }

    public enum CardSuit {
        Heart,
        Diamond,
        Spade,
        Club
    }
    public enum CardValue {
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
}
