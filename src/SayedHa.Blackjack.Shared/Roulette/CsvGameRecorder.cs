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
        public virtual string GetCsvFilePath() => Path.Combine(OutputPath, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}game.txt" : $"game.txt");
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
