using System.Diagnostics;

namespace SayedHa.Blackjack.Shared {
    public class GameResult {
        public GameResult(Hand dealerHand, List<Hand> opponentHands) {
            Debug.Assert(dealerHand != null);
            Debug.Assert(opponentHands != null);

            DealerHand = dealerHand;
            OpponentHands = opponentHands;
        }
        public Hand DealerHand { get; protected init; }
        public List<Hand> OpponentHands { get; protected init; }
    }
}
