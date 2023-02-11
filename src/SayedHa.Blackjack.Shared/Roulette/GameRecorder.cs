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
using System.Reflection.Metadata.Ecma335;

namespace SayedHa.Blackjack.Shared.Roulette {
    /// <summary>
    /// GameRecorders are responsible for capturing the results of the game.
    /// If the game results are to be persisted anywhere, the game recorder
    /// should be the one persisting the data as well.
    /// </summary>
    public interface IGameRecorder : IDisposable {
        public bool EnableFileOutput { get; set; }
        public Task RecordSpinAsync(GameCell cell);
        public Task GameCompleted();
        public string? OutputPath { get; set; }
        public string? FilenamePrefix { get; set; }
        public bool StopWhenBankrupt { get; set; }
        public bool IsBankrupt { get; }
    }

    /// <summary>
    /// Game recorders can implement this class if they have a game
    /// summary text that they want to display to the end-user.
    /// </summary>
    public interface IConsoleSummaryGameRecorder : IGameRecorder {
        public Task WriteTextSummaryToAsync(StreamWriter writer);
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
        public string? OutputPath { get; set; }
        public string? FilenamePrefix { get; set; }
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

        public virtual Task GameCompleted() {
            return Task.CompletedTask;
        }
    }
}
