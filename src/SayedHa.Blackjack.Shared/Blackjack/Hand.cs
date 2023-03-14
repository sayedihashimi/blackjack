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
using System.Diagnostics;
using System.Text;

namespace SayedHa.Blackjack.Shared {
    public class DealerHand : Hand {
        public DealerHand(ILogger logger) : base(0, logger) {

        }

        public Card? DealersVisibleCard {
            get => DealtCards != null && DealtCards.Count >= 2 ? DealtCards[1] : null;
        }
        public Card? DealersHiddenCard {
            get => DealtCards != null && DealtCards.Count >= 1 ? DealtCards[0] : null;
        }
    }
    public class Hand {
        private ILogger _logger = NullLogger.Instance;
        public Hand(float bet, ILogger logger, float? betResult = null) {
            Debug.Assert(bet >= 0);
            Bet = bet;
            _logger = logger ?? NullLogger.Instance;
            if(betResult is not null) {
                BetResult = betResult;
            }
        }

        // TODO: Still not sure if this property is needed, let's see.
        public HandStatus Status { get; protected internal set; } = HandStatus.InPlay;
        public HandResult HandResult { get; protected internal set; } = HandResult.InPlay;

        public float Bet { get; set; }
        /// <summary>
        /// The amount won or lost in this hand. Negative for amounts paid to the dealer
        /// and positive amounts for hands that have won.
        /// </summary>
        public float? BetResult { get; set; }

        private List<Card> _dealtCards = new List<Card>();
        public List<Card> DealtCards {
            get {
                return _dealtCards;
            }
            internal set {
                _dealtCards = value;
                _scoreCached = ComputeScore();
            }
        }
        protected int _scoreCached;

        /// <summary>
        /// Adds the card to the DealtCards
        /// </summary>
        /// <returns>Returns the card that was passed in</returns>
        public Card ReceiveCard(Card card) {
            Debug.Assert(card != null);
            // _logger.LogLine($"  card: {card.ToString()}");
            DealtCards.Add(card);
            _scoreCached = ComputeScore();

            return card;
        }

        public int GetScore() {
            return _scoreCached;
        }

        /// <summary>
        /// Call this mehtod at the end of the game to indicate if the hand was a win/loss
        /// </summary>
        /// <param name="result"></param>
        public void SetHandResult(HandResult result, float betResult) {
            HandResult = result;
            BetResult = betResult;
        }
        /// <summary>
        /// Call this method when no other actions can be taken on the Hand.
        /// The most common case that this is used is when a hand has been completed
        /// and the dealer moves on to play the next hand.
        /// For example, after a double down is executed, this should be called
        /// to prevent any other actions on this hand.
        /// </summary>
        public void MarkHandAsClosed() {
            Status = HandStatus.Closed;
        }

