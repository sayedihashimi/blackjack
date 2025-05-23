﻿// This file is part of SayedHa.Blackjack.
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
using SayedHa.Blackjack.Cli.Extensions;
using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Blackjack;
using Spectre.Console;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Transactions;
using System.Linq;
using SayedHa.Blackjack.Shared.Betting;

namespace SayedHa.Blackjack.Cli {
    public class PlayCommand : CommandBase {
        /// TODO: move this to CommandBase
        private readonly IReporter _reporter;
        private readonly BlackjackAppSettings _appSettings;
        private int NumDecks { get; set; }
        private SessionReportData _sessionReportData { get; set; }
        private int BetAmount { get; set; } = 0;
        public PlayCommand(IReporter reporter, BlackjackAppSettings appSettings) {
            _reporter = reporter;
            _appSettings = appSettings;
        }

        public override Command CreateCommand() =>
            new Command(name: "play", description: "Play blackjack") {
                CommandHandler.Create<PlayCommandOptions>(HandlePlayCommand),
                OptionVerbose()
            };

        private async Task HandlePlayCommand(PlayCommandOptions options) {
            if (AnsiConsole.Confirm("Blackjack: Ready to get started?")) {
                await PlayGameAsync();
            }
            else {
                Console.WriteLine("Come again soon!");
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

            var enableHints = AnsiConsole.Confirm("[bold]Enable hints?[/]", false);

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
            gameRunner.ShufflingCards += GameRunner_ShufflingCards;
            gameRunner.WrongNextActionSelected += GameRunner_WrongNextActionSelected;

            var bettingStrategy = new SpectreConsoleBettingStrategy(bankroll);
            var pf = new SpectreConsoleParticipantFactory(bettingStrategy, _reporter);
            BlackjackSettings.GetBlackjackSettings().CreateBettingStrategy = (bankroll) => new SpectreConsoleBettingStrategy(bankroll);
            // TODO: Make this into a setting or similar
            var discardFirstCard = true;
            var game = gameRunner.CreateNewGame(NumDecks, 1, pf, discardFirstCard);

            _sessionReportData = new SessionReportData(game.Opponents[0]);

            if (enableHints) {
                foreach (var participant in game.Opponents) {
                    participant.ValidateNextAction = true;
                }
            }

            PrintUI(game, ShouldHideFirstCard(game));
            do {
                var gameResult = gameRunner.PlayGame(game);
                PrintUI(game, false);
            } while (KeepPlaying(bankroll, game));

            if (enableHints) {
                WriteSummaryReport(_sessionReportData);
            }
        }

        private void WriteSummaryReport(SessionReportData sessionReportData) {
            Debug.Assert(sessionReportData != null);
            Console.Clear();
            // stats that we need to add in order to generate this report.
            // we can use the Participant.AllHands property for good info to add to the report.
            int numbets = sessionReportData.Player.Hands.Count;

            float sumbets = 0;
            Dictionary<HandResult, int> handResultAndCount = new Dictionary<HandResult, int>();

            int numGamesWon = 0;
            int numGamesLost = 0;
            foreach (var hand in sessionReportData.Player.Hands) {
                sumbets += hand.Bet;

                int countForThisResult;
                handResultAndCount.TryGetValue(hand.HandResult, out countForThisResult);
                if (countForThisResult < 1) {
                    countForThisResult = 1;
                }

                handResultAndCount[hand.HandResult] = countForThisResult;

                switch (hand.HandResult) {
                    case HandResult.OpponentWon:
                        numGamesWon++;
                        break;
                    case HandResult.DealerWon:
                        numGamesLost++;
                        break;
                }

                // TODO: Need to track the payout to get the average hand result (avg amt won/lost each hand)

            }

            float averagebet = sumbets / numbets;

            int numIncorrectActions = 0;
            foreach (var key in sessionReportData.WrongNextActionAndCount.Keys) {
                numIncorrectActions += sessionReportData.WrongNextActionAndCount[key];
            }
            var grid = new Table();
            grid.Title = new TableTitle("[bold]Session summary[/]");
            grid.Border = TableBorder.SimpleHeavy;
            grid.HideHeaders();
            grid.AddColumn(string.Empty);
            grid.AddColumn(string.Empty);

            grid.AddRow("Initial bankroll", $"{sessionReportData.Player.BettingStrategy.Bankroll.InitialBankroll:C0}");
            grid.AddRow("Final bankroll", 
                $"{sessionReportData.Player.BettingStrategy.Bankroll.DollarsRemaining:C0} diff: {sessionReportData.Player.BettingStrategy.Bankroll.DollarsRemaining- sessionReportData.Player.BettingStrategy.Bankroll.InitialBankroll:C0}");
            grid.AddRow("Number of hands played", $"{sessionReportData.Player.GetAllHands().Count}");
            grid.AddRow("Number of hands won", $"{numGamesWon}");
            grid.AddRow("Number of hands lost", $"{numGamesLost}");
            grid.AddRow("Avg bet amount", $"{averagebet:C0}");
            // TODO: grid.AddRow("Avg hand result", "");
            grid.AddRow("Number of wrong actions selected", $"{numIncorrectActions}");

            var infoPanel = new Panel("Session summary");

            // AnsiConsole.MarkupLine("[bold green]Session summary[/]");
            AnsiConsole.Write(grid);
            AnsiConsole.WriteLine();

            if(sessionReportData.WrongNextActionAndCount.Count > 0) {
                var errorsTable = new Table();
                errorsTable.Title = new TableTitle("[bold red]Summary of errors[/]");
                errorsTable.Border = TableBorder.SimpleHeavy;
                //errorsTable.AddColumn("Num times");
                errorsTable.AddColumn(new TableColumn("Num times").RightAligned());
                errorsTable.AddColumn("Guidance");

                foreach (KeyValuePair<string, int> item in sessionReportData.WrongNextActionAndCount.OrderByDescending(x => x.Value)) {
                    errorsTable.AddRow(item.Value.ToString(), item.Key);
                }

                AnsiConsole.Write(errorsTable);
            }

            AnsiConsole.MarkupLine("This is an open source app, code is available at [link]https://github.com/sayedihashimi/blackjack[/]");
        }

        private void GameRunner_WrongNextActionSelected(object sender, EventArgs e) {
            var wncEa = e as WrongNextActionSelected;
            if(wncEa is object) {
                AnsiConsole.MarkupLineInterpolated($"Correct action is [bold green]'{wncEa.CorrectAction.HandAction}'[/], you selected [bold red]'{wncEa.NextActionSelected}[/]'\n[italic]{wncEa.CorrectAction.Reason}[/]\n\nTry again.\n");
                _sessionReportData.AddWrongNextActionSelected(wncEa.NextActionSelected,wncEa.CorrectAction);
            }
        }

        private void GameRunner_ShufflingCards(object sender, EventArgs e) {
            AnsiConsole.Progress()
                .AutoRefresh(true)
                .AutoClear(false)
                .HideCompleted(false)
                .Columns(
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new SpinnerColumn()
                )
                .Start(ctx => {
                    var shuffletask = ctx.AddTask("shuffling cards");

                    while (!ctx.IsFinished) {
                        Thread.Sleep(100);
                        shuffletask.Increment(10);
                    }
                });
        }

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

            Console.Clear();
            // Crashing windows terminal for some reason
            // AnsiConsole.Clear();

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
                        var cardsStr = hand.GetSpectreString(isDealerHand: false, hideFirstCard: false, includeResult: showResult, includeBet: true);

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
                dealerCardsStr = game.Dealer.Hands[0].GetSpectreString(isDealerHand: true, hideFirstCard: hideDealerFirstCard);
                dealerCardsTable.AddRow(new Panel(dealerCardsStr));
            }

