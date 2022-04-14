using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Roulette {
    /// <summary>
    /// This will keep track of individual numbers.
    /// </summary>
    public class NumberDetailsRecorder : IGameRecorder {
        public NumberDetailsRecorder(GameSettings gameSettings) {
            if(gameSettings == null) {
               throw new ArgumentNullException(nameof(gameSettings));
            }
            GameSettings = gameSettings;
            LastFewSpins = new EnumerableDropOutStack<GameCell>(3);
            CellNumberDetailsMap = new Dictionary<GameCell, NumberDetails>();

            var board = Board.BuildBoard(gameSettings);
            foreach (var cell in board.Cells!) {
                CellNumberDetailsMap.Add(cell,new NumberDetails(cell));
            }
        }

        public Dictionary<GameCell,NumberDetails> CellNumberDetailsMap { get; set; }
        protected GameSettings GameSettings { get; set; }
        protected EnumerableDropOutStack<GameCell> LastFewSpins { get; set; }
        public void Dispose() {
            // nothing to do here
        }

        protected long NumRedBlackSwaps { get; set; }

        private long _numberOfSpins = 0;

        private GameCellColor _lastColor;
        public async Task RecordSpinAsync(GameCell cell) {
            _numberOfSpins++;
            foreach (var key in CellNumberDetailsMap.Keys) {
                var item = CellNumberDetailsMap[key];
                if (cell.Equals(key)) {
                    item.NumberOfTimesHit++;
                    item.ConsecutiveHits++;
                    item.SpinsSinceLastHit = 0;

                    // 2 because we just incremented ConsecutiveHits
                    if (item.ConsecutiveHits >= 2) {
                        item.NumberOfTimesBackToBack++;
                    }
                    if (item.ConsecutiveHits >= 3) {
                        item.NumberOfTimesThreeInARow++;
                    }
                }
                else {
                    item.ConsecutiveHits = 0;
                    item.SpinsSinceLastHit++;
                }

                if (item.MaxConsecutiveHits < item.ConsecutiveHits) {
                    item.MaxConsecutiveHits = item.ConsecutiveHits;
                }
                if(item.MaxNumSpinsSinceLastHit < item.SpinsSinceLastHit) {
                    item.MaxNumSpinsSinceLastHit = item.SpinsSinceLastHit;
                }
            }

            if(_numberOfSpins > 1 && cell.Color != GameCellColor.Green && cell.Color != _lastColor) {
                NumRedBlackSwaps++;
            }
        }

        public async Task WriteReport(string filepath) {
            using var writer = new StreamWriter(filepath, true);
            await writer.WriteLineAsync($"Number of spin: {_numberOfSpins}\n");
            await writer.WriteLineAsync("Number details ".PadRight(60,'-'));
            await writer.WriteLineAsync(string.Empty);

            await writer.WriteLineAsync($"Number of black and red swaps: {NumRedBlackSwaps}");
            await writer.WriteLineAsync(string.Empty);

            foreach (var key in CellNumberDetailsMap.Keys) {
                var item = CellNumberDetailsMap[key];
                await writer.WriteLineAsync($"---- {key.Text} ---".PadRight(11,'-'));
                await writer.WriteLineAsync($"Number of hits:                  {item.NumberOfTimesHit}");
                await writer.WriteLineAsync($"Max # spins since last hit:      {item.MaxNumSpinsSinceLastHit}");
                await writer.WriteLineAsync($"Max # of consecutive hits:       {item.MaxConsecutiveHits}");
                await writer.WriteLineAsync($"Number of times back-to-back:    {item.NumberOfTimesBackToBack}");
                await writer.WriteLineAsync($"Number of times three in a row:  {item.NumberOfTimesThreeInARow}");
                await writer.WriteLineAsync(string.Empty);
            }
        }
    }

    public class NumberDetails {
        public NumberDetails(GameCell cell) {
            Cell = cell;

        }
        public GameCell Cell { get; set; }
        public long NumberOfTimesHit { get; set; }
        public int MaxConsecutiveHits { get; set; }
        public int MaxNumSpinsSinceLastHit { get; set; }
        public int NumberOfTimesBackToBack { get; set; }
        public int NumberOfTimesThreeInARow { get; set; }
        public int SpinsSinceLastHit { get; internal set; }
        public int ConsecutiveHits { get; internal set; }
    }
}
