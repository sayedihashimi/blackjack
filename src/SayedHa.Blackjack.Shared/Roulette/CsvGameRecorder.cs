using System.Diagnostics;

namespace SayedHa.Blackjack.Shared.Roulette {
    public class CsvGameRecorder : GameRecorderBase {
        public CsvGameRecorder(string csvFilepath) {
            Debug.Assert(!string.IsNullOrEmpty(csvFilepath));
            CsvFilePath = csvFilepath;
        }
        protected string CsvFilePath { get; init; }
        protected StreamWriter? StreamWriter { get; set; }
        protected bool isInitalized = false;
        private bool disposedValue;

        protected async Task InitalizeAsync() {
            isInitalized = true;
            StreamWriter = new StreamWriter(CsvFilePath, false);
            await WriteHeaderAsync();
        }
        protected virtual async Task WriteHeaderAsync() {
            await StreamWriter!.WriteLineAsync("text,color");
        }
        protected virtual async Task WriteLineForAsync(GameCell cell) {
            if (!isInitalized) {
                await InitalizeAsync();
            }
            await StreamWriter!.WriteLineAsync($"{cell.Text},{cell.Color}");
        }
        public override async Task RecordSpinAsync(GameCell cell) {
            await WriteLineForAsync(cell);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing && StreamWriter is not null) {
                    StreamWriter.Flush();
                    StreamWriter.Close();
                }
                disposedValue = true;
            }
        }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
