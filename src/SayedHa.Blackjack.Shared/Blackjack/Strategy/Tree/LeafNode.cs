using SayedHa.Blackjack.Shared.Blackjack.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree {
    public interface IBaseTreeNode<T> where T : System.Enum {
        public List<ITreeNode<T>>? Children { get; init; }

        public ITreeNode<T> GetAddIfNeeded(T id, NodeType nodeType);
    }
    public class BaseTreeNode<T, J> : IBaseTreeNode<T> where T : System.Enum {
        public List<ITreeNode<T>>? Children { get; init; }
        public ITreeNode<T> GetAddIfNeeded(T id, NodeType nodeType) {
            if (Children == null) {
                return null;
            }

            foreach (var node in Children) {
                if (id.Equals(node.Id)) {
                    return node;
                }
            }

            TreeNode<T, J> newNode = nodeType switch {
                NodeType.TreeNode => new TreeNode<T, J>(id),
                NodeType.LeafNode => new LeafNode<T, J>(id),
                _ => throw new UnknownValueException($"Invalid value for NodeType: '{nodeType}'")
            };

            Children.Add(newNode);
            return newNode;
        }
    }
    public interface ITreeNode<T> : IBaseTreeNode<T> where T : System.Enum {
        public T Id { get; init; }
    }
    public class TreeNode<T, J> : BaseTreeNode<T, J>, ITreeNode<T> where T : System.Enum {
        public TreeNode(T id):base() {
            if (id == null) throw new ArgumentNullException("id");
            Id = id;
            Children = new List<ITreeNode<T>>();
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
        public BaseTreeNode<CardNumber, HandAction> RootNode { get; set; } = 
            new BaseTreeNode<CardNumber, HandAction>();
        /// <summary>
        /// This isn't really needed, it's just here for unit testing.
        /// This may be removed later.
        /// </summary>
        protected internal int NumSecondCardNodesCreated { get; set; } = 0;


        public void AddNextHandActionFor(CardNumber dealerVisibleCard, CardNumber opponentCard1, CardNumber opponentCard2) {
            var dealerCardNode = RootNode.GetAddIfNeeded(dealerVisibleCard, NodeType.TreeNode);
            (CardNumber firstCardNumber, CardNumber secondCardNumber) = opponentCard1.Sort(opponentCard2);

            var firstCardNode = dealerCardNode.GetAddIfNeeded(firstCardNumber, NodeType.TreeNode);
            // TODO: Needs some refactoring to accomplish this
            // (var secondCardNode, var newResultAdded) = firstCardNode.GetAddIfNeeded(secondCardNumber, NodeType.LeafNode);
            throw new NotImplementedException();
        }
        public HandAction GetNextHandActionFor(CardNumber dealerVisibleCard, CardNumber opponentCard1, CardNumber opponentCard2) {
            throw new NotImplementedException();
        }
    }
}
