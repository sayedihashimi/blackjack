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
using System.CommandLine.Invocation;

namespace SayedHa.Blackjack.Cli {
    public class AnalyzeCommand : CommandBase {
        private IReporter _reporter;
        public AnalyzeCommand(IReporter reporter) {
            _reporter = reporter;
        }
        public override Command CreateCommand() =>
            new Command(name: "analyze", description: "will perform analysis with the specified parameters") {
                CommandHandler.Create<int,string, bool, bool>(async (numGamesToPlay, outputPath, enableConsoleLogger, verbose) => {
                    _reporter.EnableVerbose = verbose;
                    _reporter.WriteLine(string.Empty);
                    _reporter.WriteLine($"numGamesToPlay: {numGamesToPlay}");
                    _reporter.WriteLine($"outputPath: {outputPath}");
                    _reporter.WriteLine($"enableConsoleLogger: {enableConsoleLogger}");
                    _reporter.WriteLine($"verbose: {verbose}");
                    _reporter.WriteVerbose("verbose message here");
                    // added here to avoid async/await warning
                    await Task.Delay(1000);
                }),
                OptionNumGamesToPlay(),
                OptionOutputPath(),
                OptionEnableConsoleLogger(),
                OptionVerbose(),
            };
        // usage
        // [numGamesToPlay] [outputPath] [enableConsoleLogger] [enableFileLogger] [enableMultiThread]

        protected Option OptionNumGamesToPlay() {
            var opt = new Option(new string[] { "--numGamesToPlay" }, "number of games to play") {
                Argument = new Argument<string>(name: "numGamesToPlay")
            };
            opt.Argument.SetDefaultValue("10");
            return opt;
        }

        protected Option OptionOutputPath() =>
            new Option(new string[] { "--outputPath" }, "output path, folder path for where files will be written") {
                Argument = new Argument<string>(name: "outputPath")
            };

        protected Option OptionEnableConsoleLogger() {
            var opt = new Option(new string[] { "--enableConsoleLogger" }, "Option to enable or disable the console logger.") {
                Argument = new Argument<bool>(name: "enableConsoleLogger")
            };
            opt.Argument.SetDefaultValue(true);
            return opt;
        }

        protected Option OptionEnableFileLogger() {
            var opt =  new Option(new string[] { "--enableFileLogger" }, "Option to enable file logger, the outputPath parameter must be passed for log files") {
                Argument = new Argument<bool>(name: "enableFileLogger")
            };
            opt.Argument.SetDefaultValue("true");
            return opt;
        }

        protected Option OptionEnableMultiThread() {
            var opt = new Option(new string[] { "--enableMultiThread" }, "Option to enable multi threading. When running many different strategies multi-threading may improve the perf.") {
                Argument = new Argument<bool>(name: "enableMultiThread")
            };
            opt.Argument.SetDefaultValue(false);
            return opt;
        }
    }
}
