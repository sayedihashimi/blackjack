using SayedHa.Blackjack.Shared.Blackjack.Exceptions;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree {
    public interface IBaseTreeNode<T, J> {
        public List<ITreeNode<T, J>>? Children { get; init; }

        public (bool added, ITreeNode<T, J> node) GetOrAdd(T id, NodeType nodeType);

        public ITreeNode<T, J>? Get(T id);

        public ITreeNode<T, J> AddItem(T id);
        public ITreeNode<T, J> AddItem(T id, NodeType nodeType);
    }
    public class BaseTreeNode<T, J> : IBaseTreeNode<T, J> {
        public List<ITreeNode<T, J>>? Children { get; init; } = new List<ITreeNode<T, J>>();
        public (bool, ITreeNode<T, J>) GetOrAdd(T id, NodeType nodeType) {
            if (Children == null) {
                throw new TreeChildrenNullException();
            }

            var foundNode = Get(id);
            if (foundNode is object) {
                return (false, foundNode);
            }

            return (true, AddItem(id, nodeType));
        }
        /// <summary>
        /// Will get the node specified.
        /// null is returned if it's not found.
        /// </summary>
        public ITreeNode<T, J>? Get(T id) {
            if (Children == null) {
                throw new TreeChildrenNullException($"id: '{id}'");
            }
            foreach (var node in Children) {
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
    public interface ITreeNode<T, J> : IBaseTreeNode<T, J> {
        public T Id { get; init; }
    }
    public class TreeNode<T, J> : BaseTreeNode<T, J>, ITreeNode<T, J> {
        public TreeNode(T id) : base() {
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
    public class LeafNode<T, J> : TreeNode<T, J> {
        public LeafNode(T id) : base(id) {
            Children = null;
        }

        public J? Value {
            get;
            set;
        }
    }
    public enum NodeType {
        TreeNode,
        LeafNode
    }
}