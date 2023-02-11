using SayedHa.Blackjack.Shared;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Cli {
    public class PlayCommand : CommandBase {
        /// TODO: move this to CommandBase
        private readonly IReporter _reporter;
        public PlayCommand(IReporter reporter) {
            _reporter = reporter;
        }

        public override Command CreateCommand() =>
            new Command(name: "play", description: "Play blackjack") {
                CommandHandler.Create<PlayCommandOptions>(StartPlay),
                OptionVerbose()
            };

        private Task StartPlay(PlayCommandOptions options) {
            return Task.CompletedTask;
        }
        public class PlayCommandOptions {
            public bool Verbose { get; internal set; }
        }
    }
}
