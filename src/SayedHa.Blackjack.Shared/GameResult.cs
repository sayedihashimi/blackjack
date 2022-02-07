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
using System.Diagnostics;

namespace SayedHa.Blackjack.Shared {
    public class GameResult {
        public GameResult(Hand dealerHand, List<Hand> opponentHands,Participant dealer, List<Participant>opponents) {
            Debug.Assert(dealerHand != null);
            Debug.Assert(opponentHands != null);

            DealerHand = dealerHand;
            OpponentHands = opponentHands;

            DealerRemainingCash = (
                dealer.BettingStrategy.Bankroll.DollarsRemaining,
                dealer.BettingStrategy.Bankroll.DollarsRemaining - dealer.BettingStrategy.Bankroll.InitialBankroll);
            OpponentRemaining = new List<(float,float)>();
            foreach (var op in opponents) {
                OpponentRemaining.Add((
                    op.BettingStrategy.Bankroll.DollarsRemaining,
                    op.BettingStrategy.Bankroll.DollarsRemaining - op.BettingStrategy.Bankroll.InitialBankroll));
            }
        }
        public Hand DealerHand { get; protected init; }
        public List<Hand> OpponentHands { get; protected init; }

        public (float remaining, float diff) DealerRemainingCash { get; protected init; }
        public List<(float remaining,float diff)> OpponentRemaining { get;protected init; }
    }
}
