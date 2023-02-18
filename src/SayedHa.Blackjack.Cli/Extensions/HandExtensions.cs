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
using SayedHa.Blackjack.Shared;
using System.Diagnostics;
using System.Text;

namespace SayedHa.Blackjack.Cli.Extensions {
    public static class HandExtensions {
        public static string GetSpectreString(this Hand hand, bool hideFirstCard = true, bool includeScore = false, bool includeResult = false, bool includeBet = false) {
            Debug.Assert(hand != null);
            var sb = new StringBuilder();

            for (var i = 0; i < hand.DealtCards.Count; i++) {
                if (hideFirstCard && i == 0) {
                    sb.Append("??");
                }
                else {
                    sb.Append(hand.DealtCards[i].ToString(true).SpectreEscapeCards());
                }

                if (i < hand.DealtCards.Count - 1) {
                    sb.Append(",");
                }
            }

            if (includeScore) {
                if (hideFirstCard) {
                    sb.Append($" Score=??");
                }
                else {
                    sb.Append($" Score={hand.GetScore()}");
                }
            }
            if(includeBet) {
                sb.Append($" ({hand.Bet:C0})");
            }
            if (includeResult) {
                sb.Append($" [bold]{GetSpectreHandResultString(hand.HandResult)}[/]");
            }

            return sb.ToString().Trim();
        }
        public static string GetSpectreHandResultString(this HandResult handResult) => handResult switch {
            HandResult.InPlay => "In play",
            HandResult.Push => "Push",
            HandResult.DealerWon => "[bold red]Dealer won[/]",
            HandResult.OpponentWon => "[bold green]You won[/]",
            _ => throw new ApplicationException($"Unknown value for HandResult: '{handResult}'")
        };
    }
}
