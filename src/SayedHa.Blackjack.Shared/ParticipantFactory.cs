using SayedHa.Blackjack.Shared.Players;

namespace SayedHa.Blackjack.Shared {
    public class ParticipantFactory {
        public Participant GetDefaultDealer() {
            return new Dealer(new StandOnValuePlayer(17,ParticipantRole.Dealer));
        }

        public Participant CreateNewOpponent(OpponentPlayStrategy strategy, ILogger logger) => strategy switch {
            OpponentPlayStrategy.BasicStrategy => new Opponent(new BasicStrategyPlayer(logger)),
            OpponentPlayStrategy.StandOn12 => new Opponent(new StandOnValuePlayer(12, ParticipantRole.Player)),
            OpponentPlayStrategy.StandOn13 => new Opponent(new StandOnValuePlayer(13, ParticipantRole.Player)),
            OpponentPlayStrategy.StandOn14 => new Opponent(new StandOnValuePlayer(14,ParticipantRole.Player)),
            OpponentPlayStrategy.StandOn15 => new Opponent(new StandOnValuePlayer(15,ParticipantRole.Player)),
            OpponentPlayStrategy.StandOn16 => new Opponent(new StandOnValuePlayer(16,ParticipantRole.Player)),
            OpponentPlayStrategy.StandOn17 => new Opponent(new StandOnValuePlayer(17,ParticipantRole.Player)),
            OpponentPlayStrategy.StandOn18 => new Opponent(new StandOnValuePlayer(18,ParticipantRole.Player)),
            OpponentPlayStrategy.StandOn19 => new Opponent(new StandOnValuePlayer(19, ParticipantRole.Player)),
            OpponentPlayStrategy.StandOn20 => new Opponent(new StandOnValuePlayer(20, ParticipantRole.Player)),
            OpponentPlayStrategy.AlwaysStand => new Opponent(new StandOnValuePlayer(2,ParticipantRole.Player)),
            OpponentPlayStrategy.Random => new Opponent(new RandomPlayer()),
            _ => throw new ApplicationException($"unknown value for OpponentPlayStrategy: '{strategy}'")
        };

    }
}
