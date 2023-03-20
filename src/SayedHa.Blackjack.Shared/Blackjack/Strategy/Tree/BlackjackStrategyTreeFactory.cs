using SayedHa.Blackjack.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree {
    public class BlackjackStrategyTreeFactory {
        private static BlackjackStrategyTreeFactory? instance;
        private List<CardNumber> _allCardNumbersShuffled { get; }
        private List<CardNumber> _allCardNumbers { get; }
        private Random random = new Random();
        private bool _useRandomNumberGenerator;
        private bool _doubleEnabled = true;

        // all hand actions excluding split, that has special handling
        private HandAction[] _allHandActions = new HandAction[] {
            HandAction.Hit,
            HandAction.Stand,
            HandAction.Double
        };

        private BlackjackStrategyTreeFactory() : this(false) {

        }
        private BlackjackStrategyTreeFactory(bool useRandomNumberGenerator) {
            _allCardNumbers = ((CardNumber[])Enum.GetValues(typeof(CardNumber))).ToList();
            _allCardNumbersShuffled = ((CardNumber[])Enum.GetValues(typeof(CardNumber))).ToList();
            _allCardNumbersShuffled.Shuffle();
        }

        /// <summary>
        /// The value for <code>useRandomNumberGenerator</code> is only used on the first call to
        /// this method, when the instance is created. After that, this parameter is ignored.
        /// </summary>
        public static BlackjackStrategyTreeFactory GetInstance(bool useRandomNumberGenerator) {
            if(instance is null) {
                instance = new BlackjackStrategyTreeFactory(useRandomNumberGenerator);
            }

            return instance;
        }
        public BlackjackStrategyTree CreateNewRandomTree() {
            var newTree = new BlackjackStrategyTree();

            // Need to populate three things
            // 1. Pairs to split, create a random # of random pairs to split
            // 2. Soft totals (2 card hand, one of which is an Ace)
            // 3. Hard totals
            AddRandomPairsToSplit(newTree);
            AddRandomSoftTotals(newTree);
            AddRandomHardTotals(newTree);

            return newTree;
        }
        /// <summary>
        /// Will add a random number of random pairs to the pairs list.
        /// </summary>
        protected internal void AddRandomPairsToSplit(BlackjackStrategyTree tree) {
            // shuffle the card numbers
            // no need to clone and then shuffle, just keep shuffling the cards
            _allCardNumbersShuffled.Shuffle(_useRandomNumberGenerator);
            int numPairsToAdd = GetRandomIntBetween(2, 13 + 1);
            int numDealerCardsToAdd = GetRandomIntBetween(2, 10 + 1);

            var dealerCardList = new List<CardNumber>();
            for(int iDealerCard = 0;iDealerCard < numDealerCardsToAdd; iDealerCard++) {
                dealerCardList.Add(_allCardNumbersShuffled[iDealerCard]);
            }
            _allCardNumbersShuffled.Shuffle(_useRandomNumberGenerator);
            var numPairsList = new List<CardNumber>();
            for(int iNumPairs = 0; iNumPairs < numPairsToAdd; iNumPairs++) {
                numPairsList.Add(_allCardNumbersShuffled[iNumPairs]);
            }

            // now add the pairs to a random amount of dealer cards
            foreach(var dealerCard in dealerCardList) {
                foreach(var pairToAdd in numPairsList) {
                    if (GetRandomBool()) {
                        // add the pair
                        tree.AddPairSplitNextAction(dealerCard, pairToAdd);
                    }
                }
            }
        }
        protected internal void AddRandomSoftTotals(BlackjackStrategyTree tree) {
            foreach (var cardNumber in _allCardNumbers) {
                for(var score = 2; score <= 10; score++) {
                    tree.AddSoftTotalNextAction(cardNumber, score, GetRandomHandAction(_doubleEnabled));
                }
            }
        }
        protected internal void AddRandomHardTotals(BlackjackStrategyTree tree) {
            foreach(var dealerCardNumber in _allCardNumbers) {
                for(int score = 5;score <= 21; score++) {
                    tree.AddHardTotalNextAction(dealerCardNumber, score,GetRandomHandAction(_doubleEnabled));
                }
            }
        }

        public HandAction GetRandomHandAction(bool includeDouble) {
            int fromInclusive = 0;
            int toInclusive = includeDouble ? 2 : 1;
            int indexToReturn = GetRandomIntBetween(fromInclusive, toInclusive + 1);
            return _allHandActions[indexToReturn];
        }

        protected internal int GetRandomIntBetween(int fromInclusive, int toExclusive) => _useRandomNumberGenerator switch {
            true => RandomNumberGenerator.GetInt32(fromInclusive, toExclusive),
            false => random.Next(fromInclusive, toExclusive)
        };
        protected internal bool GetRandomBool() => _useRandomNumberGenerator switch {
            true => RandomNumberGenerator.GetInt32(2) == 0,
            false => random.Next(2) == 0
        };
        public BlackjackStrategyTree GetBasicStrategyTree() {
            var bsTree = new BlackjackStrategyTree();

            bsTree.AddPairSplitNextAction(CardNumber.Two, CardNumber.Ace);
            bsTree.AddPairSplitNextAction(CardNumber.Three, CardNumber.Ace);
            bsTree.AddPairSplitNextAction(CardNumber.Four, CardNumber.Ace);
            bsTree.AddPairSplitNextAction(CardNumber.Five, CardNumber.Ace);
            bsTree.AddPairSplitNextAction(CardNumber.Six, CardNumber.Ace);
            bsTree.AddPairSplitNextAction(CardNumber.Seven, CardNumber.Ace);
            bsTree.AddPairSplitNextAction(CardNumber.Eight, CardNumber.Ace);
            bsTree.AddPairSplitNextAction(CardNumber.Nine, CardNumber.Ace);
            bsTree.AddPairSplitNextAction(CardNumber.Ten, CardNumber.Ace);
            bsTree.AddPairSplitNextAction(CardNumber.Ace, CardNumber.Ace);

            bsTree.AddPairSplitNextAction(CardNumber.Two, CardNumber.Nine);
            bsTree.AddPairSplitNextAction(CardNumber.Three, CardNumber.Nine);
            bsTree.AddPairSplitNextAction(CardNumber.Four, CardNumber.Nine);
            bsTree.AddPairSplitNextAction(CardNumber.Five, CardNumber.Nine);
            bsTree.AddPairSplitNextAction(CardNumber.Six, CardNumber.Nine);
            bsTree.AddPairSplitNextAction(CardNumber.Eight, CardNumber.Nine);
            bsTree.AddPairSplitNextAction(CardNumber.Nine, CardNumber.Nine);

            bsTree.AddPairSplitNextAction(CardNumber.Two, CardNumber.Eight);
            bsTree.AddPairSplitNextAction(CardNumber.Three, CardNumber.Eight);
            bsTree.AddPairSplitNextAction(CardNumber.Four, CardNumber.Eight);
            bsTree.AddPairSplitNextAction(CardNumber.Five, CardNumber.Eight);
            bsTree.AddPairSplitNextAction(CardNumber.Six, CardNumber.Eight);
            bsTree.AddPairSplitNextAction(CardNumber.Seven, CardNumber.Eight);
            bsTree.AddPairSplitNextAction(CardNumber.Eight, CardNumber.Eight);
            bsTree.AddPairSplitNextAction(CardNumber.Nine, CardNumber.Eight);
            bsTree.AddPairSplitNextAction(CardNumber.Ten, CardNumber.Eight);
            bsTree.AddPairSplitNextAction(CardNumber.Ace, CardNumber.Eight);

            bsTree.AddPairSplitNextAction(CardNumber.Two, CardNumber.Seven);
            bsTree.AddPairSplitNextAction(CardNumber.Three, CardNumber.Seven);
            bsTree.AddPairSplitNextAction(CardNumber.Four, CardNumber.Seven);
            bsTree.AddPairSplitNextAction(CardNumber.Five, CardNumber.Seven);
            bsTree.AddPairSplitNextAction(CardNumber.Six, CardNumber.Seven);
            bsTree.AddPairSplitNextAction(CardNumber.Seven, CardNumber.Seven);

            bsTree.AddPairSplitNextAction(CardNumber.Two, CardNumber.Six);
            bsTree.AddPairSplitNextAction(CardNumber.Three, CardNumber.Six);
            bsTree.AddPairSplitNextAction(CardNumber.Four, CardNumber.Six);
            bsTree.AddPairSplitNextAction(CardNumber.Five, CardNumber.Six);
            bsTree.AddPairSplitNextAction(CardNumber.Six, CardNumber.Six);

            bsTree.AddPairSplitNextAction(CardNumber.Five, CardNumber.Four);
            bsTree.AddPairSplitNextAction(CardNumber.Six, CardNumber.Four);

            bsTree.AddPairSplitNextAction(CardNumber.Two, CardNumber.Three);
            bsTree.AddPairSplitNextAction(CardNumber.Three, CardNumber.Three);
            bsTree.AddPairSplitNextAction(CardNumber.Four, CardNumber.Three);
            bsTree.AddPairSplitNextAction(CardNumber.Five, CardNumber.Three);
            bsTree.AddPairSplitNextAction(CardNumber.Six, CardNumber.Three);
            bsTree.AddPairSplitNextAction(CardNumber.Seven, CardNumber.Three);

            bsTree.AddPairSplitNextAction(CardNumber.Two, CardNumber.Two);
            bsTree.AddPairSplitNextAction(CardNumber.Three, CardNumber.Two);
            bsTree.AddPairSplitNextAction(CardNumber.Four, CardNumber.Two);
            bsTree.AddPairSplitNextAction(CardNumber.Five, CardNumber.Two);
            bsTree.AddPairSplitNextAction(CardNumber.Six, CardNumber.Two);
            bsTree.AddPairSplitNextAction(CardNumber.Seven, CardNumber.Two);

            // soft totals
            // Ace + 10 = blackjack, stand
            bsTree.AddSoftTotalNextAction(CardNumber.Two, 10, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Three, 10, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Four, 10, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Five, 10, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Six, 10, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Seven, 10, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Eight, 10, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Nine, 10, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Ten, 10, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Ace, 10, HandAction.Stand);
            // Ace + 9
            bsTree.AddSoftTotalNextAction(CardNumber.Two, 9, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Three, 9, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Four, 9, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Five, 9, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Six, 9, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Seven, 9, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Eight, 9, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Nine, 9, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Ten, 9, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Ace, 9, HandAction.Stand);
            // Ace + 8
            bsTree.AddSoftTotalNextAction(CardNumber.Two, 8, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Three, 8, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Four, 8, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Five, 8, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Six, 8, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Seven, 8, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Eight, 8, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Nine, 8, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Ten, 8, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Ace, 8, HandAction.Stand);

            // Ace + 7
            bsTree.AddSoftTotalNextAction(CardNumber.Two, 7, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Three, 7, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Four, 7, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Five, 7, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Six, 7, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Seven, 7, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Eight, 7, HandAction.Stand);
            bsTree.AddSoftTotalNextAction(CardNumber.Nine, 7, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Ten, 7, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Ace, 7, HandAction.Hit);

            // Ace + 6
            bsTree.AddSoftTotalNextAction(CardNumber.Two, 6, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Three, 6, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Four, 6, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Five, 6, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Six, 6, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Seven, 6, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Eight, 6, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Nine, 6, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Ten, 6, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Ace, 6, HandAction.Hit);

            // Ace + 5
            bsTree.AddSoftTotalNextAction(CardNumber.Two, 5, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Three, 5, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Four, 5, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Five, 5, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Six, 5, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Seven, 5, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Eight, 5, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Nine, 5, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Ten, 5, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Ace, 5, HandAction.Hit);

            // Ace + 4
            bsTree.AddSoftTotalNextAction(CardNumber.Two, 4, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Three, 4, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Four, 4, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Five, 4, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Six, 4, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Seven, 4, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Eight, 4, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Nine, 4, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Ten, 4, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Ace, 4, HandAction.Hit);

            // Ace + 3
            bsTree.AddSoftTotalNextAction(CardNumber.Two, 3, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Three, 3, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Four, 3, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Five, 3, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Six, 3, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Seven, 3, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Eight, 3, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Nine, 3, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Ten, 3, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Ace, 3, HandAction.Hit);

            // Ace + 2
            bsTree.AddSoftTotalNextAction(CardNumber.Two, 2, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Three, 2, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Four, 2, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Five, 2, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Six, 2, HandAction.Double);
            bsTree.AddSoftTotalNextAction(CardNumber.Seven, 2, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Eight, 2, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Nine, 2, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Ten, 2, HandAction.Hit);
            bsTree.AddSoftTotalNextAction(CardNumber.Ace, 2, HandAction.Hit);


            // hard totals
            // 4 - 8 are all hits
            for(int i = 4; i <= 8; i++) {
                bsTree.AddHardTotalNextAction(CardNumber.Two, i, HandAction.Hit);
                bsTree.AddHardTotalNextAction(CardNumber.Three, i, HandAction.Hit);
                bsTree.AddHardTotalNextAction(CardNumber.Four, i, HandAction.Hit);
                bsTree.AddHardTotalNextAction(CardNumber.Five, i, HandAction.Hit);
                bsTree.AddHardTotalNextAction(CardNumber.Six, i, HandAction.Hit);
                bsTree.AddHardTotalNextAction(CardNumber.Seven, i, HandAction.Hit);
                bsTree.AddHardTotalNextAction(CardNumber.Eight, i, HandAction.Hit);
                bsTree.AddHardTotalNextAction(CardNumber.Nine, i, HandAction.Hit);
                bsTree.AddHardTotalNextAction(CardNumber.Ten, i, HandAction.Hit);
                bsTree.AddHardTotalNextAction(CardNumber.Ace, i, HandAction.Hit);
            }

            // 9
            bsTree.AddHardTotalNextAction(CardNumber.Two, 9, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Three, 9, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Four, 9, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Five, 9, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Six, 9, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Seven, 9, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Eight, 9, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Nine, 9, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Ten, 9, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Ace, 9, HandAction.Hit);
            // 10
            bsTree.AddHardTotalNextAction(CardNumber.Two, 10, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Three, 10, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Four, 10, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Five, 10, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Six, 10, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Seven, 10, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Eight, 10, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Nine, 10, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Ten, 10, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Ace, 10, HandAction.Hit);
            // 11
            bsTree.AddHardTotalNextAction(CardNumber.Two, 11, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Three, 11, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Four, 11, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Five, 11, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Six, 11, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Seven, 11, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Eight, 11, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Nine, 11, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Ten, 11, HandAction.Double);
            bsTree.AddHardTotalNextAction(CardNumber.Ace, 11, HandAction.Double);
            // 12
            bsTree.AddHardTotalNextAction(CardNumber.Two, 12, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Three, 12, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Four, 12, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Five, 12, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Six, 12, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Seven, 12, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Eight, 12, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Nine, 12, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Ten, 12, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Ace, 12, HandAction.Hit);
            // 13
            bsTree.AddHardTotalNextAction(CardNumber.Two, 13, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Three, 13, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Four, 13, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Five, 13, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Six, 13, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Seven, 13, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Eight, 13, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Nine, 13, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Ten, 13, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Ace, 13, HandAction.Hit);
            // 14
            bsTree.AddHardTotalNextAction(CardNumber.Two, 14, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Three, 14, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Four, 14, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Five, 14, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Six, 14, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Seven, 14, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Eight, 14, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Nine, 14, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Ten, 14, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Ace, 14, HandAction.Hit);
            // 15
            bsTree.AddHardTotalNextAction(CardNumber.Two, 15, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Three, 15, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Four, 15, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Five, 15, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Six, 15, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Seven, 15, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Eight, 15, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Nine, 15, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Ten, 15, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Ace, 15, HandAction.Hit);
            // 16
            bsTree.AddHardTotalNextAction(CardNumber.Two, 16, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Three, 16, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Four, 16, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Five, 16, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Six, 16, HandAction.Stand);
            bsTree.AddHardTotalNextAction(CardNumber.Seven, 16, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Eight, 16, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Nine, 16, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Ten, 16, HandAction.Hit);
            bsTree.AddHardTotalNextAction(CardNumber.Ace, 16, HandAction.Hit);
            // 17 - 20 stand
            for (int i = 17; i <= 20; i++) {
                bsTree.AddHardTotalNextAction(CardNumber.Two, i, HandAction.Stand);
                bsTree.AddHardTotalNextAction(CardNumber.Three, i, HandAction.Stand);
                bsTree.AddHardTotalNextAction(CardNumber.Four, i, HandAction.Stand);
                bsTree.AddHardTotalNextAction(CardNumber.Five, i, HandAction.Stand);
                bsTree.AddHardTotalNextAction(CardNumber.Six, i, HandAction.Stand);
                bsTree.AddHardTotalNextAction(CardNumber.Seven, i, HandAction.Stand);
                bsTree.AddHardTotalNextAction(CardNumber.Eight, i, HandAction.Stand);
                bsTree.AddHardTotalNextAction(CardNumber.Nine, i, HandAction.Stand);
                bsTree.AddHardTotalNextAction(CardNumber.Ten, i, HandAction.Stand);
                bsTree.AddHardTotalNextAction(CardNumber.Ace, i, HandAction.Stand);
            }

            return bsTree;
        }
    }
}
