using System.Diagnostics;

namespace SayedHa.Blackjack.Shared {
    public class Participant {
        public ParticipantRole Role { get; set; } = ParticipantRole.Player;
        public Hand Hand { get; set; } = new Hand();
    }

    public enum ParticipantRole {
        Dealer,
        Player
    }

    public class Hand {
        public Hand() { }
        public Hand(List<Card> cards) {
            Debug.Assert(cards != null);
            if (cards != null && cards.Count > 0) {
                foreach (var card in cards) {
                    ReceiveCard(card);
                }
            }
        }
                        
        internal List<Card> DealtCards { get; set; }=new List<Card>();

        public void ReceiveCard(Card card) {
            Debug.Assert(card != null);
            DealtCards.Add(card);
        }

        /// <summary>
        /// Returns the score for the DealtCards
        /// Note: this assumes that only the Ace card has multiple values, if that ever changes
        /// this method must be updated.
        /// </summary>
        /// <returns>numeric score of DealtCards</returns>
        public int GetScore() {
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
    }
}
