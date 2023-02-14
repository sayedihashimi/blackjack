using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Betting;
using Spectre.Console;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Text;

namespace SayedHa.Blackjack.Cli {
    public class PlayCommand : CommandBase {
        /// TODO: move this to CommandBase
        private readonly IReporter _reporter;

        public PlayCommand(IReporter reporter) {
            _reporter = reporter;
        }

        public override Command CreateCommand() =>
            new Command(name: "play", description: "Play blackjack") {
                CommandHandler.Create<PlayCommandOptions>(HandlePlayCommand),
                OptionVerbose()
            };

        private async Task HandlePlayCommand(PlayCommandOptions options) {
            // wait for the user to start the game
            if (AnsiConsole.Confirm("Ready to get started?")) {
                await PlayGameAsync();
            }
            else {
                Console.WriteLine("cancelled");
            }
        }

        private Task PlayGameAsync() {
            PlayGame();
            return Task.CompletedTask;
        }

        private void PlayGame() {
            var numDecks = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                .Title("How many decks?")
                .AddChoices(new[] { 2, 4, 6, 8, 10 })
            );
            var initialBankroll = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                .Title("Initial bankroll?")
                .AddChoices(new[] { 1000, 10000, 100000 })
            );
            //var betAmount = AnsiConsole.Prompt(
            //    new SelectionPrompt<int>()
            //    .Title("Initial bankroll?")
            //    .AddChoices(new[] { 5, 10, 25, 50, 75, 100 })
            //);

            var bankroll = new Bankroll(initialBankroll, _reporter);
            var gameRunner = new GameRunner(_reporter);
            gameRunner.NextActionSelected += GameRunner_NextActionSelected;
            gameRunner.DealerHasBlackjack += GameRunner_DealerHasBlackjack;
            gameRunner.PlayerHasBlackjack += GameRunner_PlayerHasBlackjack;
            var bettingStrategy = new SpectreConsoleBettingStrategy(bankroll);
            var pf = new SpectreConsoleParticipantFactory(bettingStrategy, _reporter);
            BlackjackSettings.GetBlackjackSettings().CreateBettingStrategy = (bankroll) => new SpectreConsoleBettingStrategy(bankroll);
            // TODO: Make this into a setting or similar
            var discardFirstCard = true;
            var game = gameRunner.CreateNewGame(numDecks, 1, pf, discardFirstCard);
            do {
                var gameResult = gameRunner.PlayGame(game);

                // print out the results of each hand now.
                foreach (var hand in gameResult.OpponentHands) {
                    var sb = new StringBuilder();
                    sb.Append($"Your hand:{hand.ToString(hideFirstCard: false, useSymbols: true, includeScore: true, includeBrackets: false, includeResult: false).EscapeMarkup()}");
                    sb.AppendLine($",Dealer hand:{game.Dealer.Hands[0].ToString(hideFirstCard: false, useSymbols: true, includeScore: true, includeBrackets: false, includeResult: false).EscapeMarkup()}");
                    sb.AppendLine($"Result = {hand.HandResult}");

                    AnsiConsole.MarkupLine(sb.ToString().EscapeMarkup());
                }

                AnsiConsole.MarkupLine($"Balance = {gameResult.OpponentRemaining[0].remaining:C0}, Change from original balance:{gameResult.OpponentRemaining[0].diff:C0}");
            } while (KeepPlaying());
        }

        private bool KeepPlaying() => AnsiConsole.Confirm("Keep playing?");

        private void GameRunner_PlayerHasBlackjack(object sender, EventArgs e) {
            AnsiConsole.MarkupLine($"[red]Player has blackjack[/]");
        }

        private void GameRunner_DealerHasBlackjack(object sender, EventArgs e) {
            AnsiConsole.MarkupLine($"[red]Dealer has blackjack[/]");
        }

        private void GameRunner_NextActionSelected(object sender, EventArgs e) {
            var nextActionArgs = e as NextActionSelectedEventArgs;
            if(nextActionArgs == null) {
                throw new ApplicationException("Error, nextActionArgs is null");
            }
            Debug.Assert(nextActionArgs.Hand != null);
            Debug.Assert(nextActionArgs.DealerHand != null);

            var sb = new StringBuilder();
            if (!nextActionArgs.IsDealerHand) {
                sb.Append($"Your hand:{nextActionArgs.Hand.ToString(hideFirstCard: false, useSymbols: true, includeScore: true, includeBrackets: false, includeResult: false).EscapeMarkup()}");
                sb.Append($",Dealer hand:{nextActionArgs.DealerHand.ToString(hideFirstCard: true, useSymbols: true, includeScore: true, includeBrackets: false, includeResult: false).EscapeMarkup()}");
                sb.AppendLine($",Action={nextActionArgs.NextAction}");
            }
            else {
                sb.Append("Dealer playing.");
                sb.Append($" Dealer hand:{nextActionArgs.DealerHand.ToString(hideFirstCard: false, useSymbols: true, includeScore: true, includeBrackets: false, includeResult: false).EscapeMarkup()}");
                sb.AppendLine($",Action={nextActionArgs.NextAction}");
            }

            AnsiConsole.WriteLine(sb.ToString().EscapeMarkup());
        }

        private HandAction GetNextActionFromUser(Hand hand) => AnsiConsole.Prompt(
            new SelectionPrompt<HandAction>()
            .Title($"Your hand:{hand.ToString(hideFirstCard: false, useSymbols: true, includeScore: true, includeBrackets: false, includeResult: false).EscapeMarkup()}\nSelect your next action.")
            .AddChoices(new[] { HandAction.Stand, HandAction.Hit, HandAction.Double, HandAction.Split })
            );

        private int GetNumDecks() {
            var numDecks = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                .Title("How many decks?")
                .AddChoices(new[] { 2, 4, 6, 8, 10 })
            );

            return numDecks;
        }

        public class PlayCommandOptions {
            public bool Verbose { get; internal set; }
        }
    }
}
