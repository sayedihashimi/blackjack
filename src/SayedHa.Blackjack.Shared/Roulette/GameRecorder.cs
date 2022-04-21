using System.Reflection.Metadata.Ecma335;

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
        public bool StopWhenBankrupt { get; set; }
        public bool IsBankrupt { get; }
    }
    public interface IGameRollupRecorder {
        public Task WriteGameSummaryHeaderToAsync(StreamWriter writer);
        public Task WriteGameSummaryToAsync(StreamWriter writer);
        public string GetMethodDisplayName();
        public string GetMethodCompactName();
        public void Reset();
    }

    public abstract class GameRecorderBase : IGameRecorder {
        public bool EnableFileOutput { get; set; } = true;
        public string OutputPath { get; set; }
        public string FilenamePrefix { get; set; }
        public bool StopWhenBankrupt { get; set; } = true;
        public abstract bool IsBankrupt { get; }
        public int MinimumBet { get; init; } = 1;
        public long MaximumBet { get; set; }=long.MaxValue;
        public bool AllowNegativeBankroll { get; set; } = false;
        public virtual void Reset() { }
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
