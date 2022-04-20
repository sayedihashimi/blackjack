using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Roulette {
    public class RouletteGameController {
        public RouletteGameController(GameSettings gameSettings,string outputPath, string filenamePrefix) {
            GameSettings = gameSettings;
            OutputPath = outputPath;
            FilenamePrefix = filenamePrefix;

            Board = Board.BuildBoard(GameSettings);
        }
        public GameSettings GameSettings { get; protected init; }
        public string OutputPath { get; protected init; }
        public string FilenamePrefix { get; protected init; }
        public List<IGameRecorder> GameRecorders { get; protected init; } = new List<IGameRecorder>();
        public List<IGameRollupRecorder> RollupRecorders { get; protected init; } = new List<IGameRollupRecorder>();
        protected Board Board { get; init; }
        public void AddGameRecorder(IGameRecorder recorder) {
            GameRecorders.Add(recorder);
            if (recorder is IGameRollupRecorder) {
                recorder.StopWhenBankrupt = GameSettings.StopWhenBankrupt;
                RollupRecorders.Add((IGameRollupRecorder)recorder);
            }
        }

        public async Task PlayAll() {
            var player = new RoulettePlayer();
            await player.PlayAsync(Board, GameSettings, GameRecorders);

            foreach(var recorder in GameRecorders) {
                await recorder.GameCompleted();
                recorder.Dispose();
            }
        }

        public async Task PlayRollup(int numGames) {
            foreach (var recorder in GameRecorders) {
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
                await player.PlayAsync(Board,GameSettings,GameRecorders);

                foreach (var rollupRecorder in RollupRecorders) {
                    await rollupRecorder.WriteGameSummaryToAsync(recorderWriterMap[rollupRecorder]);
                    rollupRecorder.Reset();
                }
            }

            // flush/dispose all the streams
            foreach(var key in recorderWriterMap.Keys) {
                var rollupRecorderStream = recorderWriterMap[key];
                await rollupRecorderStream.FlushAsync();

                rollupRecorderStream.Dispose();
            }
        }
    }
}
