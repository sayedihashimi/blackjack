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
using SayedHa.Blackjack.Shared.Roulette;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.Drawing;
using static System.CommandLine.Help.DefaultHelpText;

namespace SayedHa.Roulette.Cli {
    public class SimulateCommand : CommandBase {
        private IReporter _reporter;
        private readonly IDefaultGameSettingsFile _gameSettingsFile;

        public SimulateCommand(IReporter reporter, IDefaultGameSettingsFile gameSettingsFile) {
            _reporter = reporter;
            _gameSettingsFile = gameSettingsFile;
        }
        public override Command CreateCommand() =>
            new Command(name: "simulate", description: "This can be used to simulate a large number of spins on a roulette wheel and play using various method. Reports will be generated in the output folder.") {
                CommandHandler.Create<SimulateCommandOptions>(ExecuteSimulateAsync),
                OptionOutputPath(),
                OptionFilenamePrefix(),
                OptionSettingsFilePath(),
                OptionNumberOfSpins(),
                OptionRouletteType(),
                OptionEnableCsvOutput(),
                OptionEnableNumberDetails(),
                OptionEnableConsoleLogger(),
                OptionEnableMartingale(),
                OptionEnableBondMartingale(),
                OptionEnableGreen(),
                OptionStopWhenBankrupt(),
                OptionInitialBankroll(),
                OptionMinimumBet(),
                OptionMaximumBet(),
                OptionAllowNegativeBankroll(),
                OptionVerbose()
            };

        private async Task ExecuteSimulateAsync(SimulateCommandOptions options) {
            _reporter.EnableVerbose = options.Verbose;

            if (options.RouletteType != RouletteType.American &&
                options.RouletteType != RouletteType.European) {
                throw new ArgumentException($"Value for roulette type must be '{RouletteType.American}' or '{RouletteType.European}'");
            }

            _reporter.WriteLine(_rouletteText);
            _reporter.WriteLine(string.Empty);
            if (options.Verbose) {
                WriteOptionsToLog(options);
            }

            var gameSettings = GetGameSettingsFrom(options);
            var watch = Stopwatch.StartNew();
            var gameController = new RouletteGameController(gameSettings, options.OutputPath, options.FilenamePrefix);
            await gameController.PlayAll();
            watch.Stop();
            Console.WriteLine($"num spins: {options.NumberOfSpins:N0}\ntime: {watch.Elapsed.TotalSeconds}\nfilenames: '{options.FilenamePrefix}*'");
        }

        private void WriteOptionsToLog(SimulateCommandOptions options) {
            _reporter.WriteLine($"outputPath: {options.OutputPath}");
            _reporter.WriteLine($"filename prefix: {options.FilenamePrefix}");
            _reporter.WriteLine($"settingsFilePath: {options.SettingsFilePath}");
            _reporter.WriteLine($"num spins: {options.NumberOfSpins}");
            _reporter.WriteLine($"roulette type: {options.RouletteType}");
            _reporter.WriteLine($"enableCsv: {options.EnableCsvOutput}");
            _reporter.WriteLine($"enableNumDetails: {options.EnableNumberDetails}");
            _reporter.WriteLine($"console logger: {options.EnableConsoleLogger}");
            _reporter.WriteLine($"martingale: {options.EnableMartingale}");
            _reporter.WriteLine($"bond martingale: {options.EnableBondMartingale}");
            _reporter.WriteLine($"green: {options.EnableGreen}");
            _reporter.WriteLine($"stop when bankrupt: {options.EnableStopWhenBankrupt}");
            _reporter.WriteLine($"initial bankroll: {options.InitialBankroll}");
            _reporter.WriteLine($"min bet: {options.MinimumBet}");
            _reporter.WriteLine($"max bet: {options.MaximumBet}");
            _reporter.WriteLine($"allow neg bankroll: {options.AllowNegativeBankroll}");
            _reporter.WriteLine($"verbose: {options.Verbose}");
        }

