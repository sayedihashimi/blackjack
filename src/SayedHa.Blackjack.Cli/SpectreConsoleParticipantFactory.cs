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
