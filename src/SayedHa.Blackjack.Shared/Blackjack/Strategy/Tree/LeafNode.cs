using SayedHa.Blackjack.Shared.Blackjack.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public class BjPlayerRootNode<T, J> : BaseTreeNode<T, J> {

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
            if (leafNode is null) {
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

            foreach ((var firstCard, var secondCard) in firstAndSecondCardList) {
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
                if (firstCard == CardNumber.Ace) {
                    continue;
                }
                for (int j = 0; j < allCardNumbers.Length; j++) {
                    var secondCard = allCardNumbers[j];
                    // skip aces and pairs, those need to be added explicitly
                    if (secondCard == CardNumber.Ace || secondCard == firstCard) {
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

    public class BlackjackStrategyTree {
        protected internal BaseTreeNode<CardNumberOrScore, HandAction> aceTree = new BaseTreeNode<CardNumberOrScore, HandAction>();
        protected internal BaseTreeNode<CardNumberOrScore, HandAction> pairTree = new BaseTreeNode<CardNumberOrScore, HandAction>();
        protected internal BaseTreeNode<CardNumberOrScore, HandAction> hardTotalTree = new BaseTreeNode<CardNumberOrScore, HandAction>();
        /// <summary>
        /// Use this to register the next action when the first two cards dealt is a pair.
        /// </summary>
        protected internal void AddPairNextAction(CardNumber dealerCard, CardNumber pairCard, HandAction nextHandAction) {
            (_, var pairRootNode) = pairTree.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard), NodeType.TreeNode);
            (bool pairNodeCreated, var pairNode) = pairRootNode.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(pairCard), NodeType.LeafNode);

            var leafNode = pairNode as LeafNode<CardNumberOrScore, HandAction>;
            if (leafNode is null) {
                throw new UnexpectedNodeTypeException($"Expected LeafNode but instead received null or an object of type '{pairNode.GetType().FullName}'");
            }

            if (!pairNodeCreated) {
                // TODO: Improve this
                Console.WriteLine($"Over writing next hand action for pair '{pairCard}', from '{leafNode.Value}' to '{nextHandAction}'.");
            }

            leafNode.Value = nextHandAction;
        }
        /// <summary>
        /// Use this when the cards dealt have an Ace
        /// The score to pass in here is the sum of the
        /// score of the other cards besides the Ace.
        /// </summary>
        protected internal void AddSoftTotalNextAction(CardNumber dealerCard, int scoreTotal, HandAction nextHandAction) {
            (_, var aceDealerCardNode) = aceTree.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard), NodeType.TreeNode);
            (var scoreTotalNodeCreated, var scoreTotalNode) = aceDealerCardNode.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(scoreTotal), NodeType.LeafNode);

            var leafNode = scoreTotalNode as LeafNode<CardNumberOrScore, HandAction>;
            if (leafNode is null) {
                throw new UnexpectedNodeTypeException($"Expected LeafNode but instead received null or an object of type '{scoreTotalNode.GetType().FullName}'");
            }

            if (!scoreTotalNodeCreated) {
                // TODO: Improve this
                Console.WriteLine($"Over writing next hand action for score '{scoreTotal}', from '{leafNode.Value}' to '{nextHandAction}'.");
            }

            leafNode.Value = nextHandAction;
        }
        /// <summary>
        /// Hard total is when you don't have an Ace or a pair of cards.
        /// </summary>
        protected internal void AddHardTotalNextAction(CardNumber dealerCard, int scoreTotal, HandAction nextHandAction) {
            (_, var dealerCardNode) = hardTotalTree.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard), NodeType.TreeNode);
            (var scoreTotalNodeCreated, var scoreTotalNode) = dealerCardNode.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(scoreTotal), NodeType.LeafNode);

            if (!scoreTotalNodeCreated) {
                // TODO: Improve this
                Console.WriteLine($"Over writing next hand action for score '{scoreTotal}', to '{nextHandAction}'.");
            }
            var leafNode = scoreTotalNode as LeafNode<CardNumberOrScore, HandAction>;
            if (leafNode is null) {
                throw new UnexpectedNodeTypeException($"Expected LeafNode but instead received null or an object of type '{scoreTotalNode.GetType().FullName}'");
            }
            leafNode.Value = nextHandAction;
        }
        public void AddNextHandAction(CardNumber dealerCard, CardNumber opCard1, CardNumber opCard2, HandAction nextHandAction) {
            if (opCard1.IsAPairWith(opCard2)) {
                AddPairNextAction(dealerCard, opCard1, nextHandAction);
            }
            else if (opCard1.ContainsAnAce(opCard2)) {
                // After sort the Ace should be the secondCardNumber
                (CardNumber firstCardNumber, CardNumber secondCardNumber) = opCard1.Sort(opCard2);
                AddSoftTotalNextAction(dealerCard, firstCardNumber.GetValues()[0], nextHandAction);
            }
            else {
                AddHardTotalNextAction(dealerCard, CardNumberHelper.GetScoreTotal(opCard1,opCard2), nextHandAction);
            }
        }
        public void AddNextHandAction(CardNumber dealerCard, HandAction nextHandAction, params CardNumber[] opCards) {
            if(opCards.Length == 2) {
                AddNextHandAction(dealerCard, opCards[0], opCards[1], nextHandAction);
            }
            else {
                // add to the hardTotal tree
                var scoreTotal = CardNumberHelper.GetScoreTotal(opCards);
                AddHardTotalNextAction(dealerCard, scoreTotal, nextHandAction);
            }
        }
        // when there is only two cards this method should be called instead of the method using params
        // performance will be slightly better in this method.
        public HandAction GetNextHandAction(CardNumber dealerCard, CardNumber opCard1, CardNumber opCard2) {
            // 1: check if it's a pair, and return the result in the pair tree
            // 2: check if the cards include an Ace, if so return the result from the Ace tree
            // 3: return the result from the HardTotals tree
            if (opCard1.IsAPairWith(opCard2)) {
                return GetOrAddFromPairTree(dealerCard, opCard1);
            }
            else if (opCard1.ContainsAnAce(opCard2)) {
                // After sort the Ace should be the secondCardNumber
                (CardNumber firstCardNumber, CardNumber secondCardNumber) = opCard1.Sort(opCard2);
                return GetOrAddFromAceTree(dealerCard, firstCardNumber.GetValues()[0]);
            }
            else {
                return GetOrAddFromHardTotalTree(dealerCard, CardNumberHelper.GetScoreTotal(opCard1, opCard2));
            }
        }
        public HandAction GetNextHandAction(CardNumber dealerCard, params CardNumber[] opCards) {
            // need to check to see if there is an Ace or not in the opCards
            // if there is an ace, check the aceTree to see if there is a result
            // if no result there, return the result from the hardTotal tree.

            var containsAce = CardNumberHelper.ContainsAnAce(opCards);
            if(containsAce) {
                var otherCards = new CardNumber[opCards.Length - 1];

                // only want to ignore 1 ace
                bool aceCardFound = false;
                int currentIndex = 0;
                foreach (var card in opCards) {
                    if(!aceCardFound && card == CardNumber.Ace) {
                        aceCardFound = true;
                    }
                    else {
                        otherCards[currentIndex++] = card;
                    }
                }
                var scoreOtherCards = CardNumberHelper.GetScoreTotal(otherCards);
                if(scoreOtherCards <= 9) {
                    // check the aceTree for the result
                    return GetOrAddFromAceTree(dealerCard, scoreOtherCards);
                }
            }

            // return result from the hardTotal tree
            var scoreTotal = CardNumberHelper.GetScoreTotal(opCards);
            return GetOrAddFromHardTotalTree(dealerCard, scoreTotal);
        }
        protected internal HandAction GetOrAddFromPairTree(CardNumber dealerCard, CardNumber opCard1) {
            (_, var rootNode) = pairTree.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard), NodeType.TreeNode);
            (_, var secondNode) = rootNode.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(opCard1), NodeType.LeafNode);

            var leafNode = secondNode as LeafNode<CardNumberOrScore, HandAction>;
            if (leafNode is null) {
                throw new UnexpectedNodeTypeException($"Expected LeafNode but instead received null or an object of type '{secondNode.GetType().FullName}'");
            }
            return leafNode.Value;
        }
        protected internal HandAction GetOrAddFromAceTree(CardNumber dealerCard, int scoreTotalExcludingAce) {
            (_, var rootNode) = aceTree.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard), NodeType.TreeNode);
            (_, var secondNode) = rootNode.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(scoreTotalExcludingAce), NodeType.LeafNode);

            var leafNode = secondNode as LeafNode<CardNumberOrScore, HandAction>;
            if (leafNode is null) {
                throw new UnexpectedNodeTypeException($"Expected LeafNode but instead received null or an object of type '{secondNode.GetType().FullName}'");
            }
            return leafNode.Value;
        }
        protected internal HandAction GetOrAddFromHardTotalTree(CardNumber dealerCard, int scoreTotal) {
            (_, var dealerCardNode) = hardTotalTree.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard), NodeType.TreeNode);
            (_, var scoreTotalNode) = dealerCardNode.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(scoreTotal), NodeType.LeafNode);
            var leafNode = scoreTotalNode as LeafNode<CardNumberOrScore, HandAction>;
            if (leafNode is null) {
                throw new UnexpectedNodeTypeException($"Expected LeafNode but instead received null or an object of type '{scoreTotalNode.GetType().FullName}'");
            }
            return leafNode.Value;
        }
    }
    //public interface ITreeNodeId<T> {
    //    public T Id { get; set; }
    //}
    //public class CardNumberTreeNodeId:ITreeNodeId<CardNumber> {
    //    public CardNumber Id { get; set; }
    //}
    //public class IntTreeNodeId : ITreeNodeId<int> {
    //    public int Id { get; set; }
    //}
    public enum CardNumberOrScore {
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Score21,
        Score20,
        Score19,
        Score18,
        Score17,
        Score16,
        Score15,
        Score14,
        Score13,
        Score12,
        Score11,
        Score10,
        Score9,
        Score8,
        Score7,
        Score6,
        Score5,
        Score4,
        Score3,
        Score2,
        Busted
    }
    public static class CardNumberHelper {
        public static CardNumberOrScore ConvertToCardNumberOrScore(CardNumber cardNumber) => cardNumber switch {
            CardNumber.Ace => CardNumberOrScore.Ace,
            CardNumber.Two => CardNumberOrScore.Two,
            CardNumber.Three => CardNumberOrScore.Three,
            CardNumber.Four => CardNumberOrScore.Four,
            CardNumber.Five => CardNumberOrScore.Five,
            CardNumber.Six => CardNumberOrScore.Six,
            CardNumber.Seven => CardNumberOrScore.Seven,
            CardNumber.Eight => CardNumberOrScore.Eight,
            CardNumber.Nine => CardNumberOrScore.Nine,
            CardNumber.Ten => CardNumberOrScore.Ten,
            CardNumber.Jack => CardNumberOrScore.Jack,
            CardNumber.Queen => CardNumberOrScore.Queen,
            CardNumber.King => CardNumberOrScore.King,
            _ => throw new UnknownValueException($"Unknown value for CardNumber: '{cardNumber}'")
        };
        public static CardNumberOrScore ConvertToCardNumberOrScore(int score) => score switch {
            > 21 => CardNumberOrScore.Busted,
            21 => CardNumberOrScore.Score21,
            20 => CardNumberOrScore.Score20,
            19 => CardNumberOrScore.Score19,
            18 => CardNumberOrScore.Score18,
            17 => CardNumberOrScore.Score17,
            16 => CardNumberOrScore.Score16,
            15 => CardNumberOrScore.Score15,
            14 => CardNumberOrScore.Score14,
            13 => CardNumberOrScore.Score13,
            12 => CardNumberOrScore.Score12,
            11 => CardNumberOrScore.Score11,
            10 => CardNumberOrScore.Score10,
            9 => CardNumberOrScore.Score9,
            8 => CardNumberOrScore.Score8,
            7 => CardNumberOrScore.Score7,
            6 => CardNumberOrScore.Score6,
            5 => CardNumberOrScore.Score5,
            4 => CardNumberOrScore.Score4,
            3 => CardNumberOrScore.Score3,
            2 => CardNumberOrScore.Score2,
            _ => throw new UnknownValueException($"Unknown value for CardNumber: '{score}'")
        };
        public static int GetScoreTotal(CardNumber card1, CardNumber card2) {
            // TODO: this could be refactored to avoid creating the Hand object
            var hand = new Hand(5F, new NullLogger());
            hand.ReceiveCard(new Card { Number = card1 });
            hand.ReceiveCard(new Card { Number = card2 });
            return hand.GetScore();
        }
        public static int GetScoreTotal(params CardNumber[] cards) {
            // TODO: this could be refactored to avoid creating the Hand object
            var hand = new Hand(5F, new NullLogger());
            foreach (var card in cards) {
                hand.ReceiveCard(new Card { Number = card });
            }
            return hand.GetScore();
        }
        public static bool ContainsAnAce(CardNumber[] cards) => 
            cards.Any(card => card == CardNumber.Ace);

    }
}
