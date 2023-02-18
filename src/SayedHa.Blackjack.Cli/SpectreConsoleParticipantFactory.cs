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
using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Betting;
using SayedHa.Blackjack.Shared.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Cli {
    public class SpectreConsoleParticipantFactory : ParticipantFactory {
        public SpectreConsoleParticipantFactory(ILogger logger) : 
            base(logger) {
            throw new NotSupportedException($"This constructor is not supported in this class.");
        }

        public SpectreConsoleParticipantFactory(BettingStrategy bettingStrategy, ILogger logger) : base(bettingStrategy, OpponentPlayStrategy.UserInput, logger) {
        }

        public override Participant CreateNewOpponent(OpponentPlayStrategy strategy, ILogger logger) =>
            new SpectreConsoleParticipant(ParticipantRole.Player, new SpectreConsolePlayer(), BettingStrategy.Bankroll);
    }

    public class SpectreConsoleParticipant : Participant {
        public SpectreConsoleParticipant(ParticipantRole role, Player player, Bankroll bankroll) : base(role, player, SpectreConsoleBettingStrategy.CreateNewDefaultBettingStrategy(bankroll)) {
        }
    }
}
