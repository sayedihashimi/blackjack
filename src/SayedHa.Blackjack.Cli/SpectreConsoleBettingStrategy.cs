using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Betting;
using Spectre.Console;

namespace SayedHa.Blackjack.Cli {
    public class SpectreConsoleBettingStrategy : BettingStrategy {
        public SpectreConsoleBettingStrategy(Bankroll bankroll) : base(bankroll) {
        }

        public override int GetNextBetAmount(Game game) {
            var betAmountPrompt = new SelectionPrompt<int>()
                .Title("Bet amount?")
                .AddChoices(GetAvailableBetAmounts());
            betAmountPrompt.Converter = amount => amount.ToString("C0");
            return AnsiConsole.Prompt(betAmountPrompt);
        }
        private int[] GetAvailableBetAmounts() => Bankroll.DollarsRemaining switch {
            >= 50000 => new[] { 5, 10, 25, 50, 75, 100, 250, 500, 1000, 2500, 5000, 10000, 20000, 50000 },
            >= 20000 => new[] { 5, 10, 25, 50, 75, 100, 250, 500, 1000, 2500, 5000, 10000, 20000 },
            >= 10000 => new[] { 5, 10, 25, 50, 75, 100, 250, 500, 1000, 2500, 5000, 10000 },
            >= 5000 => new[] { 5, 10, 25, 50, 75, 100, 250, 500, 1000, 2500, 5000 },
            >= 2500 => new[] { 5, 10, 25, 50, 75, 100, 250, 500, 1000, 2500 },
            >= 1000 => new[] { 5, 10, 25, 50, 75, 100, 250, 500, 1000 },
            >= 500 => new[] { 5, 10, 25, 50, 75, 100, 250, 500 },
            >= 250 => new[] { 5, 10, 25, 50, 100, 250,},
            >= 100 => new[] { 5, 10, 25, 50, 75, 100 },
            >= 75 => new[] { 5, 10, 25, 50 , 75},
            >= 50 => new[] { 5, 10, 25, 50 },
            >= 25 => new[] { 5, 10, 25 },
            >= 10 => new[] { 5, 10 },
            >= 5 => new[] { 5 },
            > 0 => new[] {(int)Math.Floor(Bankroll.DollarsRemaining)},
            _ => new int[] { }
        };
    }
}