            // stats
            var statsGrid = new Grid();
            var overallGrid = new Grid();
            BarChart remainingCardsBarChart = null;
            var bankroll = game.Opponents?[0]?.BettingStrategy?.Bankroll;

            // this will be used to get the count only, numbers below don't matter
            var bs = new BasicHiLoStrategy(bankroll, 5, 20);
            var currentCount = bs.GetCount(game.Cards);
            if (bankroll != null) {
                remainingCardsBarChart = new BarChart()
                    .Width(60)
                    .WithMaxValue(game.Cards.GetTotalNumCards())
                    // .HideValues()
                    .AddItem("Remaining deck  ",game.Cards.GetNumRemainingCards(), Color.Orange1);

                statsGrid
                .Width(60)
                .AddColumn()

                .AddRow(new[] { $"Number of decks   {NumDecks}" })
                .AddRow(new[] { $"Initial bankroll  {bankroll.InitialBankroll:C0}" })
                .AddRow(new[] { $"Current bankroll  {bankroll.DollarsRemaining:C0} ({bankroll.DollarsRemaining - bankroll.InitialBankroll:C0})" })
                .AddRow(new[] { $"Running count     {currentCount.RunningCount}" })
                .AddRow(new[] { $"True count        {currentCount.TrueCount}" });

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

        private bool KeepPlaying(Bankroll bankroll, Game game) {
            if(bankroll.DollarsRemaining < 5) {
                var result = AnsiConsole.Confirm("[bold red]You don't have enough money to continue.[/]\nDo you want to reset back to your initial bankroll?");
                if (result) {
                    bankroll.DollarsRemaining = bankroll.InitialBankroll;
                    PrintUI(game, false);
                    AnsiConsole.MarkupInterpolated($"Your balance has been restored to [red]{bankroll.DollarsRemaining:C0}[/]\n");
                }
            }

            return AnsiConsole.Confirm("Keep playing?");
        }

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
