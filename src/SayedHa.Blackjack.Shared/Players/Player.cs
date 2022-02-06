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
namespace SayedHa.Blackjack.Shared.Players {
    /// <summary>
    /// Given a Hand, the Player will determine the next action.
    /// If the Hand is dead, no actions can be taken on the Hand. All Players should follow this rule.
    /// </summary>
    public abstract class Player {
        /// <summary>
        /// Given the hand, what should the next action be?
        /// For any closed hand the action should be Stand.
        /// Each player is free to implement whatever playing
        /// style they prefer.
        /// </summary>
        public abstract HandAction GetNextAction(Hand hand, DealerHand dealerHand);
    }
}
