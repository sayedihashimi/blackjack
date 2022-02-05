
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

        // note: these characters don't seem to be supported in the fonts used in the terminal
        public override string ToString() => $"{Number.GetFriendlyString()}{Suit.GetFriendlyString()}";
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
                // 11 value must come first here or the Hand.GetScore method must be updated
                case CardNumber.Ace: return new int[] { 11, 1 };
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
        public static string GetFriendlyString(this CardNumber cardNumber) {
            switch (cardNumber) {
                // 11 value must come first here or the Hand.GetScore method must be updated
                case CardNumber.Ace: return "A";
                case CardNumber.Two: return "2";
                case CardNumber.Three: return "3";
                case CardNumber.Four: return "4";
                case CardNumber.Five: return "5";
                case CardNumber.Six: return "6";
                case CardNumber.Seven: return "7";
                case CardNumber.Eight: return "8";
                case CardNumber.Nine: return "9";
                case CardNumber.Ten: return "10";
                case CardNumber.Jack: return "J";
                case CardNumber.Queen: return "Q";
                case CardNumber.King: return "K";
                default: throw new ApplicationException($"Unknown card number:'{cardNumber}'");
            }
        }
    }
    public static class CardSuitExtensions {
        public static string GetFriendlyString(this CardSuit cardSuit) => cardSuit switch {
            CardSuit.Club => "(c)",
            CardSuit.Diamond => "(d)",
            CardSuit.Heart => "(h)",
            CardSuit.Spade => "(s)",
            _ => throw new NotImplementedException()
        };
    }
}
