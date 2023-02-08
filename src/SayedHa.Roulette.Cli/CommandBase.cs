using System.CommandLine;

namespace SayedHa.Roulette.Cli {
    public interface ICommand {
        Command CreateCommand();
    }
    public abstract class CommandBase : ICommand {
        public abstract Command CreateCommand();

        protected Option OptionVerbose() =>
            new Option(new string[] { "--verbose" }, "Enables verbose output.") {
                Argument = new Argument<bool>(name: "verbose")
            };

        public bool EnableVerbose { get; set; }
    }
}
