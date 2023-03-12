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
        public override HandActionAndReason GetNextAction(Hand hand, DealerHand dealerHand, int dollarsRemaining) {
            bool isDoubleEnabled = BlackjackSettings.GetBlackjackSettings().DoubleDownEnabled;
            bool isSplitEnabled = BlackjackSettings.GetBlackjackSettings().SplitEnabled;
            // if only two cards, first check to see if the action should be split
            if (isSplitEnabled && hand.DealtCards.Count == 2) {
                var splitResult = ShouldSplitWith(hand.DealtCards[0].Number, hand.DealtCards[1].Number, dealerHand.DealersVisibleCard!.Number);
                if (splitResult != null) {
                    return splitResult!;
                }
            }
            // if we get to this point the action is not split

            // handle cases where there is an Ace
            // if only two cards, check to see if one of the two cards is an Ace
            // if one of the two cards is an Ace return the next action
            if (hand.DealtCards.Count == 2) {
                var nextActionIfHasAce = IfContainsAceReturnNextAction(hand.DealtCards[0], hand.DealtCards[1], dealerHand.DealersVisibleCard!);

                if (nextActionIfHasAce.hasAce) {
                    Debug.Assert(nextActionIfHasAce.nextAction.HandAction != HandAction.Split);
                    return nextActionIfHasAce.nextAction;
                }
            }

            // now we handle the generic case
            int handScore = hand.GetScore();

            // TODO: There is a bug here that this may return double even if more cards have already been dealt
            var nextHandAction = (handScore, dealerHand.DealersVisibleCard!.Number) switch {
                ( >= 17, _) => new HandActionAndReason(HandAction.Stand, "Always stand on 17 or above."),
                (16, CardNumber.Two) => new HandActionAndReason(HandAction.Stand, "Stand on 16 when dealer shows 6 or below."),
                (16, CardNumber.Three) => new HandActionAndReason(HandAction.Stand, "Stand on 16 when dealer shows 6 or below."),
                (16, CardNumber.Four) => new HandActionAndReason(HandAction.Stand, "Stand on 16 when dealer shows 6 or below."),
                (16, CardNumber.Five) => new HandActionAndReason(HandAction.Stand, "Stand on 16 when dealer shows 6 or below."),
                (16, CardNumber.Six) => new HandActionAndReason(HandAction.Stand, "Stand on 16 when dealer shows 6 or below."),
                (16, _) => new HandActionAndReason(HandAction.Hit, "Hit on 16 when dealer shows 7 or above."),

                (15, CardNumber.Two) => new HandActionAndReason(HandAction.Stand, "Stand on 15 when dealer shows 6 or below."),
                (15, CardNumber.Three) => new HandActionAndReason(HandAction.Stand, "Stand on 15 when dealer shows 6 or below."),
                (15, CardNumber.Four) => new HandActionAndReason(HandAction.Stand, "Stand on 15 when dealer shows 6 or below."),
                (15, CardNumber.Five) => new HandActionAndReason(HandAction.Stand, "Stand on 15 when dealer shows 6 or below."),
                (15, CardNumber.Six) => new HandActionAndReason(HandAction.Stand, "Stand on 15 when dealer shows 6 or below."),
                (15, _) => new HandActionAndReason(HandAction.Hit, "Hit on 15 when dealer shows 7 or above."),

                (14, CardNumber.Two) => new HandActionAndReason(HandAction.Stand, "Stand on 14 when dealer shows 6 or below."),
                (14, CardNumber.Three) => new HandActionAndReason(HandAction.Stand, "Stand on 14 when dealer shows 6 or below."),
                (14, CardNumber.Four) => new HandActionAndReason(HandAction.Stand, "Stand on 14 when dealer shows 6 or below."),
                (14, CardNumber.Five) => new HandActionAndReason(HandAction.Stand, "Stand on 14 when dealer shows 6 or below."),
                (14, CardNumber.Six) => new HandActionAndReason(HandAction.Stand, "Stand on 14 when dealer shows 6 or below."),
                (14, _) => new HandActionAndReason(HandAction.Hit, "Hit on 14 when dealer shows 7 or above."),

                (13, CardNumber.Two) => new HandActionAndReason(HandAction.Stand, "Stand on 13 when dealer shows 6 or below."),
                (13, CardNumber.Three) => new HandActionAndReason(HandAction.Stand, "Stand on 13 when dealer shows 6 or below."),
                (13, CardNumber.Four) => new HandActionAndReason(HandAction.Stand, "Stand on 13 when dealer shows 6 or below."),
                (13, CardNumber.Five) => new HandActionAndReason(HandAction.Stand, "Stand on 13 when dealer shows 6 or below."),
                (13, CardNumber.Six) => new HandActionAndReason(HandAction.Stand, "Stand on 13 when dealer shows 6 or below."),
                (13, _) => new HandActionAndReason(HandAction.Hit, "Hit on 13 when dealer shows 7 or above."),

                (12, CardNumber.Four) => new HandActionAndReason(HandAction.Stand, "Stand on 12 when dealer shows 4,5 or 6."),
                (12, CardNumber.Five) => new HandActionAndReason(HandAction.Stand, "Stand on 12 when dealer shows 4,5 or 6."),
                (12, CardNumber.Six) => new HandActionAndReason(HandAction.Stand, "Stand on 12 when dealer shows 4,5 or 6."),
                (12, _) => new HandActionAndReason(HandAction.Hit, "Hit on 12 unless dealer shows 4,5 or 6 when you stand."),

                // "Always double down on 11, always"
                (11, _) => isDoubleEnabled ?
                new HandActionAndReason(HandAction.Double, "Always double down on 11, always") :
                new HandActionAndReason(HandAction.Hit, "Hit since double not available. Always double down on 11, always"),

                (10, CardNumber.Ten) => new HandActionAndReason(HandAction.Hit, "Hit on 10 when dealer shows 10 or Ace, otherwise double down."),
                (10, CardNumber.Jack) => new HandActionAndReason(HandAction.Hit, "Hit on 10 when dealer shows 10 or Ace, otherwise double down."),
                (10, CardNumber.Queen) => new HandActionAndReason(HandAction.Hit, "Hit on 10 when dealer shows 10 or Ace, otherwise double down."),
                (10, CardNumber.King) => new HandActionAndReason(HandAction.Hit, "Hit on 10 when dealer shows 10 or Ace, otherwise double down."),
                (10, CardNumber.Ace) => new HandActionAndReason(HandAction.Hit, "Hit on 10 when dealer shows 10 or Ace, otherwise double down.Hit on 10 when dealer shows 10 or Ace, otherwise double down."),
                (10, _) => isDoubleEnabled ?
                        new HandActionAndReason(HandAction.Double, "Double on 10 unless dealer shows 10 or Ace when you hit.") :
                        new HandActionAndReason(HandAction.Hit, "Hit since double not available. Double on 10 unless dealer shows 10 or Ace when you hit."),

                (9, CardNumber.Three) => isDoubleEnabled ?
                        new HandActionAndReason(HandAction.Double, "Double on 9 when dealer shows 3, 4, 5 or 6.") :
                        new HandActionAndReason(HandAction.Hit, "Hit since double not available. Double on 9 when dealer shows 3, 4, 5 or 6."),
                (9, CardNumber.Four) => isDoubleEnabled ?
                        new HandActionAndReason(HandAction.Double, "Double on 9 when dealer shows 3, 4, 5 or 6.") :
                        new HandActionAndReason(HandAction.Hit, "Hit since double not available. Double on 9 when dealer shows 3, 4, 5 or 6."),
                (9, CardNumber.Five) => isDoubleEnabled ?
                        new HandActionAndReason(HandAction.Double, "Double on 9 when dealer shows 3, 4, 5 or 6.") :
                        new HandActionAndReason(HandAction.Hit, "Hit since double not available. Double on 9 when dealer shows 3, 4, 5 or 6."),
                (9, CardNumber.Six) => isDoubleEnabled ?
                        new HandActionAndReason(HandAction.Double, "Double on 9 when dealer shows 3, 4, 5 or 6.") :
                        new HandActionAndReason(HandAction.Hit, "Hit since double not available. Double on 9 when dealer shows 3, 4, 5 or 6."),
                (9, _) => new HandActionAndReason(HandAction.Hit, "Hit on 9, double when dealer shows 3, 4, 5 or 6."),

                ( <= 8, _) => new HandActionAndReason(HandAction.Hit, "Always hit on 8 or less.")
            };

            return nextHandAction;
        }
        protected HandActionAndReason? ShouldSplitWith(CardNumber card1, CardNumber card2, CardNumber dealerCard) {
            switch (card1, card2, dealerCard) {
                case (CardNumber.Ace, CardNumber.Ace, _):
                    return new HandActionAndReason(HandAction.Split,"Always split on Ace Ace");

                case (CardNumber.Nine, CardNumber.Nine, CardNumber.Two):
                case (CardNumber.Nine, CardNumber.Nine, CardNumber.Three):
                case (CardNumber.Nine, CardNumber.Nine, CardNumber.Four):
                case (CardNumber.Nine, CardNumber.Nine, CardNumber.Five):
                case (CardNumber.Nine, CardNumber.Nine, CardNumber.Six):
                // skip seven
                case (CardNumber.Nine, CardNumber.Nine, CardNumber.Eight):
                case (CardNumber.Nine, CardNumber.Nine, CardNumber.Nine):
                    return new HandActionAndReason(HandAction.Split, "Split on 9 9 except when dealer shows 7, 10 or Ace.");

                case (CardNumber.Eight, CardNumber.Eight, _):
                    return new HandActionAndReason(HandAction.Split, "Always split on 8 8.");

                case (CardNumber.Seven, CardNumber.Seven, CardNumber.Two):
                case (CardNumber.Seven, CardNumber.Seven, CardNumber.Three):
                case (CardNumber.Seven, CardNumber.Seven, CardNumber.Four):
                case (CardNumber.Seven, CardNumber.Seven, CardNumber.Five):
                case (CardNumber.Seven, CardNumber.Seven, CardNumber.Six):
                case (CardNumber.Seven, CardNumber.Seven, CardNumber.Seven):
                    return new HandActionAndReason(HandAction.Split, "Split on 7 7 when the dealer shows 2 - 7.");

                case (CardNumber.Six, CardNumber.Six, CardNumber.Two):
                case (CardNumber.Six, CardNumber.Six, CardNumber.Three):
                case (CardNumber.Six, CardNumber.Six, CardNumber.Four):
                case (CardNumber.Six, CardNumber.Six, CardNumber.Five):
                case (CardNumber.Six, CardNumber.Six, CardNumber.Six):
                    return new HandActionAndReason(HandAction.Split, "Split on 6 6 when the dealer shows 2 - 6.");

                case (CardNumber.Four, CardNumber.Four, CardNumber.Five):
                case (CardNumber.Four, CardNumber.Four, CardNumber.Six):
                    return new HandActionAndReason(HandAction.Split, "Split on 4 4 when double after split is available, otherwise hit.");

                case (CardNumber.Three, CardNumber.Three, CardNumber.Two):
                case (CardNumber.Three, CardNumber.Three, CardNumber.Three):
                case (CardNumber.Three, CardNumber.Three, CardNumber.Four):
                case (CardNumber.Three, CardNumber.Three, CardNumber.Five):
                case (CardNumber.Three, CardNumber.Three, CardNumber.Six):
                case (CardNumber.Three, CardNumber.Three, CardNumber.Seven):
                    return new HandActionAndReason(HandAction.Split, "Split on 3 3 when the dealer shows 2 - 7.");

                case (CardNumber.Two, CardNumber.Two, CardNumber.Two):
                case (CardNumber.Two, CardNumber.Two, CardNumber.Three):
                case (CardNumber.Two, CardNumber.Two, CardNumber.Four):
                case (CardNumber.Two, CardNumber.Two, CardNumber.Five):
                case (CardNumber.Two, CardNumber.Two, CardNumber.Six):
                case (CardNumber.Two, CardNumber.Two, CardNumber.Seven):
                    return new HandActionAndReason(HandAction.Split, "Split on 2 2 when the dealer shows 2 - 7.");

                default:
                    break;
            }

            return null;
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
        protected (bool hasAce, HandActionAndReason nextAction) IfContainsAceReturnNextAction(Card card1, Card card2, Card dealerVisibleCard) {
            var cardsContainAce = IfContainsAceReturnOtherCard(card1, card2);
            var isDoubleEnabled = BlackjackSettings.GetBlackjackSettings().DoubleDownEnabled;
            // var isSplitEnabled = BlackjackSettings.GetBlackjackSettings().SplitEnabled;
            if (cardsContainAce.hasAce) {
                var otherCard = cardsContainAce.otherCard!;
                switch (otherCard.Number, dealerVisibleCard.Number) {
                    // stand when Ace + Nine or higher (no need to list Ace because that's a split which is taken care of already)
                    case (CardNumber.Nine, _):
                    case (CardNumber.Ten, _):
                    case (CardNumber.Jack, _):
                    case (CardNumber.Queen, _):
                    case (CardNumber.King, _):
                        return (true, new HandActionAndReason(HandAction.Stand, "Stand on Ace 9 or above."));

                    // stand when Ace + 8 except if the dealer has 6, then double down
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
                        return (true, new HandActionAndReason(HandAction.Stand, "Stand on Ace 8 except if dealer shows 6."));
                    case (CardNumber.Eight, CardNumber.Six):
                        return (true, isDoubleEnabled ?
                            new HandActionAndReason(HandAction.Double, "Double on Ace 8 when the dealer shows 6.") :
                            new HandActionAndReason(HandAction.Hit, "Since double isn't available hit on Ace 8 when the dealer shows 6."));

                    // Ace + 7:
                    //  Double 2 - 6, stand 7 & 8, hit otherwise
                    case (CardNumber.Seven, CardNumber.Two):
                    case (CardNumber.Seven, CardNumber.Three):
                    case (CardNumber.Seven, CardNumber.Four):
                    case (CardNumber.Seven, CardNumber.Five):
                    case (CardNumber.Seven, CardNumber.Six):
                        return (true, isDoubleEnabled ?
                            new HandActionAndReason(HandAction.Double, "Double on Ace 7 when dealer shows 2 - 6.") :
                            new HandActionAndReason(HandAction.Hit, "Since double isn't available hit on Ace 7 when dealer shows 2 - 6."));
                    case (CardNumber.Seven, CardNumber.Seven):
                    case (CardNumber.Seven, CardNumber.Eight):
                        return (true, new HandActionAndReason(HandAction.Stand, "Stand on Ace 7 when dealer shows a 7 or 8."));
                    case (CardNumber.Seven, _):
                        return (true, new HandActionAndReason(HandAction.Stand, "Hit on Ace 7 when the dealer shows 9, 10 or Ace."));

                    // Ace + 6
                    //  Double when 3-6, otherwise Hit
                    case (CardNumber.Six, CardNumber.Three):
                    case (CardNumber.Six, CardNumber.Four):
                    case (CardNumber.Six, CardNumber.Five):
                    case (CardNumber.Six, CardNumber.Six):
                        return (true, isDoubleEnabled ?
                            new HandActionAndReason(HandAction.Double, "Double on Ace 6 when the dealer shows 3, 4, 5 or 6, hit otherwise.") :
                            new HandActionAndReason(HandAction.Hit, "Since double isn't available hit. Double on Ace + 6 when the dealer shows 3, 4, 5 or 6, hit otherwise."));
                    case (CardNumber.Six, _):
                        return (true, new HandActionAndReason(HandAction.Hit, "Hit on Ace 6 unless dealer shows 3, 4, 5 or 6 when you double."));

                    // Ace + 5
                    //  Double when 4 - 6, otherwise Hit
                    case (CardNumber.Five, CardNumber.Four):
                    case (CardNumber.Five, CardNumber.Five):
                    case (CardNumber.Five, CardNumber.Six):
                        return (true, isDoubleEnabled ? 
                            new HandActionAndReason(HandAction.Double,"Double on Ace 5 when dealer shows 4, 5 or 6, otherwise hit.") : 
                            new HandActionAndReason(HandAction.Hit,"Since double isn't available hit. Double on Ace 5 when dealer shows 4, 5 or 6, otherwise hit."));
                    case (CardNumber.Five, _):
                        return (true, new HandActionAndReason(HandAction.Hit,"Hit on Ace 5 unless dealer shows 4, 5 or 6 when you double."));

                    // Ace + 4
                    //  Double when 4 - 6, otherwise Hit
                    case (CardNumber.Four, CardNumber.Four):
                    case (CardNumber.Four, CardNumber.Five):
                    case (CardNumber.Four, CardNumber.Six):
                        return (true, isDoubleEnabled ?
                            new HandActionAndReason(HandAction.Double, "Double on Ace 4 when dealer shows 4, 5 or 6, otherwise hit.") :
                            new HandActionAndReason(HandAction.Hit, "Since double isn't available hit. Double on Ace 4 when dealer shows 4, 5 or 6, otherwise hit."));
                    case (CardNumber.Four, _):
                        return (true, new HandActionAndReason(HandAction.Hit,"Hit on Ace 4 unless dealer shows 4, 5 or 6 when you double."));

                    // Ace + 3
                    //  Double if 5,6 otherwise Hit
                    case (CardNumber.Three, CardNumber.Five):
                    case (CardNumber.Three, CardNumber.Six):
                        return (true, isDoubleEnabled ? 
                            new HandActionAndReason(HandAction.Double,"Double on Ace 3 when dealer shows 5 or 6, otherwise hit.") : 
                            new HandActionAndReason(HandAction.Hit,"Since double isn't available hit. Double on Ace 3 when dealer shows 5 or 6, otherwise hit."));
                    case (CardNumber.Three, _):
                        return (true, new HandActionAndReason(HandAction.Hit,"Hit on Ace 3 unless dealer shows 5 or 6 when you double."));

                    // Ace + 2
                    //  Double if 5,6 otherwise Hit
                    case (CardNumber.Two, CardNumber.Five):
                    case (CardNumber.Two, CardNumber.Six):
                        return (true, isDoubleEnabled ?
                            new HandActionAndReason(HandAction.Double, "Double on Ace 2 when dealer shows 5 or 6, otherwise hit.") :
                            new HandActionAndReason(HandAction.Hit, "Since double isn't available hit. Double on Ace 2 when dealer shows 5 or 6, otherwise hit."));
                    case (CardNumber.Two, _):
                        return (true, new HandActionAndReason(HandAction.Hit,"Hit on Ace 2 unless dealer shows 5 or 6 when you double."));

                    default:
                        break;
                }
            }

            // these values shouldn't be used by the caller
            return (false, isDoubleEnabled ? 
                new HandActionAndReason(HandAction.Split,"") : 
                new HandActionAndReason(HandAction.Hit,""));
        }
    }
}
