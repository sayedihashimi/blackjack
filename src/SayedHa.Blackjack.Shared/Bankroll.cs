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

namespace SayedHa.Blackjack.Shared {
    public class Bankroll {
        public Bankroll(int initialBankroll, ILogger logger) {
            InitialBankroll = initialBankroll;
            DollarsRemaining = InitialBankroll;
            Logger = logger;
        }
        protected ILogger Logger { get; init; }
        public float DollarsRemaining { get; protected set; }
        public float InitialBankroll { get; protected init; }
        public static Bankroll CreateNewDefaultBankroll(ILogger logger) {
            return new Bankroll(1000, logger);
        }
        public List<float> Transactions { get; protected set; }=new List<float>();

        public float AddToDollarsRemaining(float amount, string participantName) {
            Logger.LogLine($"Bankroll update for '{participantName}': {amount}");
            Transactions.Add(amount);
            DollarsRemaining += amount;
            return DollarsRemaining;
        }

    }
}
