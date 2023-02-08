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

namespace SayedHa.Blackjack.Shared.Roulette {
    public class RoulettePlayer {
        public async Task PlayAsync(GameSettings settings, List<IGameRecorder> recorders) {
            Debug.Assert(settings != null);
            Debug.Assert(recorders != null);
            // first build the board
            var board = Board.BuildBoard(settings);
            await PlayAsync(board, settings, recorders);
        }
        public async Task PlayAsync(Board board, GameSettings settings, List<IGameRecorder> recorders) {
            Debug.Assert(board != null);
            Debug.Assert(settings != null);
            Debug.Assert(recorders != null);

            var numCells = board.Cells!.Count;

            var numberOfSpins = settings.NumberOfSpins;
            for (int i = 0; i < numberOfSpins; i++) {
                var spinValue = GetRandomCellFrom(board);
                foreach (var recorder in recorders) {
                    await recorder.RecordSpinAsync(spinValue);
                }
            }
        }

        private Random _random = new Random();
        // TODO: does this need to be improved?
        protected GameCell GetRandomCellFrom(Board board) =>
            board.Cells![GetRandomNum(board.Cells!.Count)];
        public int GetRandomNum(int max) =>
            _random.Next(max);
    }
}
