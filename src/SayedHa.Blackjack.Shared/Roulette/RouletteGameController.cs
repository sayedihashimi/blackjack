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

        public void PlayRollup(int numGames) {
            foreach (var recorder in GameRecorders) {
                recorder.EnableFileOutput = false;
            }

            // need to create a StreamWriter to a new file for each rolluprecorder
            var recorderWriterMap = new Dictionary<IGameRollupRecorder, StreamWriter>();
            foreach (var rollupRecorder in RollupRecorders) {
                string filepath = Path.Combine(OutputPath, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}rollup.csv" : $"rollup.csv");
                recorderWriterMap.Add(rollupRecorder, new StreamWriter(filepath, false));
            }

            // play game and write content
            for (int i = 0; i < numGames; i++) {
            }

            // flush/dispose all the streams
        }
    }
}
