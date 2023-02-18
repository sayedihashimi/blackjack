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
