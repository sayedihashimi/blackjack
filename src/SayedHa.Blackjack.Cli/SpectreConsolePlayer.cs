using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Players;
using Spectre.Console;

namespace SayedHa.Blackjack.Cli {
    public class SpectreConsolePlayer : Player {
        bool IncludeScoreInOutput { get;set; } = true;
        public override HandAction GetNextAction(Hand hand, DealerHand dealerHand) => AnsiConsole.Prompt(
            new SelectionPrompt<HandAction>()
            .Title($@"Your hand:{hand.ToString(hideFirstCard: false, useSymbols: true, includeScore: IncludeScoreInOutput, includeBrackets: false, includeResult: false).EscapeMarkup()} Dealer hand: {dealerHand.ToString(hideFirstCard: true, useSymbols: true, includeScore: false, includeBrackets: false, includeResult: false).EscapeMarkup()}
Select your next action.")
            .AddChoices(new[] { HandAction.Stand, HandAction.Hit, HandAction.Double, HandAction.Split })
            );
    }
}
