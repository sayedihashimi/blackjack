using SayedHa.Blackjack.Shared.Blackjack.Exceptions;
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
        void SetHandActionForHardTotal(HandAction handAction, CardNumber dealerCard, int hardTotalScore);
        void SetHandActionForPair(HandAction handAction, CardNumber dealerCard, CardNumber pairCard);
        void SetHandActionForSoftTotal(HandAction handAction, CardNumber dealerCard, int softTotalScore);
    }

    public class NextHandActionArray : INextHandActionArray {
        protected internal int[,] pairHandActionArray = new int[10, 10];
        // soft totals are 2 - 9
        protected internal int[,] softHandActionArray = new int[10, 8];
        // hard totals 3 - 18. probably could actually be 5 - 18 but not currently
        protected internal int[,] hardTotalHandActionArray = new int[10,18];

        public void SetHandActionForPair(HandAction handAction, CardNumber dealerCard, CardNumber pairCard) {
            pairHandActionArray[GetIntFor(dealerCard), GetIntFor(pairCard)] = GetIntFor(handAction);
        }
        public void SetHandActionForSoftTotal(HandAction handAction, CardNumber dealerCard, int softTotalScore) {
            softHandActionArray[GetIntFor(dealerCard), GetIntForSoftScore(softTotalScore)] = GetIntFor(handAction);
        }
        public void SetHandActionForHardTotal(HandAction handAction, CardNumber dealerCard, int hardTotalScore) {
            hardTotalHandActionArray[GetIntFor(dealerCard),GetIntForHardTotalScore(hardTotalScore)] = GetIntFor(handAction);
        }
        public HandAction GetHandAction(CardNumber dealerCard, CardNumber opCard1, CardNumber opCard2) {
            if (opCard1.IsAPairWith(opCard2)) {
                // return value from pair array
                return GetHandActionFor(pairHandActionArray[GetIntFor(dealerCard), GetIntFor(opCard1)]);
            }
            // see if the cards contains an ace
            if (opCard1.ContainsAnAce(opCard2)) {
                // return value from soft totals
                var nonAceCard = opCard1;
                if(nonAceCard == CardNumber.Ace) {
                    nonAceCard = opCard2;
                }
                return GetHandActionFor(hardTotalHandActionArray[GetIntFor(dealerCard),GetIntFor(nonAceCard)]);
            }
            // return from hard totals


            throw new NotImplementedException();
        }
        public HandAction GetHandAction(CardNumber dealerCard, CardNumber[] opCards) {
            Debug.Assert(opCards != null);

            if (opCards.Length == 2) {
                return GetHandAction(dealerCard, opCards[0], opCards[1]);
            }

            throw new NotImplementedException();
        }

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
            HandAction.Double => 0,
            HandAction.Hit => 1,
            HandAction.Stand => 2,
            HandAction.Split => 3,
            _ => throw new UnexpectedNodeTypeException($"Unexpected value for HandAction: '{handAction}'")
        };
        protected internal HandAction GetHandActionFor(int handActionInt) => handActionInt switch {
            0 => HandAction.Double,
            1 => HandAction.Hit,
            2 => HandAction.Stand,
            3 => HandAction.Split,
            _ => throw new UnexpectedNodeTypeException($"Unexpected value for handActionInt: '{handActionInt}'")
        };
    }
}
