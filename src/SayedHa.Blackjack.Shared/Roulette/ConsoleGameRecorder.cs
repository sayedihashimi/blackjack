namespace SayedHa.Blackjack.Shared.Roulette {
    public class ConsoleGameRecorder : GameRecorderBase {
        public bool Enabled { get; set; } = true;

        public override async Task RecordSpinAsync(GameCell cell) {
            RecordSpin(cell);
        }
        public void RecordSpin(GameCell cell) {
            if (Enabled) {
                Console.WriteLine(cell.ToString());
            }
        }
    }
}
