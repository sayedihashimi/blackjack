using Newtonsoft.Json.Linq;
using SayedHa.Blackjack.Shared.Blackjack.Exceptions;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree {
    public class BlackjackStrategyTree {
        protected internal BaseTreeNode<CardNumberOrScore, HandAction> aceTree = new BaseTreeNode<CardNumberOrScore, HandAction>();
        protected internal BaseTreeNode<CardNumberOrScore, HandAction> hardTotalTree = new BaseTreeNode<CardNumberOrScore, HandAction>();

        protected internal List<int> pairsToSplit = new List<int>();
        protected internal bool DoubleEnabled { get; init; } = true;

        protected internal void AddPairToSplit(int cardValue) {
            if (!pairsToSplit.Contains(cardValue)) {
                pairsToSplit.Add(cardValue);
            }
        }

        /// <summary>
        /// Use this when the cards dealt have an Ace
        /// The score to pass in here is the sum of the
        /// score of the other cards besides the Ace.
        /// </summary>
        protected internal void AddSoftTotalNextAction(CardNumber dealerCard, int scoreTotal, HandAction nextHandAction) {
            (_, var aceDealerCardNode) = aceTree.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard), NodeType.TreeNode);
            (var scoreTotalNodeCreated, var scoreTotalNode) = aceDealerCardNode.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(scoreTotal), NodeType.LeafNode);

            if (scoreTotalNode is LeafNode<CardNumberOrScore, HandAction> leafNode) {
                if (!scoreTotalNodeCreated) {
                    // TODO: Improve this
                    Console.WriteLine($"Over writing next hand action for score '{scoreTotal}', from '{leafNode.Value}' to '{nextHandAction}'.");
                }
                leafNode.Value = nextHandAction;
            }
            else {
                throw new UnexpectedNodeTypeException($"Expected LeafNode but instead received null or an object of type '{scoreTotalNode.GetType().FullName}'");
            }
        }
        /// <summary>
        /// Hard total is when you don't have an Ace or a pair of cards.
        /// </summary>
        protected internal void AddHardTotalNextAction(CardNumber dealerCard, int scoreTotal, HandAction nextHandAction) {
            (_, var dealerCardNode) = hardTotalTree.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard), NodeType.TreeNode);
            (var scoreTotalNodeCreated, var scoreTotalNode) = dealerCardNode.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(scoreTotal), NodeType.LeafNode);

            if (scoreTotalNode is LeafNode<CardNumberOrScore, HandAction> leafNode) {
                if (!scoreTotalNodeCreated) {
                    // TODO: Improve this
                    Console.WriteLine($"Over writing next hand action for score '{scoreTotal}', to '{nextHandAction}'.");
                }
                leafNode.Value = nextHandAction;
            }
            else {
                throw new UnexpectedNodeTypeException($"Expected LeafNode but instead received null or an object of type '{scoreTotalNode.GetType().FullName}'");
            }

        }
        public void AddNextHandAction(CardNumber dealerCard, CardNumber opCard1, CardNumber opCard2, HandAction nextHandAction) {
            if (nextHandAction == HandAction.Split) {
                // verify both cards are a pair
                if (opCard1 != opCard2) {
                    throw new UnexpectedValueException($"Expected a pair but received '{opCard1}' and '{opCard2}'");
                }
                AddPairToSplit(opCard1.GetValues()[0]);
            }
            //if (opCard1.IsAPairWith(opCard2)) {
            //    AddPairNextAction(dealerCard, opCard1, nextHandAction);
            //}
            else if (opCard1.ContainsAnAce(opCard2)) {
                // After sort the Ace should be the secondCardNumber
                (CardNumber firstCardNumber, CardNumber secondCardNumber) = opCard1.Sort(opCard2);
                AddSoftTotalNextAction(dealerCard, firstCardNumber.GetValues()[0], nextHandAction);
            }
            else {
                AddHardTotalNextAction(dealerCard, CardNumberHelper.GetScoreTotal(opCard1, opCard2), nextHandAction);
            }
        }
        public void AddNextHandAction(CardNumber dealerCard, HandAction nextHandAction, params CardNumber[] opCards) {
            ArgumentNullException.ThrowIfNull(opCards);

            if (opCards.Length < 2) {
                throw new UnexpectedValueException($"To add a hand action, two or more cards are required, number of cards: '{opCards.Length}'");
            }
            if (opCards.Length == 2) {
                AddNextHandAction(dealerCard, opCards[0], opCards[1], nextHandAction);
            }
            else {
                if (nextHandAction == HandAction.Split) {
                    throw new UnexpectedValueException($"Cannot split when hand contains more than two cards, number of cards: '{opCards.Length}'");
                }
                // add to the hardTotal tree
                var scoreTotal = CardNumberHelper.GetScoreTotal(opCards);
                AddHardTotalNextAction(dealerCard, scoreTotal, nextHandAction);
            }
        }
        // when there is only two cards this method should be called instead of the method using params
        // performance will be slightly better in this method.
        public HandAction GetNextHandAction(CardNumber dealerCard, CardNumber opCard1, CardNumber opCard2) {
            // 1: check if it's a pair
            // 2: check if the cards include an Ace, if so return the result from the Ace tree
            // 3: return the result from the HardTotals tree

            if (opCard1.IsAPairWith(opCard2) && pairsToSplit.Contains(opCard1.GetValues()[0])) {
                return HandAction.Split;
            }

            if (opCard1.ContainsAnAce(opCard2)) {
                // After sort the Ace should be the secondCardNumber
                (CardNumber firstCardNumber, CardNumber secondCardNumber) = opCard1.Sort(opCard2);
                return GetOrAddFromAceTree(dealerCard, firstCardNumber.GetValues()[0]);
            }
            else {
                var nextHandAction = GetOrAddFromHardTotalTree(dealerCard, CardNumberHelper.GetScoreTotal(opCard1, opCard2));
                if(nextHandAction == HandAction.Double && !DoubleEnabled) {
                    nextHandAction = HandAction.Hit;
                }
                return nextHandAction;
            }
        }
        public HandAction GetNextHandAction(CardNumber dealerCard, params CardNumber[] opCards) {
            // need to check to see if there is an Ace or not in the opCards
            // if there is an ace, check the aceTree to see if there is a result
            // if no result there, return the result from the hardTotal tree.

            var containsAce = CardNumberHelper.ContainsAnAce(opCards);
            if (containsAce) {
                var otherCards = new CardNumber[opCards.Length - 1];

                // only want to ignore 1 ace
                bool aceCardFound = false;
                int currentIndex = 0;
                foreach (var card in opCards) {
                    if (!aceCardFound && card == CardNumber.Ace) {
                        aceCardFound = true;
                    }
                    else {
                        otherCards[currentIndex++] = card;
                    }
                }
                var scoreOtherCards = CardNumberHelper.GetScoreTotal(otherCards);
                if (scoreOtherCards <= 9) {
                    // check the aceTree for the result
                    var nextHandActionAce = GetOrAddFromAceTree(dealerCard, scoreOtherCards);
                    // don't need to look at DoubleEnabled because Double isn't allowed when there are more than 2 cards dealt
                    if(nextHandActionAce == HandAction.Double) {
                        nextHandActionAce = HandAction.Hit;
                    }
                    return nextHandActionAce;
                }
            }

            // return result from the hardTotal tree
            var scoreTotal = CardNumberHelper.GetScoreTotal(opCards);
            var nextHandAction = GetOrAddFromHardTotalTree(dealerCard, scoreTotal);
            // don't need to look at DoubleEnabled because Double isn't allowed when there are more than 2 cards dealt
            if (nextHandAction == HandAction.Double) {
                nextHandAction = HandAction.Hit;
            }
            return nextHandAction;
        }
        protected internal HandAction GetOrAddFromAceTree(CardNumber dealerCard, int scoreTotalExcludingAce) {
            (_, var rootNode) = aceTree.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard), NodeType.TreeNode);
            (_, var secondNode) = rootNode.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(scoreTotalExcludingAce), NodeType.LeafNode);

            if (secondNode is LeafNode<CardNumberOrScore, HandAction> leafNode) {

                return leafNode.Value;
            }
            else {
                throw new UnexpectedNodeTypeException($"Expected LeafNode but instead received null or an object of type '{secondNode.GetType().FullName}'");
            }
        }
        protected internal HandAction GetOrAddFromHardTotalTree(CardNumber dealerCard, int scoreTotal) {
            (_, var dealerCardNode) = hardTotalTree.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard), NodeType.TreeNode);
            (_, var scoreTotalNode) = dealerCardNode.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(scoreTotal), NodeType.LeafNode);

            if (scoreTotalNode is LeafNode<CardNumberOrScore, HandAction> leafNode) {
                return leafNode.Value;
            }
            else {
                throw new UnexpectedNodeTypeException($"Expected LeafNode but instead received null or an object of type '{scoreTotalNode.GetType().FullName}'");
            }
        }

        public void WriteTreeStringTo(StringWriter writer) {
            int columnWidth = 3;
            if (hardTotalTree != null 
                && hardTotalTree.Children != null
                && hardTotalTree.Children.Count > 0) {
                var dealerNodeList = new List<ITreeNode<CardNumberOrScore, HandAction>>();
                dealerNodeList.Sort();

                var treeAsDictionary = GetDictionaryForTree(hardTotalTree);
                WriteStringForDictionary(writer, columnWidth, "hard-totals", treeAsDictionary);
            }
            writer.WriteLine();
            if(aceTree != null && aceTree.Children!.Count > 0) {
                // create a list of the dealer nodes, sort them and then 
                var dealerNodeList = new List<ITreeNode<CardNumberOrScore,HandAction>>();
                dealerNodeList.Sort();
                // var leafNodeList = new List<LeafNode<CardNumberOrScore, HandAction>>();

                var treeAsDictionary = GetDictionaryForTree(aceTree);
                
                //foreach (var dealerNode in aceTree.Children) {
                //    dealerNodeList.Add(dealerNode);
                //    var currentLeafNodeList = new List<LeafNode<CardNumberOrScore, HandAction>>();

                //    foreach(var child in dealerNode.Children!) {
                //        if(child is LeafNode<CardNumberOrScore, HandAction> leafNode) {
                //            currentLeafNodeList.Add(leafNode);
                //        }
                //        else {
                //            throw new UnexpectedNodeTypeException($"Expected a LeafNode but instead received type: '{child.GetType().FullName}'");
                //        }
                //    }
                //    treeAsDictionary.Add(dealerNode.Id, currentLeafNodeList);
                //}

                // print dealer cards header
                
                WriteStringForDictionary(writer, columnWidth, "soft-totals", treeAsDictionary);
                //writer.WriteLine("soft-totals");
                //writer.Write(new string(' ', columnWidth));

                //for(int i = 0; i < aceTree.Children!.Count; i++) {
                //    var dealerNode = aceTree.Children![i];
                //    var str = i == aceTree.Children!.Count - 1 ? $"{GetStrFor(dealerNode.Id).PadLeft(columnWidth -1)}" : $"{GetStrFor(dealerNode.Id)},".PadLeft(columnWidth);
                //    writer.Write(str);
                //}

                //writer.WriteLine();
                //foreach (var key in treeAsDictionary.Keys) { 
                //    var leafNodes = treeAsDictionary[key];
                //    // print the hard total value
                //    writer.Write($"{GetStrFor(key)},".PadLeft(columnWidth));
                //    // write each value
                //    for(int i = 0; i < leafNodes.Count; i++) {
                //        var str = i == leafNodes.Count - 1 ? $"{GetStrFor(leafNodes[i].Value).PadLeft(columnWidth-1)}" : $"{GetStrFor(leafNodes[i].Value)},".PadLeft(columnWidth);
                //        writer.Write(str);
                //    }
                //    writer.WriteLine();
                //}
            }
        }
        private void WriteStringForDictionary(StringWriter writer, int columnWidth, string treeName, Dictionary<CardNumberOrScore, List<LeafNode<CardNumberOrScore, HandAction>>> treeAsDictionary) {
            writer.WriteLine(treeName);
            writer.Write(new string(' ', columnWidth));

            for (int i = 0; i < aceTree.Children!.Count; i++) {
                var dealerNode = aceTree.Children![i];
                var str = i == aceTree.Children!.Count - 1 ? $"{GetStrFor(dealerNode.Id).PadLeft(columnWidth - 1)}" : $"{GetStrFor(dealerNode.Id)},".PadLeft(columnWidth);
                writer.Write(str);
            }

            writer.WriteLine();
            foreach (var key in treeAsDictionary.Keys) {
                var leafNodes = treeAsDictionary[key];
                // print the hard total value
                writer.Write($"{GetStrFor(key)},".PadLeft(columnWidth));
                // write each value
                for (int i = 0; i < leafNodes.Count; i++) {
                    var str = i == leafNodes.Count - 1 ? $"{GetStrFor(leafNodes[i].Value).PadLeft(columnWidth - 1)}" : $"{GetStrFor(leafNodes[i].Value)},".PadLeft(columnWidth);
                    writer.Write(str);
                }
                writer.WriteLine();
            }
        }
        private Dictionary<CardNumberOrScore, List<LeafNode<CardNumberOrScore, HandAction>>> GetDictionaryForTree(BaseTreeNode<CardNumberOrScore, HandAction> aceTree) {
            var treeAsDictionary = new Dictionary<CardNumberOrScore, List<LeafNode<CardNumberOrScore, HandAction>>>();

            // create a list of nodes for each dealer card and add to the dictionary
            foreach(var dealerNode in aceTree.Children!) {
                treeAsDictionary.Add(dealerNode.Id, new List<LeafNode<CardNumberOrScore, HandAction>>());
            }

            var scoreAndHandActionDict = new Dictionary<CardNumberOrScore, List<LeafNode<CardNumberOrScore, HandAction>>>();
            foreach(var dealerNode in aceTree.Children) {
                foreach (var child in dealerNode.Children!) {
                    if (child is LeafNode<CardNumberOrScore, HandAction> leafNode) {
                        if (!scoreAndHandActionDict.ContainsKey(child.Id)) {
                            scoreAndHandActionDict.Add(child.Id, new List<LeafNode<CardNumberOrScore, HandAction>>());
                        }

                        scoreAndHandActionDict[child.Id].Add(leafNode);
                    }
                    else {
                        throw new UnexpectedNodeTypeException($"Expected a LeafNode but instead received type: '{child.GetType().FullName}'");
                    }
                }
            }

            return scoreAndHandActionDict;

            //foreach (var dealerNode in aceTree.Children) {
            //    var currentLeafNodeList = new List<LeafNode<CardNumberOrScore, HandAction>>();
            //    foreach (var child in dealerNode.Children!) {
            //        if (child is LeafNode<CardNumberOrScore, HandAction> leafNode) {
            //            currentLeafNodeList.Add(leafNode);
            //            scoreAndHandActionDict[child.Id].Add(leafNode);
            //        }
            //        else {
            //            throw new UnexpectedNodeTypeException($"Expected a LeafNode but instead received type: '{child.GetType().FullName}'");
            //        }
            //    }
            //    treeAsDictionary.Add(dealerNode.Id, currentLeafNodeList);
            //}

            





            return treeAsDictionary;
        }
        private string GetStrFor(HandAction v) => v switch {
            HandAction.Hit => "H",
            HandAction.Stand => "S",
            HandAction.Split => "Sp",
            HandAction.Double => "D",
            _ => throw new NotImplementedException()
        };
        private string GetStrFor(CardNumberOrScore card) => card switch {
            CardNumberOrScore.Ace => "A",
            CardNumberOrScore.Two => "2",
            CardNumberOrScore.Three => "3",
            CardNumberOrScore.Four => "4",
            CardNumberOrScore.Five => "5",
            CardNumberOrScore.Six => "6",
            CardNumberOrScore.Seven => "7",
            CardNumberOrScore.Eight => "8",
            CardNumberOrScore.Nine => "9",
            CardNumberOrScore.Ten => "10",
            CardNumberOrScore.Jack => "10",
            CardNumberOrScore.Queen => "10",
            CardNumberOrScore.King => "10",
            CardNumberOrScore.Score21 => "21",
            CardNumberOrScore.Score20 => "20",
            CardNumberOrScore.Score19 => "19",
            CardNumberOrScore.Score18 => "18",
            CardNumberOrScore.Score17 => "17",
            CardNumberOrScore.Score16 => "16",
            CardNumberOrScore.Score15 => "15",
            CardNumberOrScore.Score14 => "14",
            CardNumberOrScore.Score13 => "13",
            CardNumberOrScore.Score12 => "12",
            CardNumberOrScore.Score11 => "11",
            CardNumberOrScore.Score10 => "10",
            CardNumberOrScore.Score9 => "9",
            CardNumberOrScore.Score8 => "8",
            CardNumberOrScore.Score7 => "7",
            CardNumberOrScore.Score6 => "6",
            CardNumberOrScore.Score5 => "5",
            CardNumberOrScore.Score4 => "4",
            CardNumberOrScore.Score3 => "3",
            CardNumberOrScore.Score2 => "2",
            CardNumberOrScore.Busted => "B",

            _ => throw new UnexpectedValueException($"Unexpected value for CardNumberOrScore: '{card}'")
        };
    }
}
