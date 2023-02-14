using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Betting;
using Spectre.Console;

namespace SayedHa.Blackjack.Cli {
    public class SpectreConsoleBettingStrategy : BettingStrategy {
        public SpectreConsoleBettingStrategy(Bankroll bankroll) : base(bankroll) {
        }

        public override int GetNextBetAmount(Game game) => AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                .Title("Bet amount?")
                .AddChoices(new[] { 5, 10, 25, 50, 75, 100 })
            );
    }
}
