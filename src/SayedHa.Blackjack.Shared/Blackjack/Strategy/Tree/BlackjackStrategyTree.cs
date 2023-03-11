using SayedHa.Blackjack.Shared.Blackjack.Exceptions;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree {
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
}
