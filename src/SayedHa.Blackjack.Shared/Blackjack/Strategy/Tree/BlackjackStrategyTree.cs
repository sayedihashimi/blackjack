﻿using Newtonsoft.Json.Linq;
using SayedHa.Blackjack.Shared.Blackjack.Exceptions;
using SayedHa.Blackjack.Shared.Extensions;
using System.ComponentModel.Design.Serialization;
using System.Text;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree {
    public class BlackjackStrategyTree {
        protected internal BaseTreeNode<CardNumberOrScore, HandAction> aceTree = new ();
        protected internal BaseTreeNode<CardNumberOrScore, HandAction> hardTotalTree = new ();
        protected internal BaseTreeNode<CardNumberOrScore, HandAction> pairTree = new ();
        protected internal bool DoubleEnabled { get; init; } = true;
        protected internal string? Name { get; set; }
        // This is the DollarsRemaining after all the games have been played.
        // If it is set, that's an indication that this strategy doesn't need
        // to be exercised again. The assigned score will continue to be used.
        public float? FitnessScore { get; set; }

        protected internal ulong GetIntFor(HandAction handAction) => handAction switch {
            HandAction.Hit => 2,
            HandAction.Stand => 3,
            HandAction.Double => 4,
            HandAction.Split => 5,
            _ => throw new UnknownValueException($"Unknown value for HandAction: '{handAction}'")
        };

        protected internal ulong GetIntFor(CardNumberOrScore card) => card switch {
            CardNumberOrScore.Ace => 11,
            CardNumberOrScore.Two => 2,
            CardNumberOrScore.Three => 3,
            CardNumberOrScore.Four => 4,
            CardNumberOrScore.Five => 5,
            CardNumberOrScore.Six => 6,
            CardNumberOrScore.Seven => 7,
            CardNumberOrScore.Eight => 8,
            CardNumberOrScore.Nine => 9,
            CardNumberOrScore.Ten => 10,
            CardNumberOrScore.Jack => 10,
            CardNumberOrScore.Queen => 10,
            CardNumberOrScore.King => 10,
            CardNumberOrScore.Score21 => 21,
            CardNumberOrScore.Score20 => 20,
            CardNumberOrScore.Score19 => 19,
            CardNumberOrScore.Score18 => 18,
            CardNumberOrScore.Score17 => 17,
            CardNumberOrScore.Score16 => 16,
            CardNumberOrScore.Score15 => 15,
            CardNumberOrScore.Score14 => 14,
            CardNumberOrScore.Score13 => 13,
            CardNumberOrScore.Score12 => 12,
            CardNumberOrScore.Score11 => 11,
            CardNumberOrScore.Score10 => 10,
            CardNumberOrScore.Score9 => 9,
            CardNumberOrScore.Score8 => 8,
            CardNumberOrScore.Score7 => 7,
            CardNumberOrScore.Score6 => 6,
            CardNumberOrScore.Score5 => 5,
            CardNumberOrScore.Score4 => 4,
            CardNumberOrScore.Score3 => 3,
            CardNumberOrScore.Score2 => 2,
            CardNumberOrScore.Busted => 22,
            _ => throw new UnexpectedValueException($"Unexpected value for CardNumberOrScore: '{card}'")
        };
        /// <summary>
        /// Use this to register pair splits.
        /// </summary>
        protected internal void AddPairSplitNextAction(CardNumber dealerCard, CardNumber pairCard, HandAction handAction = HandAction.Split) {
            // TODO: for all the cards that have a value of 10 we can use the same node, don't need different ones.
            (_, var pairRootNode) = pairTree.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard), NodeType.TreeNode);
            (bool pairNodeCreated, var pairNode) = pairRootNode.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(pairCard), NodeType.LeafNode);

            if (pairNode is LeafNode<CardNumberOrScore, HandAction> leafNode) {
                if (!pairNodeCreated) {
                    // TODO: Improve this
                    // Console.WriteLine($"Over writing next hand action for pair '{pairCard}', from '{leafNode.Value}' to '{HandAction.Split}'.");
                }
                leafNode.Value = handAction;
            }
            else {
                throw new UnexpectedNodeTypeException($"Expected LeafNode but instead received null or an object of type '{pairNode.GetType().FullName}'");
            }
        }

        /// <summary>
        /// Use this when the cards dealt have an Ace
        /// The score to pass in here is the sum of the
        /// score of the other cards besides the Ace.
        /// </summary>
        protected internal void AddSoftTotalNextAction(CardNumber dealerCard, int scoreTotal, HandAction nextHandAction) {
            if (scoreTotal > 10) {
                Console.WriteLine("over 10");
            }
            (_, var aceDealerCardNode) = aceTree.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard), NodeType.TreeNode);
            (var scoreTotalNodeCreated, var scoreTotalNode) = aceDealerCardNode.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(scoreTotal), NodeType.LeafNode);

            if (scoreTotalNode is LeafNode<CardNumberOrScore, HandAction> leafNode) {
                if (!scoreTotalNodeCreated) {
                    // TODO: Improve this
                    // Console.WriteLine($"Over writing next hand action for score '{scoreTotal}', from '{leafNode.Value}' to '{nextHandAction}'.");
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
                    // Console.WriteLine($"Over writing next hand action for score '{scoreTotal}', to '{nextHandAction}'.");
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
                // AddPairToSplit(opCard1.GetValues()[0]);
                AddPairSplitNextAction(dealerCard, opCard1);
            }
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

            // TODO: Split/Double shouldn't be specified here, should we add a warning?

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

            if (opCard1.IsAPairWith(opCard2) && DoesPairTreeContainSplit(dealerCard, opCard1)) {
                return HandAction.Split;
            }

            if (!opCard1.IsAPairWith(opCard2) && opCard1.ContainsAnAce(opCard2)) {
                // After sort the Ace should be the secondCardNumber
                (CardNumber firstCardNumber, CardNumber secondCardNumber) = opCard1.Sort(opCard2);
                var valFromAceTree = GetFromAceTree(dealerCard, CardNumberHelper.GetNumericScore(firstCardNumber));
                if(valFromAceTree is object) {
                    return valFromAceTree.Value;
                }
                else {
                    Console.WriteLine("its here");
                }
            }

            var nextHandAction = GetFromHardTotalTree(dealerCard, CardNumberHelper.GetScoreTotal(opCard1, opCard2));
            if (nextHandAction!.Value == HandAction.Double && !DoubleEnabled) {
                nextHandAction = HandAction.Hit;
            }
            return nextHandAction.Value;
        }
        public HandAction GetNextHandAction(CardNumber dealerCard, params CardNumber[] opCards) {
            // need to check to see if there is an Ace or not in the opCards
            // if there is an ace, check the aceTree to see if there is a result
            // if no result there, return the result from the hardTotal tree.

            var cardScore = CardNumberHelper.GetScoreTotal(opCards);
            if (cardScore >= BlackjackSettings.GetBlackjackSettings().MaxScore) {
                return HandAction.Stand;
            }

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
                    var nextHandActionAce = GetFromAceTree(dealerCard, scoreOtherCards);
                    // don't need to look at DoubleEnabled because Double isn't allowed when there are more than 2 cards dealt
                    if (nextHandActionAce!.Value == HandAction.Double) {
                        nextHandActionAce = HandAction.Hit;
                    }
                    return nextHandActionAce.Value;
                }
            }

            // return result from the hardTotal tree
            var scoreTotal = CardNumberHelper.GetScoreTotal(opCards);
            var nextHandAction = GetFromHardTotalTree(dealerCard, scoreTotal);
            // don't need to look at DoubleEnabled because Double isn't allowed when there are more than 2 cards dealt
            if (nextHandAction!.Value == HandAction.Double) {
                nextHandAction = HandAction.Hit;
            }
            return nextHandAction.Value;
        }
        protected internal bool DoesPairTreeContainSplit(CardNumber dealerCard, CardNumber op1Card) {
            var dealerNode = pairTree.Get(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard));
            if (dealerNode == null) {
                return false;
            }
            var pairNode = dealerNode.Get(CardNumberHelper.ConvertToCardNumberOrScore(op1Card));
            if (pairNode == null) {
                return false;
            }

            if (pairNode is LeafNode<CardNumberOrScore, HandAction> leafNode) {
                return leafNode.Value == HandAction.Split;
            }
            else {
                throw new UnexpectedNodeTypeException($"Expected LeafNode, but received: '{pairNode.GetType().FullName}'");
            }
        }
        //protected internal HandAction GetOrAddFromAceTree(CardNumber dealerCard, int scoreTotalExcludingAce) {
        //    (_, var rootNode) = aceTree.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard), NodeType.TreeNode);
        //    (_, var secondNode) = rootNode.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(scoreTotalExcludingAce), NodeType.LeafNode);

        //    if (secondNode is LeafNode<CardNumberOrScore, HandAction> leafNode) {
        //        return leafNode.Value;
        //    }
        //    else {
        //        throw new UnexpectedNodeTypeException($"Expected LeafNode but instead received null or an object of type '{secondNode.GetType().FullName}'");
        //    }
        //}
        protected internal HandAction? GetFromAceTree(CardNumber dealerCard, int scoreTotalExcludingAce) {
            var rootNode = aceTree.Get(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard));
            if (rootNode is null) { return null; }

            var secondNode = rootNode.Get(CardNumberHelper.ConvertToCardNumberOrScore(scoreTotalExcludingAce));
            if (secondNode is LeafNode<CardNumberOrScore, HandAction> leafNode) {
                return leafNode.Value;
            }

            return null;
        }
        //protected internal HandAction GetOrAddFromHardTotalTree(CardNumber dealerCard, int scoreTotal) {
        //    (_, var dealerCardNode) = hardTotalTree.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard), NodeType.TreeNode);
        //    (_, var scoreTotalNode) = dealerCardNode.GetOrAdd(CardNumberHelper.ConvertToCardNumberOrScore(scoreTotal), NodeType.LeafNode);

        //    if (scoreTotalNode is LeafNode<CardNumberOrScore, HandAction> leafNode) {
        //        return leafNode.Value;
        //    }
        //    else {
        //        throw new UnexpectedNodeTypeException($"Expected LeafNode but instead received null or an object of type '{scoreTotalNode.GetType().FullName}'");
        //    }
        //}
        protected internal HandAction? GetFromHardTotalTree(CardNumber dealerCard, int scoreTotal) {
            var dealerNode = hardTotalTree.Get(CardNumberHelper.ConvertToCardNumberOrScore(dealerCard));
            if(dealerNode is null) {
                return null;
            }
            var scoreTotalNode = dealerNode.Get(CardNumberHelper.ConvertToCardNumberOrScore(scoreTotal));
            if(scoreTotalNode is LeafNode<CardNumberOrScore,HandAction> leafNode) {
                return leafNode.Value;
            }

            return null;
        }
        public Comparison<BlackjackStrategyTree> GetBlackjackTreeComparison() => (strategy1, strategy2) => (strategy1.FitnessScore, strategy2.FitnessScore) switch {
            (null, null) => 0,
            (not null, null) => -1,
            (null, not null) => 1,
            (_, _) => -1 * strategy1.FitnessScore.Value.CompareTo(strategy2.FitnessScore.Value),
        };
        public Comparison<ITreeNode<CardNumberOrScore, HandAction>> GetTreeNodeComparison() => (node1, node2) => (node1, node2) switch {
            (null, null) => 0,
            (not null, null) => -1,
            (null, not null) => 1,
            (node1: var n1, node2: var n2) when n1.Id == n2.Id => 0,
            (node1: var n1, node2: var n2) when n1.Id < n2.Id => -1,
            (node1: var n1, node2: var n2) when n1.Id > n2.Id => 1,
            _ => throw new NotImplementedException()
        };

        public Comparison<CardNumberOrScore> GetCardNumberOrScoreComparison() => (card1, card2) => CardNumberHelper.GetNumericScore(card1).CompareTo(CardNumberHelper.GetNumericScore(card2));

        public string GetString() {
            var sb = new StringBuilder();
            var sWriter = new StringWriter(sb);
            WriteTreeStringTo(sWriter);
            return sb.ToString();
        }
        private string? _treeId;
        protected internal string GetTreeIdString() {
            if(_treeId == null) {
                var stringBuilder = new StringBuilder();
                using var sWriter = new StringWriter(stringBuilder);
                WriteTreeStringTo(sWriter, true);
                sWriter.Flush();
                _treeId = stringBuilder.ToString();
            }

            return _treeId;
        }
        public void WriteTreeStringTo(StringWriter writer, bool compactStr = false) {
            int columnWidth = 4;
            if (hardTotalTree != null
                && hardTotalTree.Children != null
                && hardTotalTree.Children.Count > 0) {
                if (!compactStr) {

                    writer.WriteLine($"Fitness Score: {FitnessScore} {Name}");
                }

                var treeAsDictionary = GetDictionaryForTree(hardTotalTree);
                WriteStringForDictionary(writer, columnWidth, "hard-totals", treeAsDictionary, hardTotalTree, compactStr);
            }
            writer.WriteLine();
            if (aceTree != null && aceTree.Children!.Count > 0) {
                var treeAsDictionary = GetDictionaryForTree(aceTree);

                WriteStringForDictionary(writer, columnWidth, "soft-totals", treeAsDictionary, aceTree, compactStr);
            }
            if (pairTree?.Children?.Count > 0) {
                WritePairTreeTo(writer, columnWidth, compactStr);
            }
        }
        private void WriteStringForDictionary(StringWriter writer,
            int columnWidth,
            string treeName,
            Dictionary<CardNumberOrScore, List<LeafNode<CardNumberOrScore, HandAction>>> treeAsDictionary,
            BaseTreeNode<CardNumberOrScore, HandAction> tree, 
            bool compactStr = false) {

            if (!compactStr) {
                writer.WriteLine(treeName);
                writer.Write(new string(' ', columnWidth));
            }

            tree.Children!.Sort(GetTreeNodeComparison());
            for (int i = 0; i < tree.Children!.Count; i++) {
                var dealerNode = tree.Children![i];
                if (!compactStr) {
                    var str = i == tree.Children!.Count - 1 ? $"{GetStrFor(dealerNode.Id).PadLeft(columnWidth - 1)}" : $"{GetStrFor(dealerNode.Id)},".PadLeft(columnWidth);
                    writer.Write(str);
                }
                else {
                    writer.Write(GetStrFor(dealerNode.Id));
                }
            }

            if (!compactStr) {
                writer.WriteLine();
            }

            var keys = treeAsDictionary.Keys.ToList();
            keys.Sort(GetCardNumberOrScoreComparison());
            foreach (var key in keys) {
                var leafNodes = treeAsDictionary[key];
                // print the hard total value
                if (!compactStr) {
                    writer.Write($"{GetStrFor(key)},".PadLeft(columnWidth));
                }
                else {
                    writer.Write(GetStrFor(key));
                }

                // write each value
                for (int i = 0; i < leafNodes.Count; i++) {
                    if (!compactStr) {
                        var str = i == leafNodes.Count - 1 ? $"{GetStrFor(leafNodes[i].Value).PadLeft(columnWidth - 1)}" : $"{GetStrFor(leafNodes[i].Value)},".PadLeft(columnWidth);
                        writer.Write(str);
                    }
                    else {
                        writer.Write(GetStrFor(leafNodes[i].Value));
                    }
                }

                if (!compactStr) {
                    writer.WriteLine();
                }

            }
        }
        private void WritePairTreeTo(StringWriter writer, int columnWidth, bool compactStr = false) {
            if (!compactStr) {
                writer.WriteLine("pairs");
                writer.Write(new string(' ', columnWidth));
            }
            

            // iterate through all dealer cards and all possible player cards
            var dealerCards = ((CardNumber[])Enum.GetValues(typeof(CardNumber))).ToList();

            // remove J/Q/K cards because 10 will cover those in the pair tree
            dealerCards.Remove(CardNumber.Jack);
            dealerCards.Remove(CardNumber.Queen);
            dealerCards.Remove(CardNumber.King);
            dealerCards.Sort((c1, c2) => c1.CompareTo(c2));

            // write out the dealer card number row
            for (int i = 0; i < dealerCards.Count; i++) {
                var dealerCardScore = CardNumberHelper.ConvertToCardNumberOrScore(dealerCards[i]);
                if (!compactStr) {
                    var str = i == dealerCards.Count - 1 ? GetStrFor(dealerCardScore).PadLeft(columnWidth - 1) : $"{GetStrFor(dealerCardScore)},".PadLeft(columnWidth);
                    writer.Write(str);
                }
                else {
                    var str = i == dealerCards.Count - 1 ? GetStrFor(dealerCardScore) : $"{GetStrFor(dealerCardScore)}";
                    writer.Write(str);
                }
            }
            if (!compactStr) {
                writer.WriteLine();
            }

            var allPlayerCards = ((CardNumber[])Enum.GetValues(typeof(CardNumber))).ToList();
            allPlayerCards.Remove(CardNumber.Jack);
            allPlayerCards.Remove(CardNumber.Queen);
            allPlayerCards.Remove(CardNumber.King);
            allPlayerCards.Sort((c1, c2) => c1.CompareTo(c2));

            for (int i = 0; i < allPlayerCards.Count; i++) {
                var op1Card = allPlayerCards[i];
                if (!compactStr) {
                    writer.Write($"{GetStrFor(op1Card)},".PadLeft(columnWidth));
                }
                else {
                    writer.Write($"{GetStrFor(op1Card)}");
                }

                for (int j = 0; j < dealerCards.Count; j++) {
                    var dealerCard = dealerCards[j];
                    if (DoesPairTreeContainSplit(dealerCard, op1Card)) {
                        if (!compactStr) {
                            var str = j == allPlayerCards.Count - 1 ? "Sp".PadLeft(columnWidth - 1) : "Sp,".PadLeft(columnWidth);
                            writer.Write(str);
                        }
                        else {
                            writer.Write("Sp");
                        }
                    }
                    else {
                        if (!compactStr) {
                            var str = j == allPlayerCards.Count - 1 ? "-".PadLeft(columnWidth - 1) : "-,".PadLeft(columnWidth);
                            writer.Write(str);
                        }
                        else {
                            writer.Write("-");
                        }
                    }
                }

                if (!compactStr) {
                    writer.WriteLine();
                }
            }
        }
        private Dictionary<CardNumberOrScore, List<LeafNode<CardNumberOrScore, HandAction>>> GetDictionaryForTree(BaseTreeNode<CardNumberOrScore, HandAction> aceTree) {
            var treeAsDictionary = new Dictionary<CardNumberOrScore, List<LeafNode<CardNumberOrScore, HandAction>>>();

            // create a list of nodes for each dealer card and add to the dictionary
            aceTree.Children!.Sort(GetTreeNodeComparison());
            foreach (var dealerNode in aceTree.Children!) {
                treeAsDictionary.Add(dealerNode.Id, new List<LeafNode<CardNumberOrScore, HandAction>>());
            }

            var scoreAndHandActionDict = new Dictionary<CardNumberOrScore, List<LeafNode<CardNumberOrScore, HandAction>>>();
            // aceTree.Children!.Sort(GetTreeNodeComparison());
            foreach (var dealerNode in aceTree.Children) {
                dealerNode.Children!.Sort(GetTreeNodeComparison());
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
        }
        private string GetStrFor(HandAction v) => v switch {
            HandAction.Hit => "H",
            HandAction.Stand => "S",
            HandAction.Split => "Sp",
            HandAction.Double => "D",
            _ => throw new NotImplementedException()
        };
        private string GetStrFor(CardNumber cardNumber) => cardNumber switch {
            CardNumber.Ace => "A",
            CardNumber.Two => "2",
            CardNumber.Three => "3",
            CardNumber.Four => "4",
            CardNumber.Five => "5",
            CardNumber.Six => "6",
            CardNumber.Seven => "7",
            CardNumber.Eight => "8",
            CardNumber.Nine => "9",
            CardNumber.Ten => "10",
            CardNumber.Jack => "10",
            CardNumber.Queen => "10",
            CardNumber.King => "10",
            _ => throw new UnexpectedValueException($"Unexpected value for CardNumber: '{cardNumber}'")
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
