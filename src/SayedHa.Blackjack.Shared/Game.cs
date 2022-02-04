using SayedHa.Blackjack.Shared.Players;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace SayedHa.Blackjack.Shared {
    public class Game {
        public Game(CardDeck cards, Participant dealer, List<Participant> opponents, int shuffleThresholdPercent=KnownValues.DefaultShuffleThresholdPercent) {
            Cards = cards;
            Dealer = dealer;
            Opponents = opponents;
            ShuffleThresholdPercent = shuffleThresholdPercent;
        }

        public GameStatus Status { get; set; } = GameStatus.InPlay;

        public CardDeck Cards { get; internal init; }
        protected internal Participant Dealer { get; internal init; }
        protected internal List<Participant> Opponents { get; internal init; }
        public int ShuffleThresholdPercent { get; internal init; }
    }
    public enum GameStatus {
        InPlay,
        Finished
    }

    public class GameFactory {
        public Game CreateNewGame(
                int numDecks,
                int numOpponents,
                OpponentPlayStrategy opponentPlayStrategy,
                int shuffleThresholdPercent = KnownValues.DefaultShuffleThresholdPercent,
                ILogger? logger = null) {

            logger = logger ?? new NullLogger();
            Debug.Assert(numDecks > 0);
            Debug.Assert(numOpponents > 0);

            var cards = new CardDeckFactory().GetDeckStandardDeckOfCards(numDecks,true);
            var pf = new ParticipantFactory();
            var dealerPlayer = pf.GetDefaultDealer();
            var opponents = new List<Participant>();

            for (var i = 0; i < numOpponents; i++) {
                opponents.Add(pf.CreateNewOpponent(opponentPlayStrategy, logger));
            }

            return new Game(cards, dealerPlayer, opponents);
        }
    }
    public enum OpponentPlayStrategy {
        BasicStrategy,
        StandOn12,
        StandOn13,
        StandOn14,
        StandOn15,
        StandOn16,
        // default for the dealer as well as StandOnValuePlayer
        StandOn17,
        StandOn18,
        StandOn19,
        StandOn20,
        AlwaysStand,
        Random
    }
}
