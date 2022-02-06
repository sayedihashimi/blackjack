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
using SayedHa.Blackjack.Shared.Players;
using System.Diagnostics;

namespace SayedHa.Blackjack.Shared {
    public class Participant {
        public Participant(ParticipantRole role, Player player,Bankroll bankroll){
            Role = role;
            Player = player;

            Bankroll = bankroll;
        }
        public ParticipantRole Role { get; init; }

        public Bankroll Bankroll { get; protected init; }

        // needs to be a list because of splits
        public List<Hand> Hands { get; set; } = new List<Hand>();

        /// <summary>
        /// This determines the next action for the players hand(s).
        /// </summary>
        public Player Player { get; init; }
    }

    public class Dealer:Participant {
        public Dealer(Player player, Bankroll bankroll) :
            base(ParticipantRole.Dealer, player, bankroll) {
        }
    }
    public class Opponent : Participant {
        public Opponent(Player player, Bankroll bankroll) : 
            base(ParticipantRole.Player, player,bankroll) {
        }
    }

    public enum ParticipantRole {
        Dealer,
        Player
    }
}
