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
    public static class KnownValues {
        public const int MaxScore = 21;
        public const int DefaultShuffleThresholdPercent = 20;
    }
    public class BlackjackSettings {
        private BlackjackSettings() {
            DefaultBet = 1;
            DefaultBankrollAmount = 1000;

            MaxScore = 21;
            DefaultShuffleThresholdPercent = 20;
        }
        private static BlackjackSettings _instance = new BlackjackSettings();
        public int DefaultBet { get; protected init; }
        public int MaxScore { get; protected init; }
        public int DefaultShuffleThresholdPercent { get; protected init; }
        public int DefaultBankrollAmount { get; protected init; }

        // replace this with DI later
        public static BlackjackSettings GetBlackjackSettings() {
            return _instance;
        }
    }
}
