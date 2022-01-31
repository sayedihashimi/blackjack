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
        public override string ToString() {
            switch (Number, Suit) {
                case (CardNumber.Ace, CardSuit.Heart): return "🂱";
                case (CardNumber.Two, CardSuit.Heart): return "🂲";
                case (CardNumber.Three, CardSuit.Heart): return "🂳";
                case (CardNumber.Four, CardSuit.Heart): return "🂴";
                case (CardNumber.Five, CardSuit.Heart): return "🂵";
                case (CardNumber.Six, CardSuit.Heart): return "🂶";
                case (CardNumber.Seven, CardSuit.Heart): return "🂷";
                case (CardNumber.Eight, CardSuit.Heart): return "🂸";
                case (CardNumber.Nine, CardSuit.Heart): return "🂹";
                case (CardNumber.Ten, CardSuit.Heart): return "🂺";
                case (CardNumber.Jack, CardSuit.Heart): return "🂻";
                case (CardNumber.Queen, CardSuit.Heart): return "🂽";
                case (CardNumber.King, CardSuit.Heart): return "🂾";
                case (CardNumber.Ace, CardSuit.Diamond): return "🃁";
                case (CardNumber.Two, CardSuit.Diamond): return "🃂";
                case (CardNumber.Three, CardSuit.Diamond): return "🃃";
                case (CardNumber.Four, CardSuit.Diamond): return "🃄";
                case (CardNumber.Five, CardSuit.Diamond): return "🃅";
                case (CardNumber.Six, CardSuit.Diamond): return "🃆";
                case (CardNumber.Seven, CardSuit.Diamond): return "🃇";
                case (CardNumber.Eight, CardSuit.Diamond): return "🃈";
                case (CardNumber.Nine, CardSuit.Diamond): return "🃉";
                case (CardNumber.Ten, CardSuit.Diamond): return "🃊";
                case (CardNumber.Jack, CardSuit.Diamond): return "🃋";
                case (CardNumber.Queen, CardSuit.Diamond): return "🃍";
                case (CardNumber.King, CardSuit.Diamond): return "🃎";
                case (CardNumber.Ace, CardSuit.Spade): return "🂡";
                case (CardNumber.Two, CardSuit.Spade): return "🂢";
                case (CardNumber.Three, CardSuit.Spade): return "🂣";
                case (CardNumber.Four, CardSuit.Spade): return "🂤";
                case (CardNumber.Five, CardSuit.Spade): return "🂥";
                case (CardNumber.Six, CardSuit.Spade): return "🂦";
                case (CardNumber.Seven, CardSuit.Spade): return "🂧";
                case (CardNumber.Eight, CardSuit.Spade): return "🂨";
                case (CardNumber.Nine, CardSuit.Spade): return "🂩";
                case (CardNumber.Ten, CardSuit.Spade): return "🂪";
                case (CardNumber.Jack, CardSuit.Spade): return "🂪";
                case (CardNumber.Queen, CardSuit.Spade): return "🂭";
                case (CardNumber.King, CardSuit.Spade): return "🂮";
                case (CardNumber.Ace, CardSuit.Club): return "🃑";
                case (CardNumber.Two, CardSuit.Club): return "🃒";
                case (CardNumber.Three, CardSuit.Club): return "🃓";
                case (CardNumber.Four, CardSuit.Club): return "🃔";
                case (CardNumber.Five, CardSuit.Club): return "🃕";
                case (CardNumber.Six, CardSuit.Club): return "🃖";
                case (CardNumber.Seven, CardSuit.Club): return "🃗";
                case (CardNumber.Eight, CardSuit.Club): return "🃘";
                case (CardNumber.Nine, CardSuit.Club): return "🃙";
                case (CardNumber.Ten, CardSuit.Club): return "🃚";
                case (CardNumber.Jack, CardSuit.Club): return "🃛";
                case (CardNumber.Queen, CardSuit.Club): return "🃝";
                case (CardNumber.King, CardSuit.Club): return "🃞";
                default: throw new ApplicationException($"Unknown card number & suit: Number='{Number}',Suit='{Suit}'");


            }

            throw new NotImplementedException();
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
