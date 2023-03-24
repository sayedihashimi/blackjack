using SayedHa.Blackjack.Shared.Blackjack.Exceptions;
using System.Security.Cryptography;
using System;
using System.Diagnostics;

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
        public static int GetNumericScore(CardNumber card) => card switch {
            CardNumber.Ace => 11,
            CardNumber.Two => 2,
            CardNumber.Three => 3,
            CardNumber.Four => 4,
            CardNumber.Five => 5,
            CardNumber.Six => 6,
            CardNumber.Seven => 7,
            CardNumber.Eight => 8,
            CardNumber.Nine => 9,
            CardNumber.Ten => 10,
            CardNumber.Jack => 10,
            CardNumber.Queen => 10,
            CardNumber.King => 10,
            _ => throw new UnexpectedValueException($"Unknown value for CardNumber: '{card}'")
        };
        public static int GetNumericScore(CardNumberOrScore card) => card switch {
            CardNumberOrScore.Ace => 11,
            CardNumberOrScore.Two => 2,
            CardNumberOrScore.Three => 3,
            CardNumberOrScore.Four => 4,
            CardNumberOrScore.Five => 5,
            CardNumberOrScore.Six => 6,
            CardNumberOrScore.Seven => 7,
            CardNumberOrScore.Eight => 8,
            CardNumberOrScore.Nine => 9,
            CardNumberOrScore.Ten => 10,
            CardNumberOrScore.Jack => 10,
            CardNumberOrScore.Queen => 10,
            CardNumberOrScore.King => 10,
            CardNumberOrScore.Score21 => 21,
            CardNumberOrScore.Score20 => 20,
            CardNumberOrScore.Score19 => 19,
            CardNumberOrScore.Score18 => 18,
            CardNumberOrScore.Score17 => 17,
            CardNumberOrScore.Score16 => 16,
            CardNumberOrScore.Score15 => 15,
            CardNumberOrScore.Score14 => 14,
            CardNumberOrScore.Score13 => 13,
            CardNumberOrScore.Score12 => 12,
            CardNumberOrScore.Score11 => 11,
            CardNumberOrScore.Score10 => 10,
            CardNumberOrScore.Score9 => 9,
            CardNumberOrScore.Score8 => 8,
            CardNumberOrScore.Score7 => 7,
            CardNumberOrScore.Score6 => 6,
            CardNumberOrScore.Score5 => 5,
            CardNumberOrScore.Score4 => 4,
            CardNumberOrScore.Score3 => 3,
            CardNumberOrScore.Score2 => 2,
            CardNumberOrScore.Busted => throw new UnexpectedValueException($"Cannot get the value of a busted CardNumberOrScore"),
            _ => throw new UnexpectedValueException($"Unknown value for CardNumberOrScore: '{card}'"),
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
        public static int ComputeScore(CardNumber card1, CardNumber card2) => ComputeScore(new[] { card1, card2 });
        public static int ComputeScore(CardNumber[]dealtCards) {
            if (dealtCards == null || dealtCards.Length == 0) return 0;

            // have to handle the Ace case where it has more than one value
            var cardsWithMultipleValues = new List<CardNumber>();
            var cardsWithSingleValue = new List<CardNumber>();
            foreach (var card in dealtCards) {
                if (card.GetValues().Length > 1) {
                    cardsWithMultipleValues.Add(card);
                }
                else {
                    cardsWithSingleValue.Add(card);
                }
            }

            // calculate all scores and return the best value
            var sumSingleValueCards = cardsWithSingleValue.Sum(card => card.GetValues()[0]);

            var bestScore = sumSingleValueCards;
            // TODO: Compute all possible scores and then return the best score.
            var allScores = ComputeScoreForCardsWithMultipleValues(bestScore, cardsWithMultipleValues);
            if (allScores.Count == 1) {
                return allScores[0];
            }
            else {
                allScores = allScores.OrderByDescending(x => x).ToList();

                foreach (var score in allScores) {
                    bestScore = score;
                    if (score <= BlackjackSettings.GetBlackjackSettings().MaxScore) {
                        break;
                    }
                }
            }
            return bestScore;
        }

        private static List<int> ComputeScoreForCardsWithMultipleValues(int currentScore, List<CardNumber> CardsWithMultipleValues) {
            Debug.Assert(CardsWithMultipleValues != null);

            var scoreList = new List<int>();
            if (CardsWithMultipleValues?.Count == 0) {
                return new List<int> { currentScore };
            }

            scoreList = ComputeScoreForCardsWithMultipleValues(CardsWithMultipleValues!, currentScore, new List<int>());

            return scoreList;
        }
        private static List<int> ComputeScoreForCardsWithMultipleValues(List<CardNumber> CardsWithMultipleValues, int initialScore, List<int> previousScores) {
            if (CardsWithMultipleValues?.Count == 0) {
                return previousScores;
            }
            if (CardsWithMultipleValues.Count == 1) {
                var newScores = new List<int>();
                newScores.AddRange(previousScores);
                foreach (var value in CardsWithMultipleValues[0].GetValues()) {
                    newScores.Add(initialScore + value);
                }

                return newScores;
            }
            var newScores1 = new List<int>();
            newScores1.AddRange(previousScores);
            // now we need to take off the first card and recurse to get the value
            foreach (var value in CardsWithMultipleValues[0].GetValues()) {
                // iterate through the other cards to get the score of the remaining list
                var remainingCards = new List<CardNumber>();
                for (int i = 1; i < CardsWithMultipleValues.Count; i++) {
                    remainingCards.Add(CardsWithMultipleValues[i]);
                }
                newScores1.AddRange(ComputeScoreForCardsWithMultipleValues(remainingCards, initialScore + value, newScores1));
            }

            var scoresNoDupes = new List<int>();
            foreach (var score in newScores1) {
                if (!scoresNoDupes.Contains(score)) {
                    scoresNoDupes.Add(score);
                }
            }

            return scoresNoDupes;
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

        public static HandAction GetRandomHandAction(bool includeDouble, HandAction[] _allHandActions) {
            int fromInclusive = 0;
            int toInclusive = includeDouble ? 2 : 1;
            int indexToReturn = GetRandomIntBetween(fromInclusive, toInclusive + 1);
            return _allHandActions[indexToReturn];
        }
        internal static Random random { get; set; } = new Random();
        public static bool _useRandomNumberGenerator { get; set; } = true;
        public static int GetRandomIntBetween(int fromInclusive, int toExclusive) => _useRandomNumberGenerator switch {
            true => RandomNumberGenerator.GetInt32(fromInclusive, toExclusive),
            false => random.Next(fromInclusive, toExclusive)
        };
    }
}
