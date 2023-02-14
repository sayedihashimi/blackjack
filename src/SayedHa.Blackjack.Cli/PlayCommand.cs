﻿using SayedHa.Blackjack.Shared;
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
            var betAmount = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                .Title("Initial bankroll?")
                .AddChoices(new[] { 5, 10, 25, 50, 75, 100 })
            );
            // var numDecks = AnsiConsole.Ask<int
            var cards = new CardDeckFactory().CreateCardDeck(numDecks, true);
            AnsiConsole.WriteLine("Shuffling cards");

            var pf = new ParticipantFactory(_reporter);

            var bs = BettingStrategy.CreateNewDefaultBettingStrategy(new Bankroll(initialBankroll, _reporter));
            pf = new ParticipantFactory(bs, OpponentPlayStrategy.BasicStrategy, _reporter);
            var dealerPlayer = pf.GetDefaultDealer();
            var humanPlayer = pf.CreateNewOpponent(OpponentPlayStrategy.BasicStrategy, _reporter);

            var humanHand = new Hand(betAmount, _reporter);
            var dealerHand = new DealerHand(_reporter);

            // deal all cards out and then display the cards to the user
            _ = humanHand.ReceiveCard(cards.GetCardAndMoveNext());
            _ = dealerHand.ReceiveCard(cards.GetCardAndMoveNext());
            _ = humanHand.ReceiveCard(cards.GetCardAndMoveNext());
            _ = dealerHand.ReceiveCard(cards.GetCardAndMoveNext());

            AnsiConsole.WriteLine("Dealing cards");
            AnsiConsole.WriteLine("Dealing to player, face up.");

            AnsiConsole.MarkupLine($"[red]Player[/] cards: {humanHand.ToString(hideFirstCard: false, useSymbols: true, includeScore: true, includeBrackets: false, includeResult: false).EscapeMarkup()}");
            AnsiConsole.MarkupLine($"[red]Dealer[/] cards: {humanHand.ToString(hideFirstCard: true, useSymbols: true, includeScore: true, includeBrackets: false, includeResult: false).EscapeMarkup()}");

            HandAction nextHandAction;
            do {
                nextHandAction = AnsiConsole.Prompt(
                    new SelectionPrompt<HandAction>()
                    .Title("Select your next action")
                    .AddChoices(new[] { HandAction.Stand, HandAction.Hit, HandAction.Double, HandAction.Split })
                    );
                AnsiConsole.MarkupLine($"Selected action: {nextHandAction}");

                if (nextHandAction == HandAction.Hit) {
                    _ = humanHand.ReceiveCard(cards.GetCardAndMoveNext());
                    AnsiConsole.MarkupLine($"[red]Player[/] cards: {humanHand.ToString(hideFirstCard: false, useSymbols: true, includeScore: true, includeBrackets: false, includeResult: false).EscapeMarkup()}");
                }
                if (humanHand.GetScore() > BlackjackSettings.GetBlackjackSettings().MaxScore) {
                    AnsiConsole.MarkupLine($"BUSTED");
                    humanHand.MarkHandAsClosed();
                }

            } while (nextHandAction != HandAction.Stand && humanHand.Status != HandStatus.Closed);
            AnsiConsole.WriteLine("done");
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
            var betAmount = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                .Title("Initial bankroll?")
                .AddChoices(new[] { 5, 10, 25, 50, 75, 100 })
            );

            BlackjackSettings.GetBlackjackSettings().CreateBettingStrategy = (bankroll) => new SpectreConsoleBettingStrategy(bankroll);

            var gameRunner = new GameRunner(_reporter);
            gameRunner.NextActionSelected += GameRunner_NextActionSelected;
            var bankroll = new Bankroll(initialBankroll, _reporter);
            var bettingStrategy = new SpectreConsoleBettingStrategy(bankroll);
            var pf = new SpectreConsoleParticipantFactory(bettingStrategy, _reporter);
            // TODO: Make this into a setting or similar
            var discardFirstCard = true;
            var game = gameRunner.CreateNewGame(numDecks, 1, pf, discardFirstCard);

            var gameResult = gameRunner.PlayGame(game);

            // print out the results of each hand now.
            foreach (var hand in gameResult.OpponentHands) {
                var sb = new StringBuilder();
                sb.Append($"Your hand:{hand.ToString(hideFirstCard: false, useSymbols: true, includeScore: true, includeBrackets: false, includeResult: false).EscapeMarkup()}");
                sb.AppendLine($",Dealer hand:{game.Dealer.Hands[0].ToString(hideFirstCard: false, useSymbols: true, includeScore: true, includeBrackets: false, includeResult: false).EscapeMarkup()}");
                sb.AppendLine($"Result = {hand.HandResult}");

                AnsiConsole.MarkupLine(sb.ToString().EscapeMarkup());
            }

            //AnsiConsole.MarkupLine($"{game.Opponents[0].BettingStrategy.Bankroll}");
            AnsiConsole.MarkupLine($"Balance = {gameResult.OpponentRemaining[0].remaining}, Change from original balance:{gameResult.OpponentRemaining[0].diff}");
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
            //AnsiConsole.MarkupLine($"Your hand:{hand.ToString(hideFirstCard: false, useSymbols: true, includeScore: true, includeBrackets: false, includeResult: false).EscapeMarkup()},Action:{nextActionArgs.NextAction}\n");
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
