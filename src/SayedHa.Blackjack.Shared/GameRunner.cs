using System.Diagnostics;
using System.Text;

namespace SayedHa.Blackjack.Shared {
    public class GameRunner {
        public GameRunner(int numDecks, int numOpponents) {
            NumDecks = numDecks;
            NumOpponents = numOpponents;
        }
        // TODO: need to plumb in this logger in a better way
        private ILogger _logger = new Logger();
        private GameFactory gameFactory = new GameFactory();
        protected int NumDecks { get; init; }
        protected int NumOpponents { get; init; }

        public Game CreateNewGame(int numDecks = 4, int numOpponents = 1) =>
            gameFactory.CreateNewGame(numDecks, numOpponents);

        public void PlayGame(Game game) {
            Debug.Assert(game != null);
            Debug.Assert(game.Cards != null);
            // very first action for a new deck is to discard one card
            _ = game.Cards.GetCardAndMoveNext();

            if (game == null || game.Cards == null) {
                throw new ApplicationException();
            }

            // deal two cards to each opponent
            // TODO: move index to participant, maybe make a name property or something
            int index = 1;
            foreach (var opponent in game.Opponents) {
                _logger.LogLine($"dealing cards to opponent {index}");
                var newhand = new Hand();
                newhand.ReceiveCard(game.Cards.GetCardAndMoveNext()!);
                newhand.ReceiveCard(game.Cards.GetCardAndMoveNext()!);
                opponent.Hands.Add(newhand);
                // TODO: at some point we need to cover the case when game.Cards runs out of cards and needs to be shuffled again.
                index++;
            }

            // deal two cards to the dealer
            _logger.LogLine("Dealing cards to dealer (second card dealt is visible to the opponents)");
            var dealerHand = new Hand();
            dealerHand.ReceiveCard(game.Cards.GetCardAndMoveNext()!);
            dealerHand.ReceiveCard(game.Cards.GetCardAndMoveNext()!);
            game.Dealer.Hands.Add(dealerHand);

            if (!DoesDealerHaveBlackjack(dealerHand)) {
                // play each opponent now
                foreach (var opponent in game.Opponents) {
                    PlayForParticipant(opponent, game.Dealer, game.Cards);
                }
                _logger.LogLine(string.Empty);
                
                // now play for the dealer
                PlayForParticipant(game.Dealer, game.Dealer, game.Cards);

                // now determine the results of the game
                var dealerScore = game.Dealer.Hands[0].GetScore();

                _logger.LogLine(string.Empty);
                foreach (var opponent in game.Opponents) {
                    var sb = new StringBuilder();
                    sb.Append("Result: ");
                    foreach(var hand in opponent.Hands) {
                        var handScore = hand.GetScore();
                        if (handScore > 21) {
                            sb.Append("Busted ");
                        }
                        else if(handScore == dealerScore) {
                            sb.Append("Push ");
                        }
                        else if(handScore > dealerScore) {
                            sb.Append("Win ");
                        }
                        else {
                            sb.Append("Lose ");
                        }
                    }
                    _logger.LogLine(sb.ToString());
                }

            }
            else {
                _logger.LogLine("Dealer has blackjack, you lose");
            }            
        }

        private bool DoesDealerHaveBlackjack(Hand dealerHand) => (dealerHand.DealtCards[0].Number, dealerHand.DealtCards[1].Number) switch {
            (CardNumber.Ace, CardNumber.Ten) => true,
            (CardNumber.Ace, CardNumber.Jack) => true,
            (CardNumber.Ace, CardNumber.Queen) => true,
            (CardNumber.Ace, CardNumber.King) => true,
            (CardNumber.Ten, CardNumber.Ace) => true,
            (CardNumber.Jack, CardNumber.Ace) => true,
            (CardNumber.Queen, CardNumber.Ace) => true,
            (CardNumber.King, CardNumber.Ace) => true,
            _ => false
        };

        protected void PlayForParticipant(Participant participant, Participant dealer, CardDeck cards) {
            Debug.Assert(participant != null);
            Debug.Assert(participant.Hands != null);
            Debug.Assert(participant.Hands.Count == 1);
            // we need to play the hand for the opponent now
            // if a split occurs, we need to create a new hand and play each hand seperately
            _logger.LogLine($"playing for '{participant.Role}'");
            PlayHand(participant.Hands[0], dealer.Hands[0], participant, cards);
        }

        // returns a list because a hand can be split
        protected List<Hand> PlayHand(Hand hand, Hand dealerHand, Participant participant, CardDeck cards) {
            Debug.Assert(hand != null);
            Debug.Assert(participant != null);
            Debug.Assert(cards != null);

            var nextAction = participant.Player.GetNextAction(hand, dealerHand);

            // if the nextAction is to split we need to create two hands and deal a new card to each hand
            if (nextAction == HandAction.Split) {
                _logger.LogLine($"action = split. Hand={hand}");
                var newHand = new Hand();
                newHand.ReceiveCard(hand.DealtCards[1]);
                hand.DealtCards.RemoveAt(1);
                participant.Hands.Add(newHand);

                // deal a card to each hand now
                hand.ReceiveCard(cards.GetCardAndMoveNext()!);
                newHand.ReceiveCard(cards.GetCardAndMoveNext()!);

                // note: recursion below
                // return the result of playing both of these hands
                var result = new List<Hand>();
                result.AddRange(PlayHand(hand, dealerHand, participant, cards));
                result.AddRange(PlayHand(newHand, dealerHand, participant, cards));
                return result;
            }

            // by the time that we get here the NextAction shoudln't be split
            int maxNumIterations = 20;
            int numIterations = 0;
            while (hand.Status != HandStatus.Closed && (numIterations++ < maxNumIterations)) {
                PlayNextAction(hand, dealerHand, participant, cards);
            }

            return participant.Hands;
        }
        protected void PlayNextAction(Hand hand, Hand dealerHand, Participant participant, CardDeck cards) {
            // next action at this point shouldn't be split.
            // splits should have already been taken care of at this point
            var nextAction = participant.Player.GetNextAction(hand, dealerHand);
            _logger.LogLine($"  action: {nextAction}, Hand={hand}");
            switch (nextAction) {
                case HandAction.Split: throw new ApplicationException("no splits here");
                case HandAction.Stand:
                    hand.Status = HandStatus.Closed;
                    break;
                case HandAction.Hit:
                    hand.ReceiveCard(cards.GetCardAndMoveNext()!);
                    // note: recursion below
                    PlayNextAction(hand, dealerHand, participant, cards);
                    break;
                case HandAction.Double:
                    hand.ReceiveCard(cards.GetCardAndMoveNext()!);
                    hand.Status = HandStatus.Closed;
                    break;
                default:
                    throw new ApplicationException($"unknown value for nextAction:'{nextAction}'");
            }
        }
    }
}
