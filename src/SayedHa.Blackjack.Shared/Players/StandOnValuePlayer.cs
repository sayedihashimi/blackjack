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
using System.Diagnostics;

namespace SayedHa.Blackjack.Shared.Players {
    /// <summary>
    /// The dealer player only has the following actions available:
    ///  - Hit
    ///  - Stand
    /// The dealer must hit if the card score is less than 17 (MinScoreToStand).
    /// </summary>
    public class StandOnValuePlayer : Player {
        public StandOnValuePlayer(int minScoreToStand, ParticipantRole role) {
            MinScoreToStand = minScoreToStand;
            Role = role;
        }

        public ParticipantRole Role { get; protected init; }

        public int MinScoreToStand { get; protected init; }
        public override HandAction GetNextAction(Hand hand, DealerHand dealerHand) {
            Debug.Assert(hand != null);

            if (hand.Status == HandStatus.Closed) {
                return HandAction.Stand;
            }

            return hand.GetScore() < MinScoreToStand ? HandAction.Hit : HandAction.Stand;
        }
    }

    public class RandomPlayer : Player {
        public override HandAction GetNextAction(Hand hand, DealerHand dealerHand) {
            if (hand.Status == HandStatus.Closed) {
                return HandAction.Stand;
            }

            var randomNum = new Random().Next(0, 10000);
            var shouldHit = randomNum % 2 == 0 ? true : false;

            return shouldHit ? HandAction.Hit : HandAction.Stand;
        }
    }
}
