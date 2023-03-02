using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    public class DealerCard {
        // maybe just use the regular Card class, not planning to add any more data here
    }

    public class Results {
        public long NumberOfWins { get; set; }
        public long NumberOfLosses { get; set; }
        public long NumberOfPushes { get; set; }
    }

    // nodes for the tree
    public class StrategyBuilderRootNode {
        // the root node will store a list of nodes for the dealer's card that is visible
        protected List<DealerVisibleCardNode> DealerVisibleCardNodes { get; init; } = new List<DealerVisibleCardNode>();

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
            CardNumber = CardNumber;
        }
        public CardNumber CardNumber { get; init; }
        public List<PlayerSecondCardNode> SecondCards { get; init; } = new List<PlayerSecondCardNode>();

        public PlayerSecondCardNode GetAddIfNeeded(CardNumber secondCardNumber) {
            foreach (var node in SecondCards) {
                if (node.CardNumber == secondCardNumber) {
                    return node;
                }
            }
            // create, add and return
            PlayerSecondCardNode addedItem = new PlayerSecondCardNode(secondCardNumber);
            SecondCards.Add(addedItem);
            return addedItem;
        }
    }
    public class PlayerSecondCardNode {
        public PlayerSecondCardNode(CardNumber cardNumber) {
            CardNumber = cardNumber;
        }
        public CardNumber CardNumber { get; init; }
        public Results Results { get; init; } = new Results();
    }

    public class ResultTree {
        public void RecordResult(CardNumber dealerVisibleCard, CardNumber opponentCard1, CardNumber opponentCard2, HandResult handResult) {
            var dealerNode = RootNode.GetAddIfNeeded(dealerVisibleCard);
            (CardNumber firstCardNumber, CardNumber secondCardNumber) = (opponentCard1, opponentCard2) switch {
                { opponentCard1: var oc1,opponentCard2: var oc2} when oc1 > oc2 => (oc1,oc2), 
                { opponentCard1: var oc1, opponentCard2: var oc2 } when oc1 < oc2 => (oc1, oc2), 
                { opponentCard1: var oc1, opponentCard2: var oc2 } when oc1 == oc2 => (oc1, oc2),
                (_,_) => throw new ApplicationException($"Unable to compare these CardNumbers: '{opponentCard1}','{opponentCard2}'")
            };

            var dealerCardNode = RootNode.GetAddIfNeeded(dealerVisibleCard);
            var firstCardNode = dealerCardNode.GetAddIfNeeded(firstCardNumber);
            var secondCardNode = firstCardNode.GetAddIfNeeded(secondCardNumber);
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

            // TODO: Need to figure out a way to track all the result summaries
        }
        public List<ResultSummary> GetAllResults() {
            throw new NotImplementedException();
        }
        protected StrategyBuilderRootNode RootNode { get; init; } = new StrategyBuilderRootNode();

    }
    public class ResultSummary {
        public ResultSummary(Card dealerVisibleCard, Card opponentCard1, Card opponentCard2, Results results) {
            DealerVisibleCard = dealerVisibleCard;
            OpponentCard1 = opponentCard1;
            OpponentCard2 = opponentCard2;
            Results = results;
        }

        public Card DealerVisibleCard { get; set; }
        public Card OpponentCard1 { get; set; }
        public Card OpponentCard2 { get; set; }
        public Results Results { get; set; }
    }
}
