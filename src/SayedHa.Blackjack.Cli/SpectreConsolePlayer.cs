using SayedHa.Blackjack.Cli.Extensions;
using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Players;
using Spectre.Console;

namespace SayedHa.Blackjack.Cli {
    public class SpectreConsolePlayer : Player {
        bool IncludeScoreInOutput { get;set; } = true;
        public override HandAction GetNextAction(Hand hand, DealerHand dealerHand) {
            var handScore = hand.GetScore();
            if(handScore >= 21) {
                return HandAction.Stand;
            }

            return AnsiConsole.Prompt(
            new SelectionPrompt<HandAction>()
            .Title($@"Select your next action. ({hand.GetSpectreString(false, includeScore: true)})")
            .AddChoices(hand.GetValidActions().ToArray())
            );
        }
    }
}
