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
namespace SayedHa.Blackjack.Shared.Roulette {
    public class ConsoleGameRecorder : GameRecorderBase {
        public ConsoleGameRecorder() : base() {
            EnableFileOutput = false;
        }
        public bool Enabled { get; set; } = true;

        public override bool IsBankrupt => false;

        public override Task RecordSpinAsync(GameCell cell) {
            RecordSpin(cell);
            return Task.CompletedTask;
        }
        public void RecordSpin(GameCell cell) {
            if (Enabled) {
                Console.WriteLine(cell.ToString());
            }
        }
    }
}
