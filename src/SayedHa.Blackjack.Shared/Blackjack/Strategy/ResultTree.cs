using SayedHa.Blackjack.Shared.Blackjack.Exceptions;
using System.Diagnostics;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    public class ResultTree {
        /// <summary>
        /// This isn't really needed, it's just here for unit testing.
        /// This may be removed later.
        /// </summary>
        protected internal int NumSecondCardNodesCreated { get; set; } = 0;

        private List<ResultSummary> _allResults = new List<ResultSummary>();

        public List<ResultSummary> GetAllResults() => _allResults;
        protected internal StrategyBuilderRootNode RootNode { get; init; } = new StrategyBuilderRootNode();

        /// <summary>
        /// Records the result with the tree.
        /// If the combo of dealer card and opponent cards hasn't been added to the tree it will be,
        /// as well as creating a new result summary to track the results.
        /// A win is recorded when the sum of hands.BetResult > 0
        /// A loss is recorded when the sum of hands.BetResult < 0
        /// A push is recorded when the sum of hands.BetResult == 0
        /// </summary>
        /// <param name="hands">This is a list because hands can be split.</param>
        /// <exception cref="ApplicationException"></exception>
        public void RecordResult(CardNumber dealerVisibleCard, CardNumber opponentCard1, CardNumber opponentCard2, List<Hand>hands) {
            Debug.Assert(hands != null && hands.Count > 0);

            var dealerNode = RootNode.GetAddIfNeeded(dealerVisibleCard);

            (CardNumber firstCardNumber, CardNumber secondCardNumber) = opponentCard1.Sort(opponentCard2);

            var dealerCardNode = RootNode.GetAddIfNeeded(dealerVisibleCard);
            var firstCardNode = dealerCardNode.GetAddIfNeeded(firstCardNumber);
            (var secondCardNode, var newResultAdded) = firstCardNode.GetAddIfNeeded(secondCardNumber);

            if(newResultAdded) {
                NumSecondCardNodesCreated++;
            }

            float totalGainOrLoss = 0F;
            foreach (var hand in hands) {
                if(hand.BetResult is null) {
                    throw new BetResultException($"BetResult is empty");
                }
                totalGainOrLoss += hand.BetResult.Value;
            }

            var handResult = HandResult.Push;
            if(totalGainOrLoss > 0F) {
                handResult = HandResult.OpponentWon;
            }
            else if(totalGainOrLoss < 0F) {
                handResult = HandResult.DealerWon;
            }

            switch(handResult) {
                case HandResult.OpponentWon:
                    secondCardNode.Results.NumberOfWins++;
                    break;
                case HandResult.DealerWon:
                    secondCardNode.Results.NumberOfLosses++;
                    break;
                case HandResult.Push:
                    secondCardNode.Results.NumberOfPushes++;
                    break;
                default:
                    throw new ApplicationException($"Unknown value for HandResult: '{handResult}'");
            }

            if (newResultAdded) {
                var rs = new ResultSummary(dealerVisibleCard, firstCardNumber, secondCardNumber, secondCardNode.Results, totalGainOrLoss);
                _allResults.Add(rs);
                secondCardNode.ResultSummary = rs;
            }
            else {
                // update the totalGainOrLoss
                secondCardNode.ResultSummary!.TotalGainOrLoss += totalGainOrLoss;
            }
        }
    }
    public class Results {
        public long NumberOfWins { get; set; }
        public long NumberOfLosses { get; set; }
        public long NumberOfPushes { get; set; }
    }
    public class ResultSummary {
        public ResultSummary(CardNumber dealerVisibleCard, CardNumber opponentCard1, CardNumber opponentCard2, Results results, float totalGainOrLoss) {
            DealerVisibleCard = dealerVisibleCard;
            OpponentCard1 = opponentCard1;
            OpponentCard2 = opponentCard2;
            Results = results;
            TotalGainOrLoss = totalGainOrLoss;
        }

        public CardNumber DealerVisibleCard { get; set; }
        public CardNumber OpponentCard1 { get; set; }
        public CardNumber OpponentCard2 { get; set; }
        public Results Results { get; set; }
        public float TotalGainOrLoss { get; set; } = 0F;
    }
}
