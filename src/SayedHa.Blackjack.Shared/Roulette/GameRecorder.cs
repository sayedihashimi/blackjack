namespace SayedHa.Blackjack.Shared.Roulette {
    /// <summary>
    /// GameRecorders are responsbile for capturing the results of the game.
    /// If the game results are to be persisted anywhere, the game recorder
    /// should be the one persisting the data as well.
    /// </summary>
    public interface IGameRecorder : IDisposable {
        public bool EnableFileOutput { get; set; }
        public Task RecordSpinAsync(GameCell cell);
        public Task GameCompleted();
        public string OutputPath { get; set; }
        public string FilenamePrefix { get; set; }
    }
    public abstract class GameRecorderBase : IGameRecorder {
        public bool EnableFileOutput { get; set; } = true;
        public string OutputPath { get; set; }
        public string FilenamePrefix { get; set; }

        protected virtual void Dispose(bool disposing) {

        }
        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public abstract Task RecordSpinAsync(GameCell cell);

        public virtual async Task GameCompleted() {
        }
    }
}
