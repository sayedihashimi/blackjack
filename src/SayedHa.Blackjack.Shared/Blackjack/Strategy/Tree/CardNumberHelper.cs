using SayedHa.Blackjack.Shared.Blackjack.Exceptions;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree {
    public static class CardNumberHelper {
        public static CardNumberOrScore ConvertToCardNumberOrScore(CardNumber cardNumber) => cardNumber switch {
            CardNumber.Ace => CardNumberOrScore.Ace,
            CardNumber.Two => CardNumberOrScore.Two,
            CardNumber.Three => CardNumberOrScore.Three,
            CardNumber.Four => CardNumberOrScore.Four,
            CardNumber.Five => CardNumberOrScore.Five,
            CardNumber.Six => CardNumberOrScore.Six,
            CardNumber.Seven => CardNumberOrScore.Seven,
            CardNumber.Eight => CardNumberOrScore.Eight,
            CardNumber.Nine => CardNumberOrScore.Nine,
            // setting these all to Ten to decrease the number of nodes in the tree
            CardNumber.Ten => CardNumberOrScore.Ten,
            CardNumber.Jack => CardNumberOrScore.Ten,
            CardNumber.Queen => CardNumberOrScore.Ten,
            CardNumber.King => CardNumberOrScore.Ten,
            _ => throw new UnknownValueException($"Unknown value for CardNumber: '{cardNumber}'")
        };
        public static CardNumberOrScore ConvertToCardNumberOrScore(int score) => score switch {
            > 21 => CardNumberOrScore.Busted,
            21 => CardNumberOrScore.Score21,
            20 => CardNumberOrScore.Score20,
            19 => CardNumberOrScore.Score19,
            18 => CardNumberOrScore.Score18,
            17 => CardNumberOrScore.Score17,
            16 => CardNumberOrScore.Score16,
            15 => CardNumberOrScore.Score15,
            14 => CardNumberOrScore.Score14,
            13 => CardNumberOrScore.Score13,
            12 => CardNumberOrScore.Score12,
            11 => CardNumberOrScore.Score11,
            10 => CardNumberOrScore.Score10,
            9 => CardNumberOrScore.Score9,
            8 => CardNumberOrScore.Score8,
            7 => CardNumberOrScore.Score7,
            6 => CardNumberOrScore.Score6,
            5 => CardNumberOrScore.Score5,
            4 => CardNumberOrScore.Score4,
            3 => CardNumberOrScore.Score3,
            2 => CardNumberOrScore.Score2,
            _ => throw new UnknownValueException($"Unknown value for CardNumber: '{score}'")
        };
        public static int GetScoreTotal(CardNumber card1, CardNumber card2) {
            // TODO: this could be refactored to avoid creating the Hand object
            var hand = new Hand(5F, NullLogger.Instance);
            hand.ReceiveCard(new Card { Number = card1 });
            hand.ReceiveCard(new Card { Number = card2 });
            return hand.GetScore();
        }
        public static int GetScoreTotal(params CardNumber[] cards) {
            // TODO: this could be refactored to avoid creating the Hand object
            var hand = new Hand(5F, NullLogger.Instance);
            foreach (var card in cards) {
                hand.ReceiveCard(new Card { Number = card });
            }
            return hand.GetScore();
        }
        public static bool ContainsAnAce(CardNumber[] cards) => 
            cards.Any(card => card == CardNumber.Ace);

        // Returns a list of cardnumbers for each possible pair
        public static List<CardNumber> GetAllPossiblePairCards() => new List<CardNumber>{
            CardNumber.Ace,
            CardNumber.Two,
            CardNumber.Three,
            CardNumber.Four,
            CardNumber.Five,
            CardNumber.Six,
            CardNumber.Seven,
            CardNumber.Eight,
            CardNumber.Nine,
            CardNumber.Ten
        };

        ///<summary>
        /// Returns a list of all the cards minus cards that have a value of 10 as well as A. 
        /// A + 10 is blackjack so that's not included in the soft total tree.
        /// A + A is not there either because that will just use the hard total.
        ///</summary>
        public static List<int> GetAllPossibleSoftTotalValues()=>new List<int>{ 2,3,4,5,6,7,8,9 };
        public static List<int> GetAllPossibleHardTotalValues() => new List<int>{ 3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20};
    }
}
