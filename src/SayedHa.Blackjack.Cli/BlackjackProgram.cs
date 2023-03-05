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
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

namespace SayedHa.Blackjack.Cli {
    public class BlackjackProgram {
        private BlackjackAppSettings _appSettings;

        AnalyzeCommand _analyzeCommand;
        PlayCommand _playCommand;

        public BlackjackProgram(IOptions<BlackjackAppSettings> appSettings, AnalyzeCommand analyzeCommand, PlayCommand playCommand) {
            _appSettings = appSettings.Value;
            _analyzeCommand = analyzeCommand;
            _playCommand = playCommand;
        }
        public Task<int> Execute(string[] args) {
            Parser _parser = new CommandLineBuilder()
                .AddCommand(_analyzeCommand.CreateCommand())
                .AddCommand(_playCommand.CreateCommand())
                .UseDefaults()
                .Build();

            return _parser.InvokeAsync(args);
        }
    }
    public sealed class BlackjackAppSettings {
        public required bool UseRandomNumberGenerator { get; set; }
    }
}
