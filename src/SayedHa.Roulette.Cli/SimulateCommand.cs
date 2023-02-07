using SayedHa.Blackjack.Shared.Roulette;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Drawing;

namespace SayedHa.Roulette.Cli {
    public class SimulateCommand : CommandBase {
        private IReporter _reporter;
        public SimulateCommand(IReporter reporter) {
            _reporter = reporter;
        }
        public Command CreateCommand2() =>
            new Command(name: "simulate", description: "roulette simulator") {
                CommandHandler.Create<string, int,RouletteType,bool,bool,string, bool>(async (settingsFilePath, numberOfSpins,rouletteType,enableCsvOutput,enableNumberDetails,outputPath, verbose) => {
                    _reporter.EnableVerbose = verbose;
                    _reporter.WriteLine(_rouletteText);
                    _reporter.WriteLine(string.Empty);
                    _reporter.WriteLine($"settingsFilePath: {settingsFilePath}");
                    _reporter.WriteLine($"num spins: {numberOfSpins}");
                    _reporter.WriteLine($"roulette type: {rouletteType}");
                    _reporter.WriteLine($"enableCsv: {enableCsvOutput}");
                    _reporter.WriteLine($"enableNumDetails: {enableNumberDetails}");
                    _reporter.WriteLine($"outputPath: {outputPath}");
                    _reporter.WriteLine($"verbose: {verbose}");
                    _reporter.WriteVerbose("verbose message here");
                    // added here to avoid async/await warning
                    await Task.Delay(1000);
                }),
                OptionSettingsFilePath(),
                OptionNumberOfSpins(),
                OptionRouletteType(),
                OptionEnableCsvOutput(),
                OptionEnableNumberDetails(),
                OptionOutputPath(),
                OptionVerbose()
                
            };

        public override Command CreateCommand() {
            var cmd = new Command(name: "simulate", description: "roulette simulator");
            cmd.AddOption(OptionSettingsFilePath());
            cmd.AddOption(OptionNumberOfSpins());
            cmd.AddOption(OptionRouletteType());
            cmd.AddOption(OptionEnableCsvOutput());
            cmd.AddOption(OptionEnableNumberDetails());
            cmd.AddOption(OptionOutputPath());
            cmd.AddOption(OptionVerbose());
            cmd.Handler = CommandHandler.Create<SimulateCommandOptions>(ExecuteSimulateAsync);
            return cmd;
        }

        private async Task ExecuteSimulateAsync(SimulateCommandOptions options) {
            _reporter.EnableVerbose = options.Verbose;
            _reporter.WriteLine(_rouletteText);
            _reporter.WriteLine(string.Empty);
            _reporter.WriteLine($"settingsFilePath: {options.SettingsFilePath}");
            _reporter.WriteLine($"num spins: {options.NumberOfSpins}");
            _reporter.WriteLine($"roulette type: {options.RouletteType}");
            _reporter.WriteLine($"enableCsv: {options.EnableCsvOutput}");
            _reporter.WriteLine($"enableNumDetails: {options.EnableNumberDetails}");
            _reporter.WriteLine($"outputPath: {options.OutputPath}");
            _reporter.WriteLine($"verbose: {options.Verbose}");
            _reporter.WriteVerbose("verbose message here");
            // added here to avoid async/await warning
            await Task.Delay(1000);
        }

        public class SimulateCommandOptions {
            public string SettingsFilePath { get; set; }
            public int NumberOfSpins { get; set; }
            public RouletteType RouletteType { get; set; }
            public bool EnableCsvOutput { get; set; }
            public bool EnableNumberDetails { get; set; }
            public string OutputPath { get; set; }
            public bool Verbose { get; internal set; }
        }

            protected Option OptionSettingsFilePath() =>
            new Option(new string[] { "--settings-file-path" }, "Path to the roulette settings file (JSON) to configure how to play.") {
                Argument = new Argument<string>(name:"settings-file-path")
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
        protected Option OptionRouletteType() {
            //var rArg = new Argument<RouletteType>("roulette-type");
            //rArg.SetDefaultValue(RouletteType.American);
            var option = new Option(new string[] {"--roulette-type"},
                $"Sets the roulette type, values include 'American' and 'European', default is {RouletteType.American}") {
                Argument = new Argument<RouletteType>(
                    getDefaultValue: () => RouletteType.American)
            };

            return option;
        }
        protected Option OptionConsoleColor() => new Option<string>("--color");

        protected Option OptionEnableCsvOutput() =>
            new Option(new string[] { "--enable-csv-output" }, "Enables output of a .csv file with all the spins.") {
                Argument = new Argument<bool>("enable-csv-output")
            };
        protected Option OptionEnableNumberDetails() =>
            new Option(new string[] { "--enable-number-details" }, "Details on each individual number (cell) will be written to a file.") {
                Argument = new Argument<bool>("enable-number-details")
            };
        protected Option OptionEnableMartingale() =>
            new Option(new string[] {"--enable-martingale"}, "Enables martingale play style simulation.") {
                Argument = new Argument<bool>("enable-martingale")
            };
        protected Option OptionEnableBondMartingale() =>
            new Option(new string[] {"--enable-bond-martingale"}, "Enables Bond martingale play style simulation.") {
                Argument = new Argument<bool>("enable-bond-martingale")
            };
        protected Option OptionEnableGreen() =>
            new Option(new string[] {"--enable-green"}, "Enables playing on green cells only.") {
                Argument = new Argument<bool>("enable-green")
            };
        protected Option OptionStopWhenBankrupt() =>
            new Option(new string[] { }, "--enable-stop-when-bankrupt") {
                Argument = new Argument<bool>("enable-stop-when-bankrupt")
            };

        // TODO: args to add: rollup-num-games,
        protected Option OptionOutputPath() =>
            new Option(new string[] { "--output-path" }, 
                $"Path to folder where the results should be written. Default is the current directory '{Path.GetFullPath(Directory.GetCurrentDirectory())}'.")
            {
                Argument = new Argument<string>(name: "output-path")
            };

        

        private void Test1() {
            var opt = new Option(new string[] {"--roulette-type"}, 
                "Selects American or European Roulette cells (0|00|etc.). Default is American. Valid values include 'American' and 'European'") {
                //Argument = new Argument<RouletteType>("roulette-type") //.SetDefaultValue(RouletteType.American)
                Argument = new Argument<RouletteType>(
                    getDefaultValue: () => RouletteType.American)
            };
            opt.Argument.SetDefaultValue(RouletteType.American);

            var rArg = new Argument<RouletteType>("roulette-type");
            rArg.SetDefaultValue(RouletteType.American);


            //var opt = new Option<string>("--{owner}",
            //        parseArgument: argResult => {
            //            if (argResult.Tokens.Any()) {
            //                // this is the user input, if any:
            //                return argResult.Tokens.Single().Value;
            //            }

            //            //if (Configuration.GetSection("owner") is { } section) {
            //            //    // ...and here's the vaue from config, if any...
            //            //    return section.Value;
            //            //}

            //            // ...and finally this error message will be shown to the user if no config value was found:
            //            argResult.ErrorMessage = "owner information is missing";
            //            return null;
            //        },
            //        isDefault: true, // This means the parseArgument delegate will be called even if the user didn't enter a value for this option.
            //        description: $"Repository owner or organization") {
            //    Required = true
            //};
        }

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
