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
        private ILogger _logger = new NullLogger();
        public Hand(float bet, ILogger logger) {
            Debug.Assert(bet >= 0);
            Bet = bet;
            _logger = logger ?? new NullLogger();
        }
        //public Hand(List<Card> cards) {
        //    Debug.Assert(cards != null);
        //    if (cards != null && cards.Count > 0) {
        //        foreach (var card in cards) {
        //            ReceiveCard(card);
        //        }
        //    }
        //}

        // TODO: Still not sure if this property is needed, let's see.
        public HandStatus Status { get; protected set; } = HandStatus.InPlay;
        public HandResult HandResult { get; protected set; } = HandResult.InPlay;

        public float Bet { get; set; }

        private List<Card> _dealtCards = new List<Card>();
        internal List<Card> DealtCards {
            get {
                return _dealtCards;
            }
            set {
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
        public void SetHandResult(HandResult result) {
            HandResult = result;
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
            foreach (var card in cardsWithMultipleValues) {
                // assume that the high value is first in the array
                var values = card.Number.GetValues();
                Debug.Assert(values.Length == 2);
                Debug.Assert(values[0] > values[1]);

                if (sumSingleValueCards + values[0] <= BlackjackSettings.GetBlackjackSettings().MaxScore) {
                    sumSingleValueCards += values[0];
                }
                else {
                    sumSingleValueCards += values[1];
                }
            }

            return sumSingleValueCards;
        }

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
