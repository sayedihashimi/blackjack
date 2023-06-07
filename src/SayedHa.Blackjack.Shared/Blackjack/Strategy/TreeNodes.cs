using System;
using System.Collections.Generic;
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
}
