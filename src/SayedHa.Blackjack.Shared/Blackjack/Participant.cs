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
using SayedHa.Blackjack.Shared.Betting;
using SayedHa.Blackjack.Shared.Players;
using System.Diagnostics;

namespace SayedHa.Blackjack.Shared {
    public class Participant {
        public Participant(ParticipantRole role, Player player, BettingStrategy bettingStrategy) {
            Role = role;
            Player = player;

            // TODO: improve this
            Name = role.ToString();
            BettingStrategy = bettingStrategy;
        }
        public ParticipantRole Role { get; init; }
        public string Name { get; protected init; }
        public BettingStrategy BettingStrategy { get; protected init; }

        /// <summary>
        /// This is the current hand(s) in play
        /// It needs to be a list because a split can create multiple hands
        /// </summary>
        public List<Hand> Hands { get; set; } = new List<Hand>();
        public LinkedList<Hand> AllHands { get; set; } = new LinkedList<Hand>();


        /// <summary>
        /// This determines the next action for the players hand(s).
        /// </summary>
        public Player Player { get; init; }
    }

    public class Dealer:Participant {
        public Dealer(Player player, BettingStrategy bettingStrategy) :
            base(ParticipantRole.Dealer, player, bettingStrategy) {
        }

        // TODO: maybe there is a better to way to do this?
        public DealerHand? DealerHand {
            get {
                if(Hands!= null && Hands.Count > 0) {
                    // not using as because it should raise an exception if it's not DealerHand
                    return (DealerHand)Hands[0];
                }
                return null;
            }
        }
    }
    public class Opponent : Participant {
        public Opponent(Player player, BettingStrategy bettingStrategy) : 
            base(ParticipantRole.Player, player, bettingStrategy) {
        }
    }

    public enum ParticipantRole {
        Dealer,
        Player
    }
}
