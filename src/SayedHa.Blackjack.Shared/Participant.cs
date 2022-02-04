using SayedHa.Blackjack.Shared.Players;

namespace SayedHa.Blackjack.Shared {
    public class Participant {
        public Participant(ParticipantRole role, Player player){
            Role = role;
            Player = player;
        }
        public ParticipantRole Role { get; init; }

        // needs to be a list because of splits
        public List<Hand> Hands { get; set; } = new List<Hand>();

        /// <summary>
        /// This determines the next action for the players hand(s).
        /// </summary>
        public Player Player { get; init; }
    }

    public class Dealer:Participant {
        public Dealer(Player player):base(ParticipantRole.Dealer, player) {
        }
    }
    public class Opponent : Participant {
        public Opponent(Player player) : base(ParticipantRole.Player, player) {
        }
    }

    public enum ParticipantRole {
        Dealer,
        Player
    }

    public class ParticipantFactory {
        public Participant GetDefaultDealer() {
            return new Dealer(new StandOnValuePlayer());
        }

        public Participant CreateNewOpponent(OpponentPlayStrategy strategy, ILogger logger) => strategy switch {
            OpponentPlayStrategy.BasicStrategy => new Opponent(new BasicStrategyPlayer(logger)),
            OpponentPlayStrategy.StandOn17 => new Opponent(new StandOnValuePlayer()),
            _ => throw new ApplicationException($"unknown value for OpponentPlayStrategy: '{strategy}'")
        };

    }
}
