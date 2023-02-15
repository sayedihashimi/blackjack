using SayedHa.Blackjack.Shared;
using System.Diagnostics;
using System.Text;

namespace SayedHa.Blackjack.Cli.Extensions {
    public static class HandExtensions {
        public static string GetSpectreString(this Hand hand, bool hideFirstCard = true, bool includeScore = false, bool includeResult = false) {
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
            if (includeResult) {
                sb.Append($" Result={hand.HandResult}");
            }

            return sb.ToString().Trim();
        }
    }
}
