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
using SayedHa.Blackjack.Cli.Extensions;
using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Players;
using Spectre.Console;

namespace SayedHa.Blackjack.Cli {
    public class SpectreConsolePlayer : Player {
        bool IncludeScoreInOutput { get; set; } = true;
        public override HandActionAndReason GetNextAction(Hand hand, DealerHand dealerHand, int dollarsRemaining) {
            var handScore = hand.GetScore();
            if (handScore >= 21) {
                return new(HandAction.Stand, "player choice");
            }

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<HandAction>()
                .Title($@"Select your next action. ({hand.GetSpectreString(isDealerHand: false, hideFirstCard: false, includeScore: true)})")
                .AddChoices(hand.GetValidActions(dollarsRemaining).ToArray())
                );

            return new(action, "player choice");
        }
    }
}
