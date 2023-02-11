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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Roulette {
    /// <summary>
    /// This will keep track of individual numbers.
    /// </summary>
    public class NumberDetailsRecorder : GameRecorderBase,IGameRollupRecorder, IConsoleSummaryGameRecorder {
        public NumberDetailsRecorder(GameSettings gameSettings,string outputPath) {
            OutputPath = outputPath;
            if(gameSettings == null) {
               throw new ArgumentNullException(nameof(gameSettings));
            }
            GameSettings = gameSettings;
            // CellNumberDetailsMap = new Dictionary<GameCell, NumberDetails>();
            CellNumberList = new List<NumberDetails>();

            var board = Board.BuildBoard(gameSettings);
            foreach (var cell in board.Cells!) {
                // CellNumberDetailsMap.Add(cell,new NumberDetails(cell));
                CellNumberList.Add(new NumberDetails(cell));
            }
        }
        public NumberDetailsRecorder(GameSettings gameSettings, string outputPath, string filenamePrefix) :this(gameSettings,outputPath) {
            FilenamePrefix = filenamePrefix;
        }
        // public Dictionary<GameCell,NumberDetails> CellNumberDetailsMap { get; set; }
        public List<NumberDetails> CellNumberList { get; private init; }
        protected GameSettings GameSettings { get; set; }

        protected long NumRedBlackSwaps { get; set; }

        public override bool IsBankrupt => false;

        private long _numberOfSpins = 0;

        private GameCellColor? _lastColor;
        public override Task RecordSpinAsync(GameCell cell) {
            _numberOfSpins++;
            foreach(var item in CellNumberList) {
                var key = item.Cell;
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

            if(_numberOfSpins > 1 &&
                cell.Color != _lastColor &&
                (cell.Color == GameCellColor.Black || cell.Color == GameCellColor.Red) &&
                (_lastColor == GameCellColor.Black || _lastColor == GameCellColor.Red)) {
                NumRedBlackSwaps++;
            }

            _lastColor = cell.Color;

            return Task.CompletedTask;
        }
        public string GetFilepath()=> Path.Combine(OutputPath!, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}number-details.txt" : $"number-details.txt");
        public string GetCsvFilepath() => Path.Combine(OutputPath!, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}number-details.csv" : $"number-details.csv");

        public async Task WriteReport() {
            if(!EnableFileOutput || string.IsNullOrEmpty(GetFilepath())) {
                return;
            }

            using var writer = new StreamWriter(GetFilepath(), false);
            using var csvWriter = new StreamWriter(GetCsvFilepath(), false);

            await writer.WriteLineAsync($"Number of spins: {_numberOfSpins:N0}\n");
            await writer.WriteLineAsync("Number details ".PadRight(60,'-'));
            await writer.WriteLineAsync(string.Empty);

            await writer.WriteLineAsync($"Number of black and red swaps: {NumRedBlackSwaps:N0}, {(double)NumRedBlackSwaps/(double)GameSettings.NumberOfSpins:P}");
            await writer.WriteLineAsync(string.Empty);

            await csvWriter.WriteLineAsync($"cell,numHits,maxSinceLast,maxConsecutive,numBackToBack,numThreeInARow");
            foreach(var item in CellNumberList) {
                var key = item.Cell;
                await writer.WriteLineAsync($"---- {key.Text} ---".PadRight(11,'-'));
                await writer.WriteLineAsync($"Number of hits:                  {item.NumberOfTimesHit:N0}");
                await writer.WriteLineAsync($"Max # spins since last hit:      {item.MaxNumSpinsSinceLastHit:N0}");
                await writer.WriteLineAsync($"Max # of consecutive hits:       {item.MaxConsecutiveHits:N0}");
                await writer.WriteLineAsync($"Number of times two in a row:    {item.NumberOfTimesBackToBack:N0}");
                await writer.WriteLineAsync($"Number of times three in a row:  {item.NumberOfTimesThreeInARow:N0}");
                await writer.WriteLineAsync(string.Empty);

                await csvWriter.WriteLineAsync($"'{key.Text}',{item.NumberOfTimesHit},{item.MaxNumSpinsSinceLastHit},{item.MaxConsecutiveHits},{item.NumberOfTimesBackToBack},{item.NumberOfTimesThreeInARow}");
            }

            await writer.FlushAsync();
            await csvWriter.FlushAsync();
        }
        public override async Task GameCompleted() {
            await WriteReport();
        }

        public async Task WriteGameSummaryHeaderToAsync(StreamWriter writer) {
            await writer.WriteLineAsync("boardPosition,numHits,maxNumSpinsSinceLastHit,maxNumConsecutiveHits,numTimesHitBackToBack,numTimesHitThreeInARow");
        }

        public async Task WriteGameSummaryToAsync(StreamWriter writer) {
            foreach(var item in CellNumberList) {
                await writer.WriteAsync($"'{item.Cell.Text}',");
                await writer.WriteAsync($"{item.NumberOfTimesHit},");
                await writer.WriteAsync($"{item.MaxNumSpinsSinceLastHit},");
                await writer.WriteAsync($"{item.MaxConsecutiveHits},");
                await writer.WriteAsync($"{item.NumberOfTimesBackToBack},");
                await writer.WriteAsync($"{item.NumberOfTimesThreeInARow}");
                await writer.WriteLineAsync(string.Empty);
            }
        }

        public string GetMethodDisplayName() => "Number details";

        public string GetMethodCompactName() => "number-details";
        public override void Reset() {
            CellNumberList.Clear();

            var board = Board.BuildBoard(GameSettings);
            foreach (var cell in board.Cells!) {
                CellNumberList.Add(new NumberDetails(cell));
            }
        }

        public async Task WriteTextSummaryToAsync(StreamWriter writer) {
            await writer.WriteLineAsync($"For details on individual cells see the file '{GetFilepath()}' and '{GetCsvFilepath()}'\n");
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
