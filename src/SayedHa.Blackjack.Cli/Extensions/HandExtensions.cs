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
