using System.Diagnostics;
using System.Text;

namespace SayedHa.Blackjack.Shared {
    public class Hand {
        private ILogger _logger = new Logger();
        public Hand() { }
        public Hand(List<Card> cards) {
            Debug.Assert(cards != null);
            if (cards != null && cards.Count > 0) {
                foreach (var card in cards) {
                    ReceiveCard(card);
                }
            }
        }

        // TODO: Still not sure if this property is needed, let's see.
        public HandStatus Status { get; internal set; } = HandStatus.InPlay;
        public HandResult HandResult { get; internal set; } = HandResult.InPlay;

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

        public Card? DealersVisibleCard {
            get {
                if (_dealtCards != null && _dealtCards.Count > 0) {
                    return _dealtCards[0];
                }

                return null;
            }
        }

        public void ReceiveCard(Card card) {
            Debug.Assert(card != null);
            _logger.Log($"  card: {card.ToString()}");
            DealtCards.Add(card);
            _scoreCached = ComputeScore();
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

                if (sumSingleValueCards + values[0] <= KnownValues.MaxScore) {
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
            sb.Append($" Score={GetScore()}");
            
            return sb.ToString().Trim();
        }
    }
    public enum HandResult {
        InPlay,
        Win,
        Loss,
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