        protected GameSettings GetGameSettingsFrom(SimulateCommandOptions options) {
            var gameSettings = new GameSettings();

            if (options.NumberOfSpins != null && options.NumberOfSpins.HasValue) {
                gameSettings.NumberOfSpins = options.NumberOfSpins.Value;
            }
            if (options.EnableConsoleLogger != null && options.EnableConsoleLogger.HasValue) {
                gameSettings.EnableConsoleLogger = options.EnableConsoleLogger.Value;
            }
            if (options.EnableCsvOutput != null && options.EnableCsvOutput.HasValue) {
                gameSettings.EnableCsvFileOutput = options.EnableCsvOutput.Value;
            }
            if (options.EnableNumberDetails != null && options.EnableNumberDetails.HasValue) {
                gameSettings.EnableNumberDetails = options.EnableNumberDetails.Value;
            }
            if (options.EnableMartingale != null && options.EnableMartingale.HasValue) {
                gameSettings.EnableMartingale = options.EnableMartingale.Value;
            }
            if (options.EnableBondMartingale != null && options.EnableBondMartingale.HasValue) {
                gameSettings.EnableBondMartingale = options.EnableBondMartingale.Value;
            }
            if (options.EnableGreen != null && options.EnableGreen.HasValue) {
                gameSettings.EnableGreen = options.EnableGreen.Value;
            }
            if (options.EnableStopWhenBankrupt != null && options.EnableStopWhenBankrupt.HasValue) {
                gameSettings.StopWhenBankrupt = options.EnableStopWhenBankrupt.Value;
            }
            if (options.AllowNegativeBankroll != null && options.AllowNegativeBankroll.HasValue) {
                gameSettings.AllowNegativeBankroll = options.AllowNegativeBankroll.Value;
            }
            if (options.InitialBankroll != null && options.InitialBankroll.HasValue) {
                gameSettings.InitialBankroll = options.InitialBankroll.Value;
            }
            if (options.MinimumBet != null && options.MinimumBet.HasValue) {
                gameSettings.MinimumBet = options.MinimumBet.Value;
            }
            if (options.MaximumBet != null && options.MaximumBet.HasValue) {
                gameSettings.MaximumBet = options.MaximumBet.Value;
            }

            gameSettings.SetRouletteType(gameSettings.RouletteType);

            return gameSettings;
        }

        public class SimulateCommandOptions {
            public string SettingsFilePath { get; set; }
            public int? NumberOfSpins { get; set; }
            public RouletteType RouletteType { get; set; }
            public bool? EnableCsvOutput { get; set; }
            public bool? EnableNumberDetails { get; set; }
            public bool? EnableConsoleLogger { get; set; }
            public bool? EnableMartingale { get; set; }
            public bool? EnableBondMartingale { get; set; }
            public bool? EnableGreen { get; set; }
            public bool? EnableStopWhenBankrupt { get; set; }
            public string OutputPath { get; set; }
            public string FilenamePrefix { get; set; }
            public long? InitialBankroll { get; set; }
            public int? MinimumBet { get; set; }
            public long? MaximumBet { get; set; }
            public bool? AllowNegativeBankroll { get; set; }
            public bool Verbose { get; internal set; }
        }

