using SayedHa.Blackjack.Cli.Extensions;
using SayedHa.Blackjack.Shared;
using Spectre.Console;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace SayedHa.Blackjack.Cli {
    public class PlayCommand : CommandBase {
        /// TODO: move this to CommandBase
        private readonly IReporter _reporter;

        private int NumDecks { get; set; }
        private int BetAmount { get; set; }
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
            AnsiConsole.Write(
                new FigletText("Blackjack")
                    .LeftJustified()
                    .Color(Color.Red));

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
            // TODO: move this somewhere else
            // change negative numbers from ($123) to -$123
            CultureInfo.CurrentCulture = CultureInfo.CurrentCulture.Clone() as CultureInfo;
            CultureInfo.CurrentCulture.NumberFormat.CurrencyNegativePattern = 1;

            NumDecks = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                .Title("How many decks?")
                .AddChoices(new[] { 2, 4, 6, 8, 10 })
            );

            var initBankrollPrompt = new SelectionPrompt<int>()
                .Title("Initial bankroll?")
                .AddChoices(new[] { 1000, 10000, 100000 });
            initBankrollPrompt.Converter = amount => amount.ToString("C0");

            var initialBankroll = AnsiConsole.Prompt(initBankrollPrompt);

            var bankroll = new Bankroll(initialBankroll, _reporter);
            var gameRunner = new GameRunner(_reporter);
            // connect event handlers
            gameRunner.NextActionSelected += GameRunner_NextActionSelected;
            gameRunner.DealerHasBlackjack += GameRunner_DealerHasBlackjack;
            gameRunner.PlayerHasBlackjack += GameRunner_PlayerHasBlackjack;
            gameRunner.BetAmountConfigured += GameRunner_BetAmountConfigured;
            gameRunner.CardReceived += GameRunner_CardReceived;

            var bettingStrategy = new SpectreConsoleBettingStrategy(bankroll);
            var pf = new SpectreConsoleParticipantFactory(bettingStrategy, _reporter);
            BlackjackSettings.GetBlackjackSettings().CreateBettingStrategy = (bankroll) => new SpectreConsoleBettingStrategy(bankroll);
            // TODO: Make this into a setting or similar
            var discardFirstCard = true;
            var game = gameRunner.CreateNewGame(NumDecks, 1, pf, discardFirstCard);
            PrintUI(game, ShouldHideFirstCard(game));
            do {
                var gameResult = gameRunner.PlayGame(game);
                PrintUI(game, false);
            } while (KeepPlaying());
        }

        private string GetResultSpectreString(HandResult handResult) => handResult switch {
            HandResult.OpponentWon => "[bold green]You won[/]",
            HandResult.DealerWon => "[bold red]Dealer won[/]",
            HandResult.Push => "[bold green]Push ==[/]",
            HandResult.InPlay => "In play",
            
            _ => throw new ApplicationException($"Unknown value for HandResult: '{handResult}'")
        };
        private bool ShouldHideFirstCard(Game game) => game.Status switch {
            GameStatus.InPlay => true,
            GameStatus.DealerPlaying => false,
            GameStatus.Finished => false,
            _ => throw new ApplicationException($"Unknown value for GameStatus: '{game.Status}'")
        };

        private void PrintUI(Game game, bool hideDealerFirstCard) {
            Debug.Assert(game != null);
            Debug.Assert(game.Opponents?.Count > 0);
            Debug.Assert(game.Dealer != null);

            if(game.Opponents?.Count == 0 || game.Opponents?[0].Hands?.Count == 0 || game.Opponents?[0].Hands?[0].DealtCards?.Count == 0) {
                // wait until the hand is initialized to start printing the ui
                return;
            }

            AnsiConsole.Clear();

            var containerTable = new Table();
            containerTable.AddColumn("Player").Width(100);
            containerTable.AddColumn("Dealer").Width(100);
            containerTable.AddColumn("Stats");

            var cardTable = new Table();
            var cardsSb = new StringBuilder();
            foreach(var player in  game.Opponents) {
                cardTable.AddColumn(new TableColumn($"{player.Name} cards"));
                if(player.Hands?.Count > 0) {
                    for(int index = 0;index< player.Hands.Count; index++) {
                        var hand = player.Hands[index];
                        bool showResult = game.Status == GameStatus.Finished;
                        var cardsStr = hand.GetSpectreString(hideFirstCard: false, includeResult: showResult, includeBet: true);

                        if(index < player.Hands.Count - 1) {
                            cardsSb.AppendLine(cardsStr);
                        }
                        else {
                            cardsSb.Append(cardsStr);
                        }

                        cardTable.AddRow(new Panel(cardsStr));
                    }
                }
            }

            // dealer table
            var dealerCardsTable = new Table();
            string dealerCardsStr = string.Empty;
            dealerCardsTable.AddColumn(new TableColumn($"Dealer cards"));
            if(game.Dealer?.Hands?.Count > 0) {
                dealerCardsStr = game.Dealer.Hands[0].GetSpectreString(hideFirstCard: hideDealerFirstCard);
                dealerCardsTable.AddRow(new Panel(dealerCardsStr));
            }

            // stats
            var statsGrid = new Grid();
            var overallGrid = new Grid();
            BarChart remainingCardsBarChart = null;
            var bankroll = game.Opponents?[0]?.BettingStrategy?.Bankroll;

            if (bankroll != null) {
                remainingCardsBarChart = new BarChart()
                    .Width(60)
                    .WithMaxValue(game.Cards.GetTotalNumCards())
                    .HideValues()
                    .AddItem("Remaining deck  ",game.Cards.GetNumRemainingCards(), Color.Orange1);

                statsGrid
                .Width(60)
                .AddColumn()

                .AddRow(new[] { $"Number of decks   {NumDecks}"})
                .AddRow(new[] { $"Initial bankroll  {bankroll.InitialBankroll:C0}"})
                .AddRow(new[] { $"Current bankroll  {bankroll.DollarsRemaining:C0} ({bankroll.DollarsRemaining - bankroll.InitialBankroll:C0})" });

                overallGrid = new Grid()
                    .AddColumn()
                    .AddRow(statsGrid)
                    .AddRow(remainingCardsBarChart);
            }

            containerTable.AddRow(new Panel(cardsSb.ToString()), new Panel(dealerCardsStr), overallGrid);

            containerTable.Border = TableBorder.Minimal;
            AnsiConsole.Write(containerTable);
            if(game.Status == GameStatus.DealerPlaying) {
                AnsiConsole.MarkupLine("Dealer playing");
            }
        }

        private bool KeepPlaying() => AnsiConsole.Confirm("Keep playing?");

        private void GameRunner_CardReceived(object sender, EventArgs e) {
            var ge = e as GameEventArgs;
            if (ge is object) {
                if (ge.UpdateUi) {
                    PrintUI(ge.Game, ShouldHideFirstCard(ge.Game));
                }
            }
        }
        private void GameRunner_BetAmountConfigured(object sender, EventArgs e) {
            BetAmountConfiguredEventArgs be = e as BetAmountConfiguredEventArgs;
            if (be is object) {
                this.BetAmount = be.BetAmount;
                PrintUI(be.Game, ShouldHideFirstCard(be.Game));
                AnsiConsole.MarkupLine($"Bet amount: [green]{be.BetAmount:C0}[/]");
            }
        }
        private void GameRunner_PlayerHasBlackjack(object sender, EventArgs e) {
            var ge = e as GameEventArgs;
            if(ge is object) {
                PrintUI(ge.Game,ShouldHideFirstCard(ge.Game));
            }
            AnsiConsole.MarkupLine($"[red]Player has blackjack[/]");
        }

        private void GameRunner_DealerHasBlackjack(object sender, EventArgs e) {
            var ge = e as GameEventArgs;
            if (ge is object) {
                PrintUI(ge.Game, ShouldHideFirstCard(ge.Game));
            }
            AnsiConsole.MarkupLine($"[red]Dealer has blackjack[/]");
        }

        private void GameRunner_NextActionSelected(object sender, EventArgs e) {
            var nextActionArgs = e as NextActionSelectedEventArgs;
            if(nextActionArgs == null) {
                throw new ApplicationException("Error, nextActionArgs is null");
            }
            Debug.Assert(nextActionArgs.Hand != null);
            Debug.Assert(nextActionArgs.DealerHand != null);

            var ge = e as GameEventArgs;
            if (ge is object) {
                if(!nextActionArgs.IsDealerHand) {
                    // PrintUI(ge.Game, ShouldHideFirstCard(ge.Game));
                }
            }
        }

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
