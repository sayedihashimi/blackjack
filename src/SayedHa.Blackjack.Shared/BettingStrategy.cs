﻿// This file is part of SayedHa.Blackjack.
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
    public abstract class BettingStrategy {
        protected BettingStrategy(Bankroll bankroll) {
            Bankroll = bankroll;
        }
        public Bankroll Bankroll { get; protected set; }

        public abstract int GetNextBetAmount(Hand hand);

        public static BettingStrategy CreateNewDefaultBettingStrategy() {
            return CreateNewDefaultBettingStrategy(Bankroll.CreateNewDefaultBankroll());
        }
        public static BettingStrategy CreateNewDefaultBettingStrategy(Bankroll bankroll) {
            return new FixedBettingStrategy(bankroll, 5);
        }
    }

    public class FixedBettingStrategy : BettingStrategy {
        public FixedBettingStrategy(Bankroll bankroll) : this(bankroll, 5) { }
        public FixedBettingStrategy(Bankroll bankroll, int betAmount) :base(bankroll) {
            BetAmount = betAmount;
        }
        public int BetAmount { get; protected set; }
        public override int GetNextBetAmount(Hand hand) {
            Debug.Assert(hand != null);

            // TODO: improve this
            return Bankroll.DollarsRemaining > 5 ? 5 : 0;
        }
    }
}
