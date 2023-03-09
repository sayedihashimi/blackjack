using SayedHa.Blackjack.Shared.Blackjack.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree {
    public interface IBaseTreeNode<T, J> where T : System.Enum {
        public List<ITreeNode<T, J>>? Children { get; init; }

        public (bool added,ITreeNode<T, J> node) GetOrAdd(T id, NodeType nodeType);

        public ITreeNode<T, J>? Get(T id);

        public ITreeNode<T, J> AddItem(T id);
        public ITreeNode<T, J> AddItem(T id, NodeType nodeType);
    }
    public class BaseTreeNode<T, J> : IBaseTreeNode<T, J> where T : System.Enum {
        public List<ITreeNode<T, J>>? Children { get; init; } = new List<ITreeNode<T, J>>();
        public (bool, ITreeNode<T, J>) GetOrAdd(T id, NodeType nodeType) {
            if (Children == null) {
                throw new TreeChildrenNullException();
            }

            var foundNode = Get(id);
            if(foundNode is object) {
                return (false, foundNode);
            }

            return (true, AddItem(id, nodeType));
        }
        /// <summary>
        /// Will get the node specified.
        /// null is returned if it's not found.
        /// </summary>
        public ITreeNode<T, J>? Get(T id) {
            if(Children == null) {
                throw new TreeChildrenNullException($"id: '{id}'");
            }
            foreach(var node in Children) {
                if (id.Equals(node.Id)) {
                    return node;
                }
            }

            return null;
        }
        public ITreeNode<T, J> AddItem(T id) {
            return AddItem(id, NodeType.TreeNode);
        }
        public ITreeNode<T, J> AddItem(T id, NodeType nodeType) {
            TreeNode<T, J> newNode = nodeType switch {
                NodeType.TreeNode => new TreeNode<T, J>(id),
                NodeType.LeafNode => new LeafNode<T, J>(id),
                _ => throw new UnknownValueException($"Invalid value for NodeType: '{nodeType}'")
            };

            Children!.Add(newNode);
            return newNode;
        }
    }
    public interface ITreeNode<T, J> : IBaseTreeNode<T, J> where T : System.Enum {
        public T Id { get; init; }
    }
    public class TreeNode<T, J> : BaseTreeNode<T, J>, ITreeNode<T, J> where T : System.Enum {
        public TreeNode(T id):base() {
            if (id == null) throw new ArgumentNullException("id");
            Id = id;
            Children = new List<ITreeNode<T, J>>();
        }

        public T Id { get; init; }
    }
    /// <summary>
    /// No children, the Children property will be set to null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="J"></typeparam>
    public class LeafNode<T, J> : TreeNode<T, J> where T : System.Enum {
        public LeafNode(T id) : base(id) {
            Children = null;
        }

        public J? Value { get; set; }
    }
    public enum NodeType {
        TreeNode,
        LeafNode
    }

    public class BjPlayerRootNode<T,J>:BaseTreeNode<T,J> where T : System.Enum {

    }

    public class BjPlayerResultContainer {
        public Results Results { get; set; }
        public ResultSummary? ResultSummary { get; set; }
    }

    public class NextHandActionTree {
        protected internal BaseTreeNode<CardNumber, HandAction> RootNode { get; set; } = 
            new BaseTreeNode<CardNumber, HandAction>();
        /// <summary>
        /// This isn't really needed, it's just here for unit testing.
        /// This may be removed later.
        /// </summary>
        protected internal int NumSecondCardNodesCreated { get; set; } = 0;

        public void AddNextHandActionFor(CardNumber dealerVisibleCard, CardNumber opponentCard1, CardNumber opponentCard2, HandAction handAction, bool allowOverride = true) {
            (_, var dealerCardNode) = RootNode.GetOrAdd(dealerVisibleCard, NodeType.TreeNode);
            (CardNumber firstCardNumber, CardNumber secondCardNumber) = opponentCard1.Sort(opponentCard2);
            
            (_, var firstCardNode) = dealerCardNode.GetOrAdd(firstCardNumber, NodeType.TreeNode);
            (var newSecondCardNodeAdded, var secondCardNode) = firstCardNode.GetOrAdd(secondCardNumber, NodeType.LeafNode);

            var leafNode = secondCardNode as LeafNode<CardNumber, HandAction>;
            if(leafNode is null) {
                throw new UnexpectedNodeTypeException($"Expected LeafNode but instead we have: '{secondCardNode.GetType().FullName}'");
            }

            if (allowOverride && !newSecondCardNodeAdded /*&& leafNode.Value != handAction*/) {
                // TODO: Improve this later, it shouldn't get here in most cases I believe
                Console.WriteLine($"Updating existing HandAction from '{leafNode.Value}' to '{handAction}'");
            }

            if (newSecondCardNodeAdded) {
                leafNode.Value = handAction;
                NumSecondCardNodesCreated++;
            }
            if (allowOverride && !newSecondCardNodeAdded) {
                Console.WriteLine($"Updating existing HandAction from '{leafNode.Value}' to '{handAction}'");
                leafNode.Value = handAction;
            }
            else {
                Console.WriteLine($"Preventing node value override because allowOverride is set to false. From '{leafNode.Value}' to '{handAction}'");
            }
        }
        /// <summary>
        /// Will add all cards that add up to the given score excluding hands with an Ace and pairs.
        /// If there's an existing value in the tree, it will not be overwritten.
        /// </summary>
        /// <param name="dealerVisibleCard"></param>
        /// <param name="score"></param>
        /// <param name="handAction"></param>
        public void AddNextHandActionForCardSum(CardNumber dealerVisibleCard, int score, HandAction handAction) {
            var firstAndSecondCardList = GetAllCardsThatAddUpTo(score);

            foreach((var firstCard, var secondCard) in firstAndSecondCardList) {
                AddNextHandActionFor(dealerVisibleCard, firstCard, secondCard, handAction, false);
            }
        }

        public HandAction GetNextHandActionFor(CardNumber dealerVisibleCard, CardNumber opponentCard1, CardNumber opponentCard2) {
            (_, var dealerCardNode) = RootNode.GetOrAdd(dealerVisibleCard, NodeType.TreeNode);
            (CardNumber firstCardNumber, CardNumber secondCardNumber) = opponentCard1.Sort(opponentCard2);

            (_, var firstCardNode) = dealerCardNode.GetOrAdd(firstCardNumber, NodeType.TreeNode);
            (var newResultAdded, var secondCardNode) = firstCardNode.GetOrAdd(secondCardNumber, NodeType.LeafNode);

            var leafNode = secondCardNode as LeafNode<CardNumber, HandAction>;
            if (leafNode is null) {
                throw new UnexpectedNodeTypeException($"Expected LeafNode but instead we have: '{secondCardNode.GetType().FullName}'");
            }

            return leafNode.Value;
        }

        /// <summary>
        /// Will return cards that add up to the given score, 
        /// excluding hands with an Ace and Pairs.
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        protected internal List<(CardNumber card1, CardNumber card2)> GetAllCardsThatAddUpTo(int score) {
            List<(CardNumber card1, CardNumber card2)> resultCards = new List<(CardNumber card1, CardNumber card2)>();
            var allCardNumbers = ((CardNumber[])Enum.GetValues(typeof(CardNumber))).ToArray();

            for (int i = 0; i < allCardNumbers.Length; i++) {
                var firstCard = allCardNumbers[i];
                if(firstCard == CardNumber.Ace) {
                    continue;
                }
                for(int j = 0; j < allCardNumbers.Length; j++) {
                    var secondCard = allCardNumbers[j];
                    // skip aces and pairs, those need to be added explicitly
                    if(secondCard == CardNumber.Ace || secondCard == firstCard) {
                        continue;
                    }

                    if (firstCard.GetValues()[0] + secondCard.GetValues()[0] == score) {
                        resultCards.Add((firstCard, secondCard));
                    }
                }
            }

            return resultCards;
        }
    }
}
