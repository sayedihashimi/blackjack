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
    public class CsvGameRecorder : GameRecorderBase {
        public CsvGameRecorder(string outputPath) : base() {
            Debug.Assert(!string.IsNullOrEmpty(outputPath));
            OutputPath = outputPath;
            //CsvFilePath = csvFilepath;
        }
        public CsvGameRecorder(string outputPath, string filenamePrefix) : this(outputPath) {
            FilenamePrefix = filenamePrefix;
        }
        public virtual string GetCsvFilePath() => Path.Combine(OutputPath!, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}game.csv" : $"game.csv");
        protected StreamWriter? StreamWriter { get; set; }

        public override bool IsBankrupt => false;

        protected bool isInitalized = false;
        private bool disposedValue;

        protected async Task InitalizeAsync() {
            if (!EnableFileOutput) { return; }

            isInitalized = true;
            StreamWriter = new StreamWriter(GetCsvFilePath(), false);
            await WriteHeaderAsync();
        }
        protected virtual async Task WriteHeaderAsync() {
            if (!EnableFileOutput) { return; }

            await StreamWriter!.WriteLineAsync("text,color");
        }
        protected virtual async Task WriteLineForAsync(GameCell cell) {
            if (!EnableFileOutput) { return; }
            if (!isInitalized) {
                await InitalizeAsync();
            }
            await StreamWriter!.WriteLineAsync($"{cell.Text},{cell.Color}");
        }
        public override async Task RecordSpinAsync(GameCell cell) {
            await WriteLineForAsync(cell);
        }

        public override async Task GameCompleted() {
            if(StreamWriter != null) {
                await StreamWriter.FlushAsync();
            }
        }

        protected new void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing && StreamWriter is not null) {
                    StreamWriter.Flush();
                    StreamWriter.Close();
                }
                disposedValue = true;
            }
        }
    }
}
