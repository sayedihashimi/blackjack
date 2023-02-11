// This file is part of SayedHa.Blackjack.
//
// SayedHa.Blackjack is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SayedHa.Blackjack is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with SayedHa.Blackjack.  If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Players {
    // assumes that Double down is available
    // assumes that Double After Split is available
    // assumes that Surrender is not available
    public class BasicStrategyPlayer : Player {
        public BasicStrategyPlayer(ILogger logger) {
            _logger = logger;
        }
        private ILogger _logger = new NullLogger();
        public override HandAction GetNextAction(Hand hand, DealerHand dealerHand) {
            bool isDoubleEnabled = BlackjackSettings.GetBlackjackSettings().DoubleDownEnabled;
            bool isSplitEnabled = BlackjackSettings.GetBlackjackSettings().SplitEnabled;
            // if only two cards, first check to see if the action should be split
            if(isSplitEnabled && hand.DealtCards.Count == 2) {
                if (ShouldSplitWith(hand.DealtCards[0].Number, hand.DealtCards[1].Number, dealerHand.DealersVisibleCard!.Number)) {
                    return HandAction.Split;
                }
            }
            // if we get to this point the action is not split

            // handle cases where there is an Ace
            // if only two cards, check to see if one of the two cards is an Ace
            // if one of the two cards is an Ace return the next action
            if (hand.DealtCards.Count == 2) {
                var nextActionIfHasAce = IfContainsAceReturnNextAction(hand.DealtCards[0], hand.DealtCards[1], dealerHand.DealersVisibleCard!);
                
                if (nextActionIfHasAce.hasAce) {
                    Debug.Assert(nextActionIfHasAce.nextAction != HandAction.Split);
                    return nextActionIfHasAce.nextAction;
                }
            }

            // now we handle the generic case
            int handScore = hand.GetScore();

            var nextHandAction = (handScore, dealerHand.DealersVisibleCard!.Number) switch {
                ( >= 17, _) => HandAction.Stand,
                (16, CardNumber.Two) => HandAction.Stand,
                (16, CardNumber.Three) => HandAction.Stand,
                (16, CardNumber.Four) => HandAction.Stand,
                (16, CardNumber.Five) => HandAction.Stand,
                (16, CardNumber.Six) => HandAction.Stand,
                (16, _) => HandAction.Hit,

                (15, CardNumber.Two) => HandAction.Stand,
                (15, CardNumber.Three) => HandAction.Stand,
                (15, CardNumber.Four) => HandAction.Stand,
                (15, CardNumber.Five) => HandAction.Stand,
                (15, CardNumber.Six) => HandAction.Stand,
                (15, _) => HandAction.Hit,

                (14, CardNumber.Two) => HandAction.Stand,
                (14, CardNumber.Three) => HandAction.Stand,
                (14, CardNumber.Four) => HandAction.Stand,
                (14, CardNumber.Five) => HandAction.Stand,
                (14, CardNumber.Six) => HandAction.Stand,
                (14, _) => HandAction.Hit,

                (13, CardNumber.Two) => HandAction.Stand,
                (13, CardNumber.Three) => HandAction.Stand,
                (13, CardNumber.Four) => HandAction.Stand,
                (13, CardNumber.Five) => HandAction.Stand,
                (13, CardNumber.Six) => HandAction.Stand,
                (13, _) => HandAction.Hit,

                (12, CardNumber.Four) => HandAction.Stand,
                (12, CardNumber.Five) => HandAction.Stand,
                (12, CardNumber.Six) => HandAction.Stand,
                (12, _) => HandAction.Hit,

                // "Always double down on 11, always"
                (11, _) => isDoubleEnabled ? HandAction.Double : HandAction.Hit,

                (10, CardNumber.Ten) => HandAction.Hit,
                (10, CardNumber.Jack) => HandAction.Hit,
                (10, CardNumber.Queen) => HandAction.Hit,
                (10, CardNumber.King) => HandAction.Hit,
                (10, CardNumber.Ace) => HandAction.Hit,
                (10, _) => isDoubleEnabled ? HandAction.Double : HandAction.Hit,

                (9, CardNumber.Three) => isDoubleEnabled ? HandAction.Double : HandAction.Hit,
                (9, CardNumber.Four) => isDoubleEnabled ? HandAction.Double : HandAction.Hit,
                (9, CardNumber.Five) => isDoubleEnabled ? HandAction.Double : HandAction.Hit,
                (9, CardNumber.Six) => isDoubleEnabled ? HandAction.Double : HandAction.Hit,

                ( <= 8, _) => HandAction.Hit,

                // shouldn't get here
                (_, _) => throw new ApplicationException("Error in GetNextAction")
            };

            return nextHandAction;
        }
        protected bool ShouldSplitWith(CardNumber card1, CardNumber card2,CardNumber dealerCard) {
            switch (card1, card2, dealerCard) {
                case (CardNumber.Ace, CardNumber.Ace, _):
                    return true;

                case (CardNumber.Nine, CardNumber.Nine, CardNumber.Two):
                case (CardNumber.Nine, CardNumber.Nine, CardNumber.Three):
                case (CardNumber.Nine, CardNumber.Nine, CardNumber.Four):
                case (CardNumber.Nine, CardNumber.Nine, CardNumber.Five):
                case (CardNumber.Nine, CardNumber.Nine, CardNumber.Six):
                // skip seven
                case (CardNumber.Nine, CardNumber.Nine, CardNumber.Eight):
                case (CardNumber.Nine, CardNumber.Nine, CardNumber.Nine):
                    return true;

                case (CardNumber.Eight, CardNumber.Eight, _):
                    return true;

                case (CardNumber.Seven, CardNumber.Seven, CardNumber.Two):
                case (CardNumber.Seven, CardNumber.Seven, CardNumber.Three):
                case (CardNumber.Seven, CardNumber.Seven, CardNumber.Four):
                case (CardNumber.Seven, CardNumber.Seven, CardNumber.Five):
                case (CardNumber.Seven, CardNumber.Seven, CardNumber.Six):
                case (CardNumber.Seven, CardNumber.Seven, CardNumber.Seven):
                    return true;

                case (CardNumber.Six, CardNumber.Six, CardNumber.Two):
                case (CardNumber.Six, CardNumber.Six, CardNumber.Three):
                case (CardNumber.Six, CardNumber.Six, CardNumber.Four):
                case (CardNumber.Six, CardNumber.Six, CardNumber.Five):
                case (CardNumber.Six, CardNumber.Six, CardNumber.Six):
                    return true;

                case (CardNumber.Four, CardNumber.Four, CardNumber.Five):
                case (CardNumber.Four, CardNumber.Four, CardNumber.Six):
                    return true;

                case (CardNumber.Three, CardNumber.Three, CardNumber.Two):
                case (CardNumber.Three, CardNumber.Three, CardNumber.Three):
                case (CardNumber.Three, CardNumber.Three, CardNumber.Four):
                case (CardNumber.Three, CardNumber.Three, CardNumber.Five):
                case (CardNumber.Three, CardNumber.Three, CardNumber.Six):
                case (CardNumber.Three, CardNumber.Three, CardNumber.Seven):
                    return true;

                case (CardNumber.Two, CardNumber.Two, CardNumber.Two):
                case (CardNumber.Two, CardNumber.Two, CardNumber.Three):
                case (CardNumber.Two, CardNumber.Two, CardNumber.Four):
                case (CardNumber.Two, CardNumber.Two, CardNumber.Five):
                case (CardNumber.Two, CardNumber.Two, CardNumber.Six):
                case (CardNumber.Two, CardNumber.Two, CardNumber.Seven):
                    return true;

                default:
                    break;
            }

            return false;
        }

        protected (bool hasAce, Card? otherCard) IfContainsAceReturnOtherCard(Card card1, Card card2) {
            Debug.Assert(card1 != null);
            Debug.Assert(card2 != null);

            if (card1.Number == CardNumber.Ace) {
                return (true, card2);
            }
            else if (card2.Number == CardNumber.Ace) {
                return (true, card1);
            }

            return (false, null);
        }
        protected (bool hasAce, HandAction nextAction) IfContainsAceReturnNextAction(Card card1, Card card2, Card dealerVisibleCard) {
            var cardsContainAce = IfContainsAceReturnOtherCard(card1, card2);
            var isDoubleEnabled = BlackjackSettings.GetBlackjackSettings().DoubleDownEnabled;
            var isSplitEnabled = BlackjackSettings.GetBlackjackSettings().SplitEnabled;
            if (cardsContainAce.hasAce) {
                var otherCard = cardsContainAce.otherCard!;
                switch (otherCard.Number, dealerVisibleCard.Number) {
                    // stand when Ace + Nine or higher (no need to list Ace because that's a split which is taken care of already)
                    case (CardNumber.Nine, _):
                    case (CardNumber.Ten, _):
                    case (CardNumber.Jack, _):
                    case (CardNumber.Queen, _):
                    case (CardNumber.King, _):
                        return (true, HandAction.Stand);

                    // stand when Ace + 8 execpt if the dealer has 6, then double down
                    case (CardNumber.Eight, CardNumber.Two):
                    case (CardNumber.Eight, CardNumber.Three):
                    case (CardNumber.Eight, CardNumber.Four):
                    case (CardNumber.Eight, CardNumber.Five):
                    case (CardNumber.Eight, CardNumber.Seven):
                    case (CardNumber.Eight, CardNumber.Eight):
                    case (CardNumber.Eight, CardNumber.Nine):
                    case (CardNumber.Eight, CardNumber.Ten):
                    case (CardNumber.Eight, CardNumber.Jack):
                    case (CardNumber.Eight, CardNumber.Queen):
                    case (CardNumber.Eight, CardNumber.King):
                    case (CardNumber.Eight, CardNumber.Ace):
                        return (true, HandAction.Stand);
                    case (CardNumber.Eight, CardNumber.Six):
                        return (true, isDoubleEnabled ? HandAction.Double : HandAction.Hit);

                    // Ace + 7:
                    //  Double 2 - 6, stand otherwise
                    case (CardNumber.Seven, CardNumber.Two):
                    case (CardNumber.Seven, CardNumber.Three):
                    case (CardNumber.Seven, CardNumber.Four):
                    case (CardNumber.Seven, CardNumber.Five):
                    case (CardNumber.Seven, CardNumber.Six):
                        return (true, isDoubleEnabled ? HandAction.Double : HandAction.Hit);
                    case (CardNumber.Seven, _):
                        return (true, HandAction.Stand);

                    // Ace + 6
                    //  Double when 3-6, otherwise Hit
                    case (CardNumber.Six, CardNumber.Three):
                    case (CardNumber.Six, CardNumber.Four):
                    case (CardNumber.Six, CardNumber.Five):
                    case (CardNumber.Six, CardNumber.Six):
                        return (true, isDoubleEnabled ? HandAction.Double : HandAction.Hit);
                    case (CardNumber.Six, _):
                        return (true, HandAction.Hit);

                    // Ace + 5
                    //  Double when 4 - 6, otherwise Hit
                    case (CardNumber.Five, CardNumber.Four):
                    case (CardNumber.Five, CardNumber.Five):
                    case (CardNumber.Five, CardNumber.Six):
                        return (true, isDoubleEnabled ? HandAction.Double : HandAction.Hit);
                    case (CardNumber.Five, _):
                        return (true, HandAction.Hit);

                    // Ace + 4
                    //  Double when 4 - 6, otherwise Hit
                    case (CardNumber.Four, CardNumber.Four):
                    case (CardNumber.Four, CardNumber.Five):
                    case (CardNumber.Four, CardNumber.Six):
                        return (true, isDoubleEnabled ? HandAction.Double : HandAction.Hit);
                    case (CardNumber.Four, _):
                        return (true, HandAction.Hit);

                    // Ace + 3
                    //  Double if 5,6 otherwise Hit
                    case (CardNumber.Three, CardNumber.Five):
                    case (CardNumber.Three, CardNumber.Six):
                        return (true, isDoubleEnabled ? HandAction.Double : HandAction.Hit);
                    case (CardNumber.Three, _):
                        return (true, HandAction.Hit);

                    // Ace + 2
                    //  Double if 5,6 otherwise Hit
                    case (CardNumber.Two, CardNumber.Five):
                    case (CardNumber.Two, CardNumber.Six):
                        return (true, isDoubleEnabled ? HandAction.Double : HandAction.Hit);
                    case (CardNumber.Two, _):
                        return (true, HandAction.Hit);



                    default:
                        break;
                }                
            }
            return (false, isDoubleEnabled ? HandAction.Split : HandAction.Hit);
        }
    }
}
