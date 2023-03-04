using SayedHa.Blackjack.Shared.Blackjack.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    // nodes for the tree
    public class StrategyBuilderRootNode {
        // the root node will store a list of nodes for the dealer's card that is visible
        protected internal List<DealerVisibleCardNode> DealerVisibleCardNodes { get; init; } = new List<DealerVisibleCardNode>();

        public DealerVisibleCardNode GetAddIfNeeded(CardNumber cardNumber) {
            foreach(var node in  DealerVisibleCardNodes) {
                if(node.DealerVisibleCard == cardNumber) {
                    return node;
                }
            }

            // create, add and return
            var addedItem = new DealerVisibleCardNode(cardNumber);
            DealerVisibleCardNodes.Add(addedItem);
            return addedItem;
        }
    }

    public class DealerVisibleCardNode {
        public DealerVisibleCardNode(CardNumber dealerVisibleCard) {
            DealerVisibleCard = dealerVisibleCard;
        }
        public CardNumber DealerVisibleCard { get; init; }
        public List<PlayerFirstCardNode> PlayerFirstCards { get; init; } = new List<PlayerFirstCardNode>();

        public PlayerFirstCardNode GetAddIfNeeded(CardNumber firstCardNumber) {
            foreach (var node in PlayerFirstCards) {
                if (node.CardNumber == firstCardNumber) {
                    return node;
                }
            }
            // create, add and return
            PlayerFirstCardNode addedItem = new PlayerFirstCardNode(firstCardNumber);
            PlayerFirstCards.Add(addedItem);
            return addedItem;
        }
    }

    public class PlayerFirstCardNode {
        public PlayerFirstCardNode(CardNumber cardNumber) {
            CardNumber = cardNumber;
        }
        public CardNumber CardNumber { get; init; }
        public List<PlayerSecondCardNode> SecondCards { get; init; } = new List<PlayerSecondCardNode>();

        public (PlayerSecondCardNode node,bool added) GetAddIfNeeded(CardNumber secondCardNumber) {
            foreach (var node in SecondCards) {
                if (node.CardNumber == secondCardNumber) {
                    return (node, false);
                }
            }
            // create, add and return
            PlayerSecondCardNode addedItem = new PlayerSecondCardNode(secondCardNumber);
            SecondCards.Add(addedItem);
            return (addedItem, true);
        }
    }
    public class PlayerSecondCardNode {
        public PlayerSecondCardNode(CardNumber cardNumber) {
            CardNumber = cardNumber;
        }
        public CardNumber CardNumber { get; init; }
        public Results Results { get; init; } = new Results();
        public ResultSummary? ResultSummary { get; set; }
    }

    public class ResultTree {
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
            (CardNumber firstCardNumber, CardNumber secondCardNumber) = (opponentCard1, opponentCard2) switch {
                { opponentCard1: var oc1, opponentCard2: var oc2} when oc1 > oc2 => (oc1, oc2), 
                { opponentCard1: var oc1, opponentCard2: var oc2 } when oc1 < oc2 => (oc2, oc1), 
                { opponentCard1: var oc1, opponentCard2: var oc2 } when oc1 == oc2 => (oc1, oc2),
                (_,_) => throw new ApplicationException($"Unable to compare these CardNumbers: '{opponentCard1}','{opponentCard2}'")
            };

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

        /// <summary>
        /// This isn't really needed, it's just here for unit testing.
        /// This may be removed later.
        /// </summary>
        protected internal int NumSecondCardNodesCreated { get; set; } = 0;

        private List<ResultSummary> _allResults = new List<ResultSummary>();

        public List<ResultSummary> GetAllResults() => _allResults;
        protected internal StrategyBuilderRootNode RootNode { get; init; } = new StrategyBuilderRootNode();
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
