namespace SayedHa.Blackjack.Shared.Players {
    /// <summary>
    /// Given a Hand, the Player will determine the next action.
    /// If the Hand is dead, no actions can be taken on the Hand. All Players should follow this rule.
    /// </summary>
    public abstract class Player {
        /// <summary>
        /// Given the hand, what should the next action be?
        /// For any closed hand the action should be Stand.
        /// Each player is free to implement whatever playing
        /// style they prefer.
        /// </summary>
        public abstract HandAction GetNextAction(Hand hand, Hand dealerHand);
    }
}
