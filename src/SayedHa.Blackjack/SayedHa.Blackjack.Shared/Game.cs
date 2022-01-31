namespace SayedHa.Blackjack.Shared {
    public class Game {
        public CardDeck? Card { get; internal init; }
        protected internal Dealer? Dealer { get; set; }
        protected internal List<Opponent>? Opponents { get; set; }
    }
}
