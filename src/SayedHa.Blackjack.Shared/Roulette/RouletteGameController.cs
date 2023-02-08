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
    public class RouletteGameController {
        public RouletteGameController(GameSettings gameSettings, string outputPath, string filenamePrefix) {
            GameSettings = gameSettings;
            this.OutputPath = outputPath;
            this.FilenamePrefix = filenamePrefix;

            InitFromGameSettings();

            Board = Board.BuildBoard(GameSettings);
        }
        public GameSettings GameSettings { get; protected init; }
        public string OutputPath { get; protected init; }
        public string FilenamePrefix { get; protected init; }
        public List<IGameRecorder> Recorders { get; protected init; } = new List<IGameRecorder>();
        public List<IGameRollupRecorder> RollupRecorders { get; protected init; } = new List<IGameRollupRecorder>();
        protected Board Board { get; init; }

        protected void InitFromGameSettings() {
            Debug.Assert(GameSettings != null);
            Debug.Assert(!string.IsNullOrEmpty(OutputPath));
            Debug.Assert(!string.IsNullOrEmpty(FilenamePrefix));

            if (GameSettings.EnableConsoleLogger) {
                Recorders.Add(new ConsoleGameRecorder());
            }

            if (GameSettings.EnableCsvFileOutput) {
                Recorders.Add(new CsvGameRecorder(OutputPath, FilenamePrefix));
            }
            if (GameSettings.EnableNumberDetails) {
                Recorders.Add(new NumberDetailsRecorder(GameSettings, OutputPath, FilenamePrefix));
            }

            if (GameSettings.EnableMartingale) {
                Recorders.Add(new MartingaleBettingRecorder(OutputPath, FilenamePrefix, GameCellColor.Black, GameSettings.MinimumBet, GameSettings.InitialBankroll, true) {
                    MaximumBet = GameSettings.MaximumBet,
                    AllowNegativeBankroll = GameSettings.AllowNegativeBankroll
                });
                Recorders.Add(new MartingaleBettingRecorder(OutputPath, FilenamePrefix, GameCellColor.Red, GameSettings.MinimumBet, GameSettings.InitialBankroll, false) {
                    MaximumBet = GameSettings.MaximumBet,
                    AllowNegativeBankroll = GameSettings.AllowNegativeBankroll
                });
            }
            if (GameSettings.EnableGreen) {
                Recorders.Add(new GreenMethodRecorder(OutputPath, FilenamePrefix, GameSettings.MinimumBet, GameSettings.InitialBankroll, true) {
                    MaximumBet = GameSettings.MaximumBet,
                    AllowNegativeBankroll = GameSettings.AllowNegativeBankroll
                });
                Recorders.Add(new GreenAgressiveMethodRecorder(OutputPath, FilenamePrefix, GameSettings.MinimumBet, GameSettings.InitialBankroll, true) {
                    MaximumBet = GameSettings.MaximumBet,
                    AllowNegativeBankroll = GameSettings.AllowNegativeBankroll
                });
            }
            if (GameSettings.EnableBondMartingale) {
                Recorders.Add(new BondMartingaleBettingRecorder(OutputPath, FilenamePrefix, GameSettings.MinimumBet, GameSettings.InitialBankroll, true) {
                    MaximumBet = GameSettings.MaximumBet,
                    AllowNegativeBankroll = GameSettings.AllowNegativeBankroll
                });
            }

            // csv with stats always needs to be added for the summary
            var csvWithStatsRecorder = new CsvWithStatsGameRecorder(OutputPath, FilenamePrefix);
            csvWithStatsRecorder.EnableWriteCsvFile = GameSettings.EnableCsvFileOutput;
            Recorders.Add(csvWithStatsRecorder);
        }

        public void AddGameRecorder(IGameRecorder recorder) {
            Recorders.Add(recorder);
            if (recorder is IGameRollupRecorder) {
                recorder.StopWhenBankrupt = GameSettings.StopWhenBankrupt;
                RollupRecorders.Add((IGameRollupRecorder)recorder);
            }
        }

        public async Task PlayAll() {
            Console.WriteLine($"Starting play for {GameSettings.NumberOfSpins} spins of {GameSettings.RouletteType} roulette.");

            var player = new RoulettePlayer();
            await player.PlayAsync(Board, GameSettings, Recorders);

            foreach (var recorder in Recorders) {
                await recorder.GameCompleted();
                if(recorder is IConsoleSummaryGameRecorder) {
                    using var sw = new StreamWriter(Console.OpenStandardOutput());
                    await ((IConsoleSummaryGameRecorder)recorder).WriteTextSummaryToAsync(sw);
                    Console.WriteLine();
                    sw.Flush();
                }

                recorder.Dispose();
            }
        }

        public async Task PlayRollup(int numGames) {
            foreach (var recorder in Recorders) {
                recorder.EnableFileOutput = false;
            }

            // need to create a StreamWriter to a new file for each rolluprecorder
            var recorderWriterMap = new Dictionary<IGameRollupRecorder, StreamWriter>();
            foreach (var rollupRecorder in RollupRecorders) {
                string filepath = Path.Combine(OutputPath, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}{rollupRecorder.GetMethodCompactName()}-rollup.csv" : $"{rollupRecorder.GetMethodCompactName()}-rollup.csv");
                Console.WriteLine($"filepath: {filepath}");
                var stream = new StreamWriter(filepath, false);
                // without this some of the content wasn't making it into the file for some reason
                stream.AutoFlush = true;
                recorderWriterMap.Add(rollupRecorder, stream);
            }

            var player = new RoulettePlayer();
            // play game and write content
            foreach (var rollupRecorder in RollupRecorders) {
                await rollupRecorder.WriteGameSummaryHeaderToAsync(recorderWriterMap[rollupRecorder]);
            }
            for (int i = 0; i < numGames; i++) {
                await player.PlayAsync(Board, GameSettings, Recorders);

                foreach (var rollupRecorder in RollupRecorders) {
                    await rollupRecorder.WriteGameSummaryToAsync(recorderWriterMap[rollupRecorder]);
                    rollupRecorder.Reset();
                }
            }

            // flush/dispose all the streams
            foreach (var key in recorderWriterMap.Keys) {
                var rollupRecorderStream = recorderWriterMap[key];
                await rollupRecorderStream.FlushAsync();

                rollupRecorderStream.Dispose();
            }
        }
    }
}
