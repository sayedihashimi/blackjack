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

        protected Board Board { get; init; }
        public void AddGameRecorder(IGameRecorder recorder) => GameRecorders.Add(recorder);

        public async Task PlayAll() {
            var player = new RoulettePlayer();
            await player.PlayAsync(Board, GameSettings, GameRecorders);

            foreach(var recorder in GameRecorders) {
                await recorder.GameCompleted();
                recorder.Dispose();
            }
        }
    }
}
