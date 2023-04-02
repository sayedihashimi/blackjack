using SayedHa.Blackjack.Shared.Blackjack.Exceptions;
using SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    public interface INextHandActionArray {
        HandAction GetHandAction(CardNumber dealerCard, CardNumber opCard1, CardNumber opCard2);
        HandAction GetHandAction(CardNumber dealerCard, CardNumber[] opCards);
        void SetHandAction(HandAction handAction, CardNumber dealerCard, CardNumber opCard1, CardNumber opCard2);
        void SetHandAction(HandAction handAction, CardNumber dealerCard, CardNumber[] opCards);
        void SetHandActionForHardTotal(HandAction handAction, CardNumber dealerCard, int hardTotalScore);
        void SetSplitForPair(bool split, CardNumber dealerCard, CardNumber pairCard);
        void SetHandActionForSoftTotal(HandAction handAction, CardNumber dealerCard, int softTotalScore);
        float? FitnessScore { get; set; }
        string? Name { get; set; }
    }

    public class NextHandActionArrayFactory {
        private static NextHandActionArrayFactory _instance = new NextHandActionArrayFactory();
        protected internal RandomHelper RandomHelper { get; set; } = new RandomHelper();
        
        public static NextHandActionArrayFactory GetInstance() {
            return _instance;
        }        

        public NextHandActionArray CreateRandomStrategy() {
            var nhaa = new NextHandActionArray();

            RandomHelper.RandomizeArray(nhaa.pairHandActionArray, 1, 2 + 1);
            RandomHelper.RandomizeArray(nhaa.softHandActionArray, 1, 3 + 1);
            RandomHelper.RandomizeArray(nhaa.hardTotalHandActionArray, 1, 3 + 1);

            //for(int i = 0; i < nhaa.pairHandActionArray.GetLength(0); i++) {
            //    for(int j = 0;j < nhaa.pairHandActionArray.GetLength(1); j++) {
            //        var pairValue = nhaa.pairHandActionArray[i, j] = RandomHelper.GetRandomIntBetween(1, 2 + 1);
            //    }
            //}

            //for(int i = 0; i < nhaa.softHandActionArray.GetLength(0); i++) {
            //    for(int j = 0; j < nhaa.softHandActionArray.GetLength(1); j++) {
            //        nhaa.softHandActionArray[i, j] = RandomHelper.GetRandomIntBetween(2, 9 + 1);
            //    }
            //}

            //for(int i = 0; i < nhaa.hardTotalHandActionArray.GetLength(0); i++) {
            //    for(int j = 0; j < nhaa.hardTotalHandActionArray.GetLength(1); j++) {
            //        nhaa.hardTotalHandActionArray[i, j] = RandomHelper.GetRandomIntBetween(3, 20 + 1);
            //    }
            //}

            return nhaa;
        }
        
        public IEnumerable<NextHandActionArray> CreateRandomStrategies(int numStrategies) {
            for(int i = 0; i<numStrategies; i++) {
                yield return CreateRandomStrategy();
            }
        }
    }
    public class NextHandActionArray : INextHandActionArray {
        // using int instead of bool because
        //  1. to be able to tell when the value is not set.
        //  2. to be more consistent with the other item here.
        protected internal int[,] pairHandActionArray = new int[10, 10];
        // soft totals are 2 - 9
        protected internal int[,] softHandActionArray = new int[10, 9];
        // hard totals 3 - 18. probably could actually be 5 - 18 but not currently
        protected internal int[,] hardTotalHandActionArray = new int[10,19];

        public string? Name { get; set; }
        public float? FitnessScore { get; set; }
        public void SetSplitForPair(bool split, CardNumber dealerCard, CardNumber pairCard) {
            pairHandActionArray[GetIntFor(dealerCard), GetIntFor(pairCard)] = GetIntFor(split);
        }

        public void SetHandAction(HandAction handAction, CardNumber dealerCard, CardNumber opCard1, CardNumber opCard2) {
            if(handAction == HandAction.Split) {
                // verify we have a pair otherwise it's not a valid action
                if (!opCard1.IsAPairWith(opCard2)) {
                    throw new UnexpectedValueException($"Expected a pair but received '{opCard1}' and '{opCard2}'");
                }
                SetSplitForPair(true, dealerCard, opCard1);
            }
            else if (!opCard1.IsAPairWith(opCard2) && opCard1.ContainsAnAce(opCard2)) {
                (CardNumber firstCardNumber, CardNumber secondCardNumber) = opCard1.Sort(opCard2);
                SetHandActionForSoftTotal(handAction, dealerCard, firstCardNumber.GetValues()[0]);
            }
            else {
                SetHandActionForHardTotal(handAction, dealerCard, CardNumberHelper.GetScoreTotal(opCard1, opCard2));
            }
        }
        public void SetHandAction(HandAction handAction, CardNumber dealerCard, CardNumber[] opCards) {
            Debug.Assert(opCards != null);
            if(opCards.Length < 2) {
                throw new UnexpectedValueException($"To add a hand action, two or more cards are required, number of cards: '{opCards.Length}'");
            }

            if(opCards.Length == 2) {
                SetHandAction(handAction, dealerCard, opCards[0], opCards[1]);
            }
            else {
                if(handAction == HandAction.Split) {
                    throw new UnexpectedValueException($"Cannot split when hand contains more than two cards, number of cards: '{opCards.Length}'");
                }
                var scoreTotal = CardNumberHelper.GetScoreTotal(opCards);
                SetHandActionForHardTotal(handAction, dealerCard, scoreTotal);
            }
        }
        public void SetHandActionForSoftTotal(HandAction handAction, CardNumber dealerCard, int softTotalScore) {
            softHandActionArray[GetIntFor(dealerCard), GetIntForSoftScore(softTotalScore)] = GetIntFor(handAction);
        }
        public void SetHandActionForHardTotal(HandAction handAction, CardNumber dealerCard, int hardTotalScore) {
            hardTotalHandActionArray[GetIntFor(dealerCard),GetIntForHardTotalScore(hardTotalScore)] = GetIntFor(handAction);
        }
        public HandAction GetHandAction(CardNumber dealerCard, CardNumber opCard1, CardNumber opCard2) {
            if (opCard1.IsAPairWith(opCard2)) {
                // see if the pair should be split if so, return it
                var valueFromPairArray = pairHandActionArray[GetIntFor(dealerCard), GetIntFor(opCard1)];
                bool? shouldSplit = ConvertBoolInt(valueFromPairArray);
                if(shouldSplit is not null && shouldSplit.HasValue && shouldSplit.Value == true) {
                    return HandAction.Split;
                }
            }
            // HandAction is not split here

            // see if the cards contains an ace, if so return from soft-totals
            if (!opCard1.IsAPairWith(opCard2) && opCard1.ContainsAnAce(opCard2)) {
                // return value from soft totals
                var nonAceCard = opCard1;
                if(nonAceCard == CardNumber.Ace) {
                    nonAceCard = opCard2;
                }

                var handAction = GetFromSoftTotals(dealerCard, nonAceCard);
                if(handAction is not null && handAction.HasValue) {
                    return handAction.Value;
                }
            }

            // return from hard totals
            var score = CardNumberHelper.GetScoreTotal(opCard1, opCard2);
            if(score >= BlackjackSettings.GetBlackjackSettings().MaxScore) {
                return HandAction.Stand;
            }
            // it should be in the array
            return GetHandActionFor(
                hardTotalHandActionArray[GetIntFor(dealerCard), GetIntForHardTotalScore(score)])!.Value;
        }
        /// <summary>
        /// Will return the value from the soft totals.
        /// </summary>
        /// <param name="dealerCard">The dealers card</param>
        /// <param name="opCard">The other card the player is holding beside the ace card.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected internal HandAction? GetFromSoftTotals(CardNumber dealerCard, CardNumber opCard) {
            var softValue = softHandActionArray[GetIntFor(dealerCard), GetIntFor(opCard)];
            var handAction = GetHandActionFor(softValue);
            if (handAction is not null && handAction.HasValue) {
                return handAction.Value;
            }
            return null;
        }
        protected internal HandAction? GetFromSoftTotals(CardNumber dealerCard, int scoreTotal) {
            int scoreIndex = GetIntForSoftScore(scoreTotal);
            var softValue = softHandActionArray[GetIntFor(dealerCard), scoreIndex];
            var handAction = GetHandActionFor(softValue);
            if (handAction is not null && handAction.HasValue) {
                return handAction.Value;
            }
            return null;
        }
        public HandAction GetHandAction(CardNumber dealerCard, params CardNumber[] opCards) {
            Debug.Assert(opCards != null);

            if(opCards.Length < 2) {
                throw new UnexpectedValueException($"Min cards 2, num cards: '{opCards.Length}'");
            }

            if (opCards.Length == 2) {
                return GetHandAction(dealerCard, opCards[0], opCards[1]);
            }

            var score = CardNumberHelper.GetScoreTotal(opCards);
            if(score >= BlackjackSettings.GetBlackjackSettings().MaxScore) {
                return HandAction.Stand;
            }

            var containsAce = CardNumberHelper.ContainsAnAce(opCards);
            if (containsAce) {
                var otherCards = new CardNumber[opCards.Length - 1];

                // only want to ignore 1 ace
                bool aceCardFound = false;
                int currentIndex = 0;
                foreach(var card in opCards) {
                    if(!aceCardFound && card == CardNumber.Ace) {
                        aceCardFound = true;
                    }
                    else {
                        otherCards[currentIndex++] = card;
                    }
                }
                var scoreOtherCards = CardNumberHelper.GetScoreTotal(otherCards);
                if(scoreOtherCards <= 9) {
                    // check soft totals for the result
                    var softHandAction = GetFromSoftTotals(dealerCard, scoreOtherCards);
                    if(softHandAction is not null && 
                        softHandAction.HasValue && 
                        softHandAction.Value != HandAction.Double) {
                        // double not allowed when there are more than 2 cards

                        return softHandAction.Value;
                    }
                }
            }

            // return the value from the hard totals
            var scoreTotal = CardNumberHelper.GetScoreTotal(opCards);
            var scoreHardTotalIndex = GetIntForHardTotalScore(scoreTotal);
            // it should be in the array
            var hardTotalValue = hardTotalHandActionArray[GetIntFor(dealerCard), scoreHardTotalIndex];
            return GetHandActionFor(hardTotalValue)!.Value;
        }
        public bool? ConvertBoolInt(int intValue) => intValue switch {
            1 => true,
            2 => false,
            _ => null
        };
        protected internal int GetIntFor(bool boolValue) => boolValue switch {
            // not using 0 becuase that's the default value that the array is initilized with
            true => 1,
            false => 2
        };
        protected internal int GetIntFor(CardNumber cardNumber) => cardNumber switch {
            CardNumber.Two => 0,
            CardNumber.Three => 1,
            CardNumber.Four => 2,
            CardNumber.Five => 3,
            CardNumber.Six => 4,
            CardNumber.Seven => 5,
            CardNumber.Eight => 6,
            CardNumber.Nine => 7,
            CardNumber.Ten => 8,
            CardNumber.Jack => 8,
            CardNumber.Queen => 8,
            CardNumber.King => 8,
            CardNumber.Ace => 9,
            _ => throw new UnknownValueException($"Unknown value for CardNumber: '{cardNumber}'")
        };
        protected internal int GetIntForSoftScore(int softScore) => softScore switch {
            2 => 0, 
            3 => 1, 
            4 => 2, 
            5 => 3, 
            6 => 4, 
            7 => 5, 
            8 => 6, 
            9 => 7,
            _ => throw new UnexpectedNodeTypeException($"Unexpected value for soft total: '{softScore}'")
        };
        protected internal int GetIntForHardTotalScore(int hardTotalScore) => hardTotalScore switch {
            3 => 0,
            4 => 1,
            5 => 2,
            6 => 3,
            7 => 4,
            8 => 5,
            9 => 6,
            10 => 7,
            11 => 8,
            12 => 9,
            13 => 10,
            14 => 11,
            15 => 12,
            16 => 13,
            17 => 14,
            18 => 16,
            19 => 17,
            20 => 18,
            _ => throw new UnexpectedNodeTypeException($"Unexpected value for hard total: '{hardTotalScore}'")
        };
        protected internal int GetIntFor(HandAction handAction) => handAction switch {
            HandAction.Double => 1,
            HandAction.Hit => 2,
            HandAction.Stand => 3,
            HandAction.Split => 4,
            _ => throw new UnexpectedNodeTypeException($"Unexpected value for HandAction: '{handAction}'")
        };
        protected internal HandAction? GetHandActionFor(int handActionInt) => handActionInt switch {
            0 => null,
            1 => HandAction.Double,
            2 => HandAction.Hit,
            3 => HandAction.Stand,
            4 => HandAction.Split,
            _ => throw new UnexpectedNodeTypeException($"Unexpected value for handActionInt: '{handActionInt}'")
        };
        public static Comparison<NextHandActionArray> NextHandActionArrayComparison { get; } = (strategy1, strategy2) => (strategy1.FitnessScore, strategy2.FitnessScore) switch {
            (null, null) => 0,
            (not null, null) => -1,
            (null, not null) => 1,
            (_, _) => -1 * strategy1.FitnessScore.Value.CompareTo(strategy2.FitnessScore.Value),
        };
    }
}