        /// <summary>
        /// Returns the score for the DealtCards
        /// Note: this assumes that only the Ace card has multiple values, if that ever changes
        /// this method must be updated.
        /// </summary>
        /// <returns>numeric score of DealtCards</returns>
        public int ComputeScore() {
            if(DealtCards == null || DealtCards.Count == 0) return 0;

            // have to handle the Ace case where it has more than one value
            var cardsWithMultipleValues = new List<Card>();
            var cardsWithSingleValue = new List<Card>();
            foreach (var card in DealtCards) {
                if(card.Number.GetValues().Length > 1) {
                    cardsWithMultipleValues.Add(card);
                }
                else {
                    cardsWithSingleValue.Add(card);
                }
            }

            // calculate all scores and return the best value
            var sumSingleValueCards = cardsWithSingleValue.Sum(card => card.Number.GetValues()[0]);

            var bestScore = sumSingleValueCards;
            // TODO: Compute all possible scores and then return the best score.
            var allScores = ComputeScoreForCardsWithMultipleValues(bestScore,cardsWithMultipleValues);
            if(allScores.Count == 1) {
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

        private List<int> ComputeScoreForCardsWithMultipleValues(int currentScore, List<Card> CardsWithMultipleValues) {
            Debug.Assert(CardsWithMultipleValues != null);

            var scoreList = new List<int>();
            if(CardsWithMultipleValues?.Count == 0) {
                return new List<int> { currentScore };
            }

            scoreList = ComputeScoreForCardsWithMultipleValues(CardsWithMultipleValues!, currentScore, new List<int>());

            return scoreList;
        }

        // TODO: This can be improved to only return the new scores in the recursion, with the current
        // implementation it adds a lot dupes to the list because previous scores are added a bunch.
        private List<int> ComputeScoreForCardsWithMultipleValues(List<Card> CardsWithMultipleValues, int initialScore, List<int> previousScores) {
            if(CardsWithMultipleValues?.Count == 0) {
                return previousScores;
            }
            if(CardsWithMultipleValues.Count == 1) {
                var newScores = new List<int>();
                newScores.AddRange(previousScores);
                foreach (var value in CardsWithMultipleValues[0].Number.GetValues()) {
                    newScores.Add(initialScore + value);
                }

                return newScores;
            }
            var newScores1 = new List<int>();
            newScores1.AddRange(previousScores);
            // now we need to take off the first card and recurse to get the value
            foreach(var value in CardsWithMultipleValues[0].Number.GetValues()) {
                // iterate through the other cards to get the score of the remaining list
                var remainingCards = new List<Card>();
                for (int i = 1; i < CardsWithMultipleValues.Count; i++) {
                    remainingCards.Add(CardsWithMultipleValues[i]);
                }
                newScores1.AddRange(ComputeScoreForCardsWithMultipleValues(remainingCards, initialScore + value, newScores1));
            }

            var scoresNoDupes = new List<int>();
            foreach(var score in newScores1) {
                if (!scoresNoDupes.Contains(score)) {
                    scoresNoDupes.Add(score);
                }
            }

            return scoresNoDupes;
        }

        public IList<HandAction> GetValidActions(int dollarsRemaining) {
            var actions = (this.Status, this.GetScore(), this.DealtCards.Count) switch {
                (HandStatus.Closed, _, _) => new List<HandAction>() { HandAction.Stand },
                (_, >= 21, _) => new List<HandAction>() { HandAction.Stand },
                (HandStatus.InPlay, < 21, <= 2) => new List<HandAction> { HandAction.Stand, HandAction.Hit, HandAction.Double, HandAction.Split },
                (HandStatus.InPlay, < 21, > 2) => new List<HandAction> { HandAction.Stand, HandAction.Hit },
                _ => throw new NotImplementedException()
            };

            // filter out the double down or split if the DollarsRemaining don't allow it
            if (dollarsRemaining < (int)Math.Floor(Bet)*2 &&
                actions.Contains(HandAction.Double)) {
                actions.Remove(HandAction.Double);
            }
            if (dollarsRemaining < (int)Math.Floor(Bet) * 2 &&
                actions.Contains(HandAction.Split)) {
                actions.Remove(HandAction.Split);
            }
            if(actions.Contains(HandAction.Split) && 
                (DealtCards.Count != 2 || DealtCards[0].Number != DealtCards[1].Number)) {
                actions.Remove(HandAction.Split);
            }

            return actions;
        }

        public bool DoesHandHaveBlackjack() => (DealtCards[0].Number, DealtCards[1].Number) switch {
            (CardNumber.Ace, CardNumber.Ten) => true,
            (CardNumber.Ace, CardNumber.Jack) => true,
            (CardNumber.Ace, CardNumber.Queen) => true,
            (CardNumber.Ace, CardNumber.King) => true,
            (CardNumber.Ten, CardNumber.Ace) => true,
            (CardNumber.Jack, CardNumber.Ace) => true,
            (CardNumber.Queen, CardNumber.Ace) => true,
            (CardNumber.King, CardNumber.Ace) => true,
            _ => false
        };
        /// <summary>
        /// For the dealer to have blackjack the visible card must be an ace
        /// </summary>
        /// <returns></returns>
        public bool DoesDealerHaveBlackjack() => (DealtCards[0].Number, DealtCards[1].Number) switch {
            (CardNumber.Ten, CardNumber.Ace) => true,
            (CardNumber.Jack, CardNumber.Ace) => true,
            (CardNumber.Queen, CardNumber.Ace) => true,
            (CardNumber.King, CardNumber.Ace) => true,
            _ => false
        };
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("[");
            for(var i = 0; i < DealtCards.Count; i++) {
                sb.Append(DealtCards[i]);
                if (i < DealtCards.Count - 1) {
                    sb.Append(",");
                }
            }
            sb.Append(']');
            sb.Append($" Score={GetScore()}. Result={HandResult}");
            
            return sb.ToString().Trim();
        }
        public string ToString(bool hideFirstCard=true, bool useSymbols=true,bool includeScore = false,bool includeBrackets = false, bool includeResult = false) {
            var sb = new StringBuilder();
            if (includeBrackets) {
                sb.Append("[");
            }            
            for (var i = 0; i < DealtCards.Count; i++) {
                if(hideFirstCard && i == 0) {
                    sb.Append("??");
                }
                else {
                    sb.Append(DealtCards[i].ToString(useSymbols));
                }
                
                if (i < DealtCards.Count - 1) {
                    sb.Append(",");
                }
            }
            if (includeBrackets) {
                sb.Append(']');
            }
            if (includeScore) {
                if (hideFirstCard) {
                    sb.Append($" Score=??");
                }
                else {
                    sb.Append($" Score={GetScore()}");
                }
            }
            if (includeResult) {
                sb.Append($" Result={HandResult}");
            }

            return sb.ToString().Trim();
        }
    }
    public enum HandResult {
        InPlay,
        DealerWon,
        OpponentWon,
        Push
    }
    public enum HandStatus {
        InPlay,
        // Closed hand is one that cannot take any more actions. For example after a Double down has been executed,
        // no more actions can be taken on that hand.
        // It doesn't mean that it's a lost hand.
        Closed
    }
}