        protected Option OptionSettingsFilePath() =>
        new Option(new string[] { "--settings-file-path" }, "Path to the roulette settings file (JSON) to configure how to play.") {
            Argument = new Argument<string>(name: "settings-file-path")
        };
        protected Option OptionNumberOfSpins() {
            var arg = new Argument<int>(name: "number-of-spins");
            arg.SetDefaultValue(10001);
            var opt = new Option(new string[] { "--number-of-spins" },
                "Number of spins of the roulette wheel. Default is 10001") {
                Argument = new Argument<int>(name: "number-of-spins")
            };

            return opt;
        }
        protected Option OptionRouletteType() =>
            new Option<RouletteType>(new string[] { "--roulette-type" },
                $"Sets the roulette type, values include 'American' and 'European', default is {RouletteType.American}") {
                Argument = new Argument<RouletteType>(
                    name: "roulette-type",
                    getDefaultValue: () => RouletteType.American).
                FromAmong(RouletteType.American.ToString(), RouletteType.European.ToString())
            };
        protected Option OptionEnableCsvOutput() =>
            new Option(new string[] { "--enable-csv-output" }, "Enables output of a .csv file with all the spins.") {
                Argument = new Argument<bool>("enable-csv-output")
            };
        protected Option OptionEnableNumberDetails() =>
            new Option(new string[] { "--enable-number-details" }, "Details on each individual number (cell) will be written to a file.") {
                Argument = new Argument<bool>("enable-number-details")
            };
        protected Option OptionEnableMartingale() =>
            new Option(new string[] { "--enable-martingale" }, "Enables martingale play style simulation.") {
                Argument = new Argument<bool>("enable-martingale")
            };
        protected Option OptionEnableBondMartingale() =>
            new Option(new string[] { "--enable-bond-martingale" }, "Enables Bond martingale play style simulation.") {
                Argument = new Argument<bool>("enable-bond-martingale")
            };
        protected Option OptionEnableGreen() =>
            new Option(new string[] { "--enable-green" }, "Enables playing on green cells only.") {
                Argument = new Argument<bool>("enable-green")
            };
        protected Option OptionEnableConsoleLogger() =>
            new Option(new string[] { "--enable-console-logger" }, "When enabled will log every spin to the console. Enabling this will significantly slow down the play time.") {
                Argument = new Argument<bool>("enable-console-logger")
            };
        protected Option OptionStopWhenBankrupt() =>
            new Option(new string[] { "--enable-stop-when-bankrupt" }, "When enabled the play will end when the bankroll goes to zero.") {
                Argument = new Argument<bool>("enable-stop-when-bankrupt")
            };
        // TODO: args to add: rollup-num-games,
        protected Option OptionOutputPath() =>
            new Option(new string[] { "--output-path" },
                $"Path to folder where the results should be written. Default is the current directory.") {
                Argument = new Argument<string>(
                    name: "output-path",
                    getDefaultValue: () => Path.GetFullPath(Directory.GetCurrentDirectory()))
            };
        protected Option OptionFilenamePrefix() =>
            new Option(new string[] { "--filename-prefix" }, "Prefix for all the files that are generated when the simulation is executed.") {
                Argument = new Argument<string>(name: "filename-prefix")
            };
        protected Option OptionInitialBankroll() =>
            new Option(new string[] { "--initial-bankroll" }, "The initial bankroll that the player has when starting the simulation.") {
                Argument = new Argument<long>(name: "initial-bankroll", getDefaultValue: () => 10000)
            };
        protected Option OptionMinimumBet() =>
            new Option(new[] { "--minimum-bet" }, "Minimum bet on each spin.") {
                Argument = new Argument<int>(name: "minimum-bet")
            };
        protected Option OptionMaximumBet() =>
            new Option(new[] { "--maximum-bet" }, "Maximum bet on each spin.") {
                Argument = new Argument<long>(name: "maximum-bet")
            };
        protected Option OptionAllowNegativeBankroll() =>
            new Option(new[] { "--allow-negative-bankroll" }, "When enabled a negative bankroll will be allowed, otherwise the game will end when the bankroll gets to zero.") {
                Argument = new Argument<bool>(name: "allow-negative-bankroll")
            };

        private readonly string _rouletteText = @"
                                   88
                                   88              ,d      ,d
                                   88              88      88
8b,dPPYba,  ,adPPYba,  88       88 88  ,adPPYba, MM88MMM MM88MMM ,adPPYba,
88P'   ""Y8 a8""     ""8a 88       88 88 a8P_____88   88      88   a8P_____88
88         8b       d8 88       88 88 8PP""""""""""""""   88      88   8PP""""""""""""""
88         ""8a,   ,a8"" ""8a,   ,a88 88 ""8b,   ,aa   88,     88,  ""8b,   ,aa
88          `""YbbdP""'   `""YbbdP'Y8 88  `""Ybbd8""'   ""Y888   ""Y888 `""Ybbd8""'

";
    }
}
