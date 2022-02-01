using System.Diagnostics;

namespace SayedHa.Blackjack.Shared.Players {
    /// <summary>
    /// The dealer player only has the following actions available:
    ///  - Hit
    ///  - Stand
    /// The dealer must hit if the card score is less than 17 (MinScoreToStand).
    /// </summary>
    public class DealerPlayer : Player {
        public DealerPlayer() : this(17) { }
        public DealerPlayer(int minScoreToStand) {
            MinScoreToStand = minScoreToStand;
        }

        public int MinScoreToStand { get; protected init; }
        public override HandAction GetNextAction(Hand hand) {
            Debug.Assert(hand != null);

            if (hand.Status == HandStatus.Closed) {
                return HandAction.Stand;
            }

            return hand.GetScore() < MinScoreToStand ? HandAction.Hit : HandAction.Stand;
        }
    }
}
