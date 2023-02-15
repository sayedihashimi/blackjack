using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Cli.Extensions {
    public static class StringExtensions {
        /// <summary>
        /// Changes the following.
        ///     ♥ changes to [red]♥[/]
        ///     ♦ changes to [red]♦[/]
        ///
        /// Do not call EscapeMarkup on the string after this call, if so the formatting will be escaped.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SpectreEscapeCards(this string str) {
            return str.Replace("♥", "[green]♥[/]").Replace("♦", "[green]♦[/]");
        }
    }
}
