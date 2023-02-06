using System.CommandLine;
using System.CommandLine.Invocation;

namespace SayedHa.Roulette.Cli {
    public class SimulateCommand : CommandBase {
        private IReporter _reporter;
        public SimulateCommand(IReporter reporter) {
            _reporter = reporter;
        }
        public override Command CreateCommand() =>
            new Command(name: "simulate", description: "roulette simulator") {
                CommandHandler.Create<string, string, bool>(async (settingsFilePath, outputPath, verbose) => {
                    _reporter.EnableVerbose = verbose;
                    _reporter.WriteLine(_rouletteText);
                    _reporter.WriteLine(string.Empty);
                    _reporter.WriteLine($"settingsFilePath: {settingsFilePath}");
                    _reporter.WriteLine($"outputPath: {outputPath}");
                    _reporter.WriteLine($"verbose: {verbose}");
                    _reporter.WriteVerbose("verbose message here");
                    // added here to avoid async/await warning
                    await Task.Delay(1000);
                }),
                OptionSettingsFilePath(),
                OptionOutputPath(),
                OptionVerbose(),
            };
        protected Option OptionPackages() =>
            new Option(new string[] { "--paramname" }, "TODO: update param description") {
                Argument = new Argument<string>(name: "paramname")
            };

        protected Option OptionSettingsFilePath() =>
            new Option(new string[] { "--settings-file-path" }, "Path to the roulette settings file (JSON) to configure how to play.")
            {
                Argument = new Argument<string>(name:"settings-file-path")
            };
        // TODO: args to add: rollup-num-games,
        protected Option OptionOutputPath() =>
            new Option(new string[] { "--output-path" }, 
                $"Path to folder where the results should be written. Default is the current directory '{Path.GetFullPath(Directory.GetCurrentDirectory())}'.")
            {
                Argument = new Argument<string>(name: "output-path")
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
