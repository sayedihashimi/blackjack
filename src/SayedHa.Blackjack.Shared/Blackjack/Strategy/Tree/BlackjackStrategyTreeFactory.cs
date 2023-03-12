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
            int numPairsToAdd = GetRandomIntBetween(1, 13 + 1);
            for(int i = 0; i < numPairsToAdd; i++) {
                tree.pairsToSplit.Add(numPairsToAdd);
            }
        }
        protected internal void AddRandomSoftTotals(BlackjackStrategyTree tree) {
            bool includeDouble = true;
            foreach (var cardNumber in _allCardNumbers) {
                for(var score = 2; score <= 9; score++) {
                    tree.AddSoftTotalNextAction(cardNumber, score, GetRandomHandAction(_doubleEnabled));
                }
            }
        }
        protected internal void AddRandomHardTotals(BlackjackStrategyTree tree) {
            foreach(var dealerCardNumber in _allCardNumbers) {
                for(int score = 5;score <= 17; score++) {
                    tree.AddHardTotalNextAction(dealerCardNumber, score,GetRandomHandAction(_doubleEnabled));
                }
            }
        }

        private HandAction GetRandomHandAction(bool includeDouble) {
            int fromInclusive = 0;
            int toInclusive = includeDouble ? 2 : 1;
            int indexToReturn = GetRandomIntBetween(fromInclusive, toInclusive + 1);
            return _allHandActions[indexToReturn];
        }

        protected internal int GetRandomIntBetween(int fromInclusive, int toExclusive) => _useRandomNumberGenerator switch {
            true => RandomNumberGenerator.GetInt32(fromInclusive, toExclusive),
            false => random.Next(fromInclusive, toExclusive)
        };
        
    }
}
