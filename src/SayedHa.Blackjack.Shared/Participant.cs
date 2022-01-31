namespace SayedHa.Blackjack.Shared {
    public class Player {
        public Player(ParticipantRole role){
            Role = role;
        }
        public ParticipantRole Role { get; init; }
        public Hand Hand { get; set; } = new Hand();
    }

    public class Dealer:Player {
        public Dealer():base(ParticipantRole.Dealer) {
        }
    }
    public class Opponent : Player {
        public Opponent() : base(ParticipantRole.Player) {
        }
    }

    public enum ParticipantRole {
        Dealer,
        Player
    }
}
