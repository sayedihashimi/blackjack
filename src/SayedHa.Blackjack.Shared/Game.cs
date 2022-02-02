using SayedHa.Blackjack.Shared.Players;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace SayedHa.Blackjack.Shared {
    public class Game {
        public Game(CardDeck cards, Participant dealer, List<Participant> opponents) {
            Cards = cards;
            Dealer = dealer;
            Opponents = opponents;
        }

        public GameStatus Status { get; set; } = GameStatus.InPlay;

        public CardDeck Cards { get; internal init; }
        protected internal Participant Dealer { get; internal init; }
        protected internal List<Participant> Opponents { get; internal init; }
    }
    public enum GameStatus {
        InPlay,
        Finished
    }

    public class GameFactory {
        public Game CreateNewGame(int numDecks = 4, int numOpponents = 1) {
            Debug.Assert(numDecks > 0);
            Debug.Assert(numOpponents > 0);

            var cards = new CardDeckFactory().GetDeckStandardDeckOfCards(numDecks);
            var pf = new ParticipantFactory();
            var dealerPlayer = pf.GetDefaultDealer();
            var opponents = new List<Participant>();

            for (var i = 0; i < numOpponents; i++) {
                opponents.Add(pf.GetDefaultOpponent());
            }

            return new Game(cards, dealerPlayer, opponents);
        }
    }
}
