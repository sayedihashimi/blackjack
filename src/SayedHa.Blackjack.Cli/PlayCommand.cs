using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Betting;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Linq.Expressions;
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
                CommandHandler.Create<PlayCommandOptions>(HandlePlayCommand),
                OptionVerbose()
            };

        private async Task HandlePlayCommand(PlayCommandOptions options) {
            // wait for the user to start the game
            if (AnsiConsole.Confirm("Ready to get started?")) {
                ToDelete();

                await PlayGameAsync();

                //_ = AnsiConsole.Prompt(
                //    new TextPrompt<string>("Press Enter/Return to continue")
                //    .AllowEmpty());
            }
            else {
                Console.WriteLine("cancelled");
            }
        }

        private async Task PlayGameAsync() {
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

            AnsiConsole.MarkupLine($"[red]Player[/] cards: {humanHand.ToString(hideFirstCard: false,useSymbols:true,includeScore:true,includeBrackets:false,includeResult:false).EscapeMarkup()}");
            AnsiConsole.MarkupLine($"[red]Dealer[/] cards: {humanHand.ToString(hideFirstCard: true, useSymbols: true, includeScore: true, includeBrackets: false, includeResult: false).EscapeMarkup()}");

            AnsiConsole.WriteLine("done");

        }

        private int GetNumDecks() {
            var numDecks = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                .Title("How many decks?")
                .AddChoices(new[] { 2,4,6,8,10 })
            );

            return numDecks;
        }

        private void ToDelete() {

            // string str = $@"🂡	🂱	🃁	🃑 🂢	🂲	🃂	🃒 🂣	🂳	🃃	🃓 🂤	🂴	🃄	🃔 🂥	🂵	🃅	🃕 🂦	
            //🂶	🃆	🃖 🂧	🂷	🃇	🃗 🂨	🂸	🃈	🃘 🂩	🂹	🃉	🃙 🂪	🂺	🃊	🃚 🂫	
            //🂻	🃋	🃛 🂬	🂼	🃌	🃜 🂭	🂽	🃍	🃝 🂮	🂾	🃎	🃞 🂠	🂿	🃏	🃟";
            // AnsiConsole.WriteLine(str);

            AnsiConsole.Console.Clear();
            
            AnsiConsole.Write(
                new FigletText("blackjack")
                .LeftJustified()
                .Color(Color.Green));


        }

        public class PlayCommandOptions {
            public bool Verbose { get; internal set; }
        }
    }
}
