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
using System.Net.Sockets;

namespace SayedHa.Blackjack.Shared {
    public class ParticipantFactory {

        public ParticipantFactory(ILogger logger):
            this(Shared.BettingStrategy.CreateNewDefaultBettingStrategy(logger), 
            OpponentPlayStrategy.BasicStrategy,
            logger) {
        }

        public ParticipantFactory(BettingStrategy bettingStrategy, OpponentPlayStrategy opponentPlayStrategy, ILogger logger) {
            BettingStrategy = bettingStrategy;
            OpponentPlayStrategy = opponentPlayStrategy;
            Logger = logger;
        }

        protected ILogger Logger {get;init;}

        public Participant GetDefaultDealer() {
            return new Dealer(new StandOnValuePlayer(17, ParticipantRole.Dealer), BettingStrategy.CreateNewDefaultBettingStrategy(Logger));
        }

        public BettingStrategy BettingStrategy { get; protected init; }
        public OpponentPlayStrategy OpponentPlayStrategy { get; protected init; }
        

        public Participant CreateNewOpponent(OpponentPlayStrategy strategy, ILogger logger) => strategy switch {
            OpponentPlayStrategy.BasicStrategy => new Opponent(new BasicStrategyPlayer(logger), BettingStrategy),
            OpponentPlayStrategy.StandOn12 => new Opponent(new StandOnValuePlayer(12, ParticipantRole.Player), BettingStrategy),
            OpponentPlayStrategy.StandOn13 => new Opponent(new StandOnValuePlayer(13, ParticipantRole.Player), BettingStrategy),
            OpponentPlayStrategy.StandOn14 => new Opponent(new StandOnValuePlayer(14, ParticipantRole.Player), BettingStrategy),
            OpponentPlayStrategy.StandOn15 => new Opponent(new StandOnValuePlayer(15, ParticipantRole.Player), BettingStrategy),
            OpponentPlayStrategy.StandOn16 => new Opponent(new StandOnValuePlayer(16, ParticipantRole.Player), BettingStrategy),
            OpponentPlayStrategy.StandOn17 => new Opponent(new StandOnValuePlayer(17, ParticipantRole.Player), BettingStrategy),
            OpponentPlayStrategy.StandOn18 => new Opponent(new StandOnValuePlayer(18, ParticipantRole.Player), BettingStrategy),
            OpponentPlayStrategy.StandOn19 => new Opponent(new StandOnValuePlayer(19, ParticipantRole.Player), BettingStrategy),
            OpponentPlayStrategy.StandOn20 => new Opponent(new StandOnValuePlayer(20, ParticipantRole.Player), BettingStrategy),
            OpponentPlayStrategy.AlwaysStand => new Opponent(new StandOnValuePlayer(2, ParticipantRole.Player), BettingStrategy),
            OpponentPlayStrategy.Random => new Opponent(new RandomPlayer(), BettingStrategy),
            _ => throw new ApplicationException($"unknown value for OpponentPlayStrategy: '{strategy}'")
        };

    }
}
