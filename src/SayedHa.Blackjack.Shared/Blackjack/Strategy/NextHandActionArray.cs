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
    public class NextHandActionArray : INextHandActionArray {
        // using int instead of bool because
        //  1. to be able to tell when the value is not set.
        //  2. to be more consistent with the other item here.
        protected internal int[,] pairHandActionArray = new int[10, 10];
        // soft totals are 2 - 9
        protected internal int[,] softHandActionArray = new int[10, 8];
        // hard totals 3 - 18. probably could actually be 5 - 18 but not currently
        protected internal int[,] hardTotalHandActionArray = new int[10,18];

        protected internal NextHandActionConverter Converter = NextHandActionConverter.Instance;
        public string? Name { get; set; }
        public float? FitnessScore { get; set; }

        public NextHandActionArray Clone() {
            NextHandActionArray clone = new NextHandActionArray();

            // don't clone the fitnessscore
            clone.Name = Name;

            for(var i = 0; i < pairHandActionArray.GetLength(0); i++) {
                for(var j = 0;j< pairHandActionArray.GetLength(1); j++) {
                    clone.pairHandActionArray[i, j] = pairHandActionArray[i,j];
                }
            }
            for (var i = 0; i < softHandActionArray.GetLength(0); i++) {
                for (var j = 0; j < softHandActionArray.GetLength(1); j++) {
                    clone.softHandActionArray[i, j] = softHandActionArray[i, j];
                }
            }
            for (var i = 0; i < hardTotalHandActionArray.GetLength(0); i++) {
                for (var j = 0; j < hardTotalHandActionArray.GetLength(1); j++) {
                    clone.hardTotalHandActionArray[i, j] = hardTotalHandActionArray[i, j];
                }
            }

            return clone;
        }


        public void SetSplitForPair(bool split, CardNumber dealerCard, CardNumber pairCard) {
            pairHandActionArray[Converter.GetIntFor(dealerCard), Converter.GetIntFor(pairCard)] = Converter.GetIntFor(split);
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
            softHandActionArray[
                Converter.GetIntFor(dealerCard),
                Converter.GetIntForSoftScore(softTotalScore)] = Converter.GetIntFor(handAction);
        }
        public void SetHandActionForHardTotal(HandAction handAction, CardNumber dealerCard, int hardTotalScore) {
            hardTotalHandActionArray[
                Converter.GetIntFor(dealerCard),
                Converter.GetIntHardTotalCellValueFromScore(hardTotalScore)] = Converter.GetIntFor(handAction);
        }
        public HandAction GetHandAction(CardNumber dealerCard, CardNumber opCard1, CardNumber opCard2) {
            if (opCard1.IsAPairWith(opCard2)) {
                // see if the pair should be split if so, return it
                var valueFromPairArray = pairHandActionArray[
                    Converter.GetIntFor(dealerCard),
                    Converter.GetIntFor(opCard1)];
                bool? shouldSplit = Converter.ConvertBoolIndex(valueFromPairArray);
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
            return NextHandActionConverter.Instance.GetHandActionFor(
                hardTotalHandActionArray[Converter.GetIntFor(dealerCard), Converter.GetIntHardTotalCellValueFromScore(score)])!.Value;
        }
        /// <summary>
        /// Will return the value from the soft totals.
        /// </summary>
        /// <param name="dealerCard">The dealers card</param>
        /// <param name="opCard">The other card the player is holding beside the ace card.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected internal HandAction? GetFromSoftTotals(CardNumber dealerCard, CardNumber opCard) {
            var softValue = softHandActionArray[Converter.GetIntFor(dealerCard), Converter.GetIntFor(opCard)];
            var handAction = NextHandActionConverter.Instance.GetHandActionFor(softValue);
            if (handAction is not null && handAction.HasValue) {
                return handAction.Value;
            }
            return null;
        }
        protected internal HandAction? GetFromSoftTotals(CardNumber dealerCard, int scoreTotal) {
            int scoreIndex = Converter.GetIntForSoftScore(scoreTotal);
            var softValue = softHandActionArray[Converter.GetIntFor(dealerCard), scoreIndex];
            var handAction = Converter.GetHandActionFor(softValue);
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
            var scoreHardTotalIndex = Converter.GetIntHardTotalCellValueFromScore(scoreTotal);
            // it should be in the array
            var hardTotalValue = hardTotalHandActionArray[Converter.GetIntFor(dealerCard), scoreHardTotalIndex];
            return Converter.GetHandActionFor(hardTotalValue)!.Value;
        }
        
        
        public static Comparison<NextHandActionArray> NextHandActionArrayComparison { get; } = (strategy1, strategy2) => (strategy1.FitnessScore, strategy2.FitnessScore) switch {
            (null, null) => 0,
            (not null, null) => -1,
            (null, not null) => 1,
            (_, _) => -1 * strategy1.FitnessScore.Value.CompareTo(strategy2.FitnessScore.Value),
        };
        private readonly List<string> _columnHeaders = new List<string> {"2","3","4","5","6","7","8","9","10","A"};

        public void WriteTo(StringWriter sWriter) {
            WriteTo(sWriter, 4);
        }
        public void WriteTo(StringWriter writer, int columnWidth) {
            // write hard totals
            writer.WriteLine("hard-totals");
            writer.Write(new string(' ', columnWidth));
            foreach (var column in _columnHeaders) {
                writer.Write(column.PadLeft(columnWidth));
            }
            writer.WriteLine();

            for(int hardTotalValueIndex = 0; hardTotalValueIndex < hardTotalHandActionArray.GetLength(1); hardTotalValueIndex++) {
                var hardTotal = Converter.GetHardTotalScoreFromIndex(hardTotalValueIndex);
                writer.Write(hardTotal.ToString().PadLeft(columnWidth));
                for(int dealerIndex = 0; dealerIndex < hardTotalHandActionArray.GetLength(0);dealerIndex++) {
                    writer.Write(Converter.GetCharForHandActionIndex(hardTotalHandActionArray[dealerIndex, hardTotalValueIndex]).ToString()!.PadLeft(columnWidth));
                }
                writer.WriteLine();
            }

            // write soft totals
            writer.WriteLine("soft-totals");
            writer.Write(new string(' ', columnWidth));

            foreach (var column in _columnHeaders) {
                writer.Write(column.PadLeft(columnWidth));
            }
            writer.WriteLine();
            for (int softTotalIndex = 0; softTotalIndex < softHandActionArray.GetLength(1); softTotalIndex++) {
                var softTotal = Converter.GetSoftTotalScoreFromIndex(softTotalIndex);
                writer.Write($"A-{softTotal.ToString()}".PadLeft(columnWidth));
                for (int dealerIndex = 0; dealerIndex < softHandActionArray.GetLength(0); dealerIndex++) {
                    writer.Write(Converter.GetCharForHandActionIndex(softHandActionArray[dealerIndex, softTotalIndex]).ToString()!.PadLeft(columnWidth));
                }
                writer.WriteLine();
            }

            // write splits
            writer.WriteLine("splits");
            writer.Write(new string(' ', columnWidth));

            foreach (var column in _columnHeaders) {
                writer.Write(column.PadLeft(columnWidth));
            }
            writer.WriteLine();

            for (int pairIndex = 0; pairIndex < pairHandActionArray.GetLength(1); pairIndex++) {
                writer.Write(Converter.GetSplitCharForIndex(pairIndex).PadLeft(columnWidth));
                for (int dealerIndex = 0; dealerIndex < pairHandActionArray.GetLength(0); dealerIndex++) {
                    writer.Write(Converter.GetCharForBoolIndex(pairHandActionArray[dealerIndex, pairIndex]).ToString()!.PadLeft(columnWidth));
                }
                writer.WriteLine();
            }

            writer.Flush();
        }
    }
}
