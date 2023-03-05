using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    /// <summary>
    /// Contains the moves for each combo of dealer card and opponent cards.
    /// These are bucketed into three categories
    ///  - Pairs
    ///  - Hands with an Ace
    ///  - Score of both opponent cards
    /// The order above also dictates the priority of the bucket match, from top to bottom.
    /// The first match that is found will be used. So if the opponent cards are Ace Ace
    /// it's a pair and not in the Hands with an Ace bucket.
    /// </summary>
    public class StrategyTable {

        public void SetNextAction(CardNumber dealerCard, CardNumber opponentCard1, CardNumber opponentCard2, HandAction nextAction) {
            if (IsAPair(opponentCard1, opponentCard2)) {
                Dictionary<CardNumber, HandAction> existingEntry;
                PairsHandActionMap.TryGetValue(opponentCard1, out existingEntry);

                if(existingEntry is null) {
                    // create and add to the dictionary
                    existingEntry = new Dictionary<CardNumber, HandAction>();
                    PairsHandActionMap.Add(opponentCard1, existingEntry);
                }

                // add the new hand action
                existingEntry.Add(dealerCard, nextAction);
                PairsHandActionMap.Add(opponentCard1, existingEntry);
            }
            else if(IsAceHand(opponentCard1, opponentCard2)) {
                Dictionary<CardNumber, HandAction> existingEntry;
                var sortedCards = opponentCard1.Sort(opponentCard2);

                (var nonAceCard, _) = opponentCard1.Sort(opponentCard2);
                HandsWithAceActionMap.TryGetValue(nonAceCard, out existingEntry);

                if(existingEntry is null) {
                    // create and add
                    existingEntry = new Dictionary<CardNumber, HandAction>();
                    HandsWithAceActionMap.Add(nonAceCard, existingEntry);
                }

                existingEntry.Add(nonAceCard, nextAction);
                HandsWithAceActionMap.Add(nonAceCard, existingEntry);
            }
            else {
                Dictionary<CardNumber, HandAction> existingEntry;
                var score = opponentCard1.GetValues()[0] + opponentCard2.GetValues()[0];
                ScoreHandActionMap.TryGetValue(score, out existingEntry);

                if(existingEntry is null) {
                    existingEntry= new Dictionary<CardNumber, HandAction>();
                }

                existingEntry.Add(dealerCard, nextAction);
                ScoreHandActionMap.Add(score, existingEntry);
            }
        }
        public HandActionAndReason GetNextAction(CardNumber dealerCard, CardNumber opponentCard1, CardNumber opponentCard2) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// They key is the CardNumber for the pair
        /// The value contains a dictionary where the key is the dealer's visible card
        /// and the value is the next HandAction.
        /// </summary>
        protected Dictionary<CardNumber, Dictionary<CardNumber, HandAction>> PairsHandActionMap { get; set; } = new Dictionary<CardNumber, Dictionary<CardNumber, HandAction>>();
        /// <summary>
        /// The key is the opponents card which is not an Ace.
        /// The value contains a dictionary where the key is the dealer's visible card
        /// and the value is the next HandAction.
        /// </summary>
        protected Dictionary<CardNumber, Dictionary<CardNumber, HandAction>> HandsWithAceActionMap { get; set; } = new Dictionary<CardNumber, Dictionary<CardNumber, HandAction>>();
        /// <summary>
        /// The key is the score of both opponents cards.
        /// The value contains a dictionary where the key is the dealer's visible card
        /// and the value is the next HandAction.
        /// </summary>
        protected Dictionary<int, Dictionary<CardNumber, HandAction>> ScoreHandActionMap { get; set; } = new Dictionary<int, Dictionary<CardNumber, HandAction>>();

        protected bool IsAPair(CardNumber card1, CardNumber card2) => card1 == card2;
        protected bool IsAceHand(CardNumber card1, CardNumber card2) => (card1, card2) switch {
            (CardNumber.Ace, _) => true,
            (_, CardNumber.Ace) => true,
        };
        // TODO: need to be able to read the values from a csv file to populate the next action
    }
}
