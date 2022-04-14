using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SayedHa.Blackjack.Shared.Betting;
using SayedHa.Blackjack.Shared.Extensions;

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
