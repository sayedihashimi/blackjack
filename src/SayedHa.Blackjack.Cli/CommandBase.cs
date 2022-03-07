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
using System.CommandLine;

namespace SayedHa.Blackjack.Cli {
    public interface ICommand {
        Command CreateCommand();
    }
    public abstract class CommandBase : ICommand {
        public abstract Command CreateCommand();

        protected Option OptionVerbose() =>
            new Option(new string[] { "--verbose" }, "enables verbose output") {
                Argument = new Argument<bool>(name: "verbose")
            };

        public bool EnableVerbose { get; set; }
    }
}
