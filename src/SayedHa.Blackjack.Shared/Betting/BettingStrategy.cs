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

namespace SayedHa.Blackjack.Shared.Betting {
    public abstract class BettingStrategy {
        protected BettingStrategy(Bankroll bankroll) {
            Bankroll = bankroll;
        }
        public Bankroll Bankroll { get; protected set; }

        public abstract int GetNextBetAmount(Game game);

        public static BettingStrategy CreateNewDefaultBettingStrategy(ILogger logger) {
            return CreateNewDefaultBettingStrategy(Bankroll.CreateNewDefaultBankroll(logger));
        }
        public static BettingStrategy CreateNewDefaultBettingStrategy(Bankroll bankroll) {
            return BlackjackSettings.GetBlackjackSettings().CreateBettingStrategy(bankroll);
        }
    }

    public class FixedBettingStrategy : BettingStrategy {
        public FixedBettingStrategy(Bankroll bankroll, int betAmount) : base(bankroll) {
            BetAmount = betAmount;
        }
        public FixedBettingStrategy(Bankroll bankroll) : this(bankroll, BlackjackSettings.GetBlackjackSettings().BetAmount) { }
        public int BetAmount { get; protected set; }
        public override int GetNextBetAmount(Game game) {
            return BetAmount;
        }
    }

    public class _1324BettingStrategy : BettingStrategy {
        public _1324BettingStrategy(Bankroll bankroll, int betAmount) : base(bankroll) {
            BetAmount = betAmount;
        }
        public _1324BettingStrategy(Bankroll bankroll) : this(bankroll, BlackjackSettings.GetBlackjackSettings().BetAmount) {

        }

        public int BetAmount { get; protected init; }
        public override int GetNextBetAmount(Game game) {
            // we can inspect the Transactions list to get the next amount

            // need to cycle through the last few entires in Transactions
            int numTransactions = Bankroll.Transactions.Count;

            int numConsecutiveWins = 0;

            for (var i = numTransactions - 1; i >= 0; i--) {
                var transaction = Bankroll.Transactions[i];
                if (transaction > 0) {
                    numConsecutiveWins++;
                }
                else {
                    break;
                }
            }

            int betMultiplier = 1;
            switch (numConsecutiveWins) {
                case > 3:
                    betMultiplier = 4;
                    break;
                case > 2:
                    betMultiplier = 2;
                    break;
                case > 1:
                    betMultiplier = 3;
                    break;
                default:
                    betMultiplier = 1;
                    break;
            }

            return betMultiplier * BetAmount;
        }
    }
}
