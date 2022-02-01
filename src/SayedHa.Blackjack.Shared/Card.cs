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
        public override string ToString() => (Number, Suit) switch {
            (CardNumber.Ace, CardSuit.Heart) => "🂱",
            (CardNumber.Two, CardSuit.Heart) => "🂲",
            (CardNumber.Three, CardSuit.Heart) => "🂳",
            (CardNumber.Four, CardSuit.Heart) => "🂴",
            (CardNumber.Five, CardSuit.Heart) => "🂵",
            (CardNumber.Six, CardSuit.Heart) => "🂶",
            (CardNumber.Seven, CardSuit.Heart) => "🂷",
            (CardNumber.Eight, CardSuit.Heart) => "🂸",
            (CardNumber.Nine, CardSuit.Heart) => "🂹",
            (CardNumber.Ten, CardSuit.Heart) => "🂺",
            (CardNumber.Jack, CardSuit.Heart) => "🂻",
            (CardNumber.Queen, CardSuit.Heart) => "🂽",
            (CardNumber.King, CardSuit.Heart) => "🂾",
            (CardNumber.Ace, CardSuit.Diamond) => "🃁",
            (CardNumber.Two, CardSuit.Diamond) => "🃂",
            (CardNumber.Three, CardSuit.Diamond) => "🃃",
            (CardNumber.Four, CardSuit.Diamond) => "🃄",
            (CardNumber.Five, CardSuit.Diamond) => "🃅",
            (CardNumber.Six, CardSuit.Diamond) => "🃆",
            (CardNumber.Seven, CardSuit.Diamond) => "🃇",
            (CardNumber.Eight, CardSuit.Diamond) => "🃈",
            (CardNumber.Nine, CardSuit.Diamond) => "🃉",
            (CardNumber.Ten, CardSuit.Diamond) => "🃊",
            (CardNumber.Jack, CardSuit.Diamond) => "🃋",
            (CardNumber.Queen, CardSuit.Diamond) => "🃍",
            (CardNumber.King, CardSuit.Diamond) => "🃎",
            (CardNumber.Ace, CardSuit.Spade) => "🂡",
            (CardNumber.Two, CardSuit.Spade) => "🂢",
            (CardNumber.Three, CardSuit.Spade) => "🂣",
            (CardNumber.Four, CardSuit.Spade) => "🂤",
            (CardNumber.Five, CardSuit.Spade) => "🂥",
            (CardNumber.Six, CardSuit.Spade) => "🂦",
            (CardNumber.Seven, CardSuit.Spade) => "🂧",
            (CardNumber.Eight, CardSuit.Spade) => "🂨",
            (CardNumber.Nine, CardSuit.Spade) => "🂩",
            (CardNumber.Ten, CardSuit.Spade) => "🂪",
            (CardNumber.Jack, CardSuit.Spade) => "🂪",
            (CardNumber.Queen, CardSuit.Spade) => "🂭",
            (CardNumber.King, CardSuit.Spade) => "🂮",
            (CardNumber.Ace, CardSuit.Club) => "🃑",
            (CardNumber.Two, CardSuit.Club) => "🃒",
            (CardNumber.Three, CardSuit.Club) => "🃓",
            (CardNumber.Four, CardSuit.Club) => "🃔",
            (CardNumber.Five, CardSuit.Club) => "🃕",
            (CardNumber.Six, CardSuit.Club) => "🃖",
            (CardNumber.Seven, CardSuit.Club) => "🃗",
            (CardNumber.Eight, CardSuit.Club) => "🃘",
            (CardNumber.Nine, CardSuit.Club) => "🃙",
            (CardNumber.Ten, CardSuit.Club) => "🃚",
            (CardNumber.Jack, CardSuit.Club) => "🃛",
            (CardNumber.Queen, CardSuit.Club) => "🃝",
            (CardNumber.King, CardSuit.Club) => "🃞",
            _ => throw new ApplicationException($"Unknown card number & suit: Number='{Number}',Suit='{Suit}'")
        };

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
        public static string GetFriendlyString(this CardSuit cardSuit) {
            switch (cardSuit) {
                case CardSuit.Club: return "♥"; // "♧";
                case CardSuit.Diamond: return "♦"; // "♢";
                case CardSuit.Heart: return "♥"; // "♡";
                case CardSuit.Spade: return "♠"; // "♤";
            }
            throw new NotImplementedException();
        }
    }
}
