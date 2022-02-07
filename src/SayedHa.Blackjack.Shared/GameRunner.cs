// This file is part of SayedHa.Blackjack.
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
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;

namespace SayedHa.Blackjack.Shared {
    public class GameRunner {
        public GameRunner(ILogger logger) {
            _logger = logger;
        }

        private ILogger _logger = new NullLogger();
        private GameFactory gameFactory = new GameFactory();

        public Game CreateNewGame(int numDecks, int numOpponents, ParticipantFactory participantFactory, bool discardFirstCard) {
            var game = gameFactory.CreateNewGame(numDecks, numOpponents, participantFactory, KnownValues.DefaultShuffleThresholdPercent, _logger);

            if (discardFirstCard) {
                // very first action for a new deck is to discard one card.
                _ = game.Cards.GetCardAndMoveNext();
            }

            return game;
        }

        protected void ShuffleCardsIfNeeded(Game game) {
            Debug.Assert(game != null);
            Debug.Assert(game.Cards != null);
            Debug.Assert(game.Cards.GetNumRemainingCards() > 0);

            var cards = game.Cards;

            // if the discarded # of cards exceeds specified amount, shuffle the cards
            var percentRemainingCards = (float)cards.GetNumRemainingCards() / (float)cards.GetTotalNumCards();
            if (percentRemainingCards * 100 <= game.ShuffleThresholdPercent) {
                _logger.LogLine("**** shuffling cards");
                game.Cards.ShuffleCards();
                // discard the first card after every shuffle
                _ = game.Cards.GetCardAndMoveNext();
            }
        }

        public GameResult PlayGame(Game game) {
            Debug.Assert(game != null);
            Debug.Assert(game.Cards != null);

            // need to reset the hands
            game.Dealer.Hands.Clear();
            foreach(var op in game.Opponents) {
                op.Hands.Clear();
            }
            // check to see if the deck needs to be shuffled or not
            ShuffleCardsIfNeeded(game);

            // TODO: Move this somewhere else, it's being called too much
            // very first action for a new deck is to discard one card.
            _ = game.Cards.GetCardAndMoveNext();

            if (game == null || game.Cards == null) {
                throw new ApplicationException();
            }

            // TODO: move this
            // int betAmount = 5;

            // deal two cards to each opponent
            // TODO: move index to participant, maybe make a name property or something
            int index = 1;
            foreach (var opponent in game.Opponents) {
                _logger.LogLine($"dealing cards to opponent {index}");
                int betAmount = opponent.BettingStrategy.GetNextBetAmount();
                var newhand = new Hand(betAmount, _logger);
                newhand.ReceiveCard(game.Cards.GetCardAndMoveNext()!);
                newhand.ReceiveCard(game.Cards.GetCardAndMoveNext()!);
                opponent.Hands.Add(newhand);

                index++;
            }

            // deal two cards to the dealer
            _logger.LogLine("Dealing cards to dealer (second card dealt is visible to the opponents)");
            var dealerHand = new DealerHand(_logger);
            dealerHand.ReceiveCard(game.Cards.GetCardAndMoveNext()!);
            dealerHand.ReceiveCard(game.Cards.GetCardAndMoveNext()!);
            game.Dealer.Hands.Add(dealerHand);

            // TODO: Change how the flow works should be more like:
            //  1) Does dealer have blackjack? => gane over
            //  2) Does player have blackjack? => 3:2 payout to that hand
            //  3) Regular play

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
                    foreach (var hand in opponent.Hands) {
                        var handScore = hand.GetScore();
                        if (handScore > 21) {
                            hand.SetHandResult(HandResult.DealerWon);
                            opponent.BettingStrategy.Bankroll.AddToDollarsRemaining(hand.Bet * -1, opponent.Name);
                            game.Dealer.BettingStrategy.Bankroll.AddToDollarsRemaining(hand.Bet, game.Dealer.Name);
                            sb.Append($"Busted -{hand.Bet} ");
                        }
                        else if (handScore == dealerScore) {
                            hand.SetHandResult(HandResult.Push);
                            // no change to any bankroll on a push
                            sb.Append("Push ");
                        }
                        else if (handScore > dealerScore || dealerScore > 21) {
                            hand.SetHandResult(HandResult.OpponentWon);
                            float betMultiplier = 1;
                            betMultiplier = handScore == 21 ? 3 / 2 : 1;
                            var amtToAdd = hand.Bet * betMultiplier;
                            opponent.BettingStrategy.Bankroll.AddToDollarsRemaining(amtToAdd, opponent.Name);
                            game.Dealer.BettingStrategy.Bankroll.AddToDollarsRemaining(amtToAdd*-1, game.Dealer.Name);
                            sb.Append($"Win {amtToAdd}");
                        }
                        else {
                            hand.SetHandResult(HandResult.DealerWon);
                            opponent.BettingStrategy.Bankroll.AddToDollarsRemaining(hand.Bet * -1, opponent.Name);
                            game.Dealer.BettingStrategy.Bankroll.AddToDollarsRemaining(hand.Bet, game.Dealer.Name);
                            sb.Append($"Lose -{hand.Bet} ");
                        }
                    }
                    _logger.LogLine(sb.ToString());
                }

            }
            else {
                foreach (var op in game.Opponents) {
                    foreach (var hand in op.Hands) {
                        hand.SetHandResult(HandResult.DealerWon);
                        op.BettingStrategy.Bankroll.AddToDollarsRemaining(hand.Bet * -1, op.Name);
                        game.Dealer.BettingStrategy.Bankroll.AddToDollarsRemaining(hand.Bet, game.Dealer.Name);
                    }
                    _logger.LogLine("Dealer has blackjack, you lose");
                }
            }

            var gameResults = new List<GameResult>();
            var allHands = new List<Hand>();
            foreach (var op in game.Opponents) {
                allHands.AddRange(op.Hands);
            }

            return new GameResult(game.Dealer.Hands[0], allHands,game.Dealer, game.Opponents);
        }

        private bool DoesHandHaveBlackjack(Hand dealerHand) => (dealerHand.DealtCards[0].Number, dealerHand.DealtCards[1].Number) switch {
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
        /// <summary>
        /// For the dealer to have blackjack the visible card must be an ace
        /// </summary>
        /// <param name="dealerHand"></param>
        /// <returns></returns>
        private bool DoesDealerHaveBlackjack(DealerHand dealerHand) => (dealerHand.DealersVisibleCard!.Number, dealerHand.DealersHiddenCard!.Number) switch {
            (CardNumber.Ace, CardNumber.Ten) => true,
            (CardNumber.Ace, CardNumber.Jack) => true,
            (CardNumber.Ace, CardNumber.Queen) => true,
            (CardNumber.Ace, CardNumber.King) => true,
            _ => false
        };

        protected void PlayForParticipant(Participant participant, Participant dealer, CardDeck cards) {
            Debug.Assert(participant != null);
            Debug.Assert(participant.Hands != null);
            Debug.Assert(participant.Hands.Count == 1);
            // we need to play the hand for the opponent now
            // if a split occurs, we need to create a new hand and play each hand seperately
            _logger.LogLine($"playing for '{participant.Role}'");
            PlayHand(participant.Hands[0], (dealer.Hands[0] as DealerHand)!, participant, cards);
        }

        // returns a list because a hand can be split
        protected List<Hand> PlayHand(Hand hand, DealerHand dealerHand, Participant participant, CardDeck cards) {
            Debug.Assert(hand != null);
            Debug.Assert(participant != null);
            Debug.Assert(cards != null);

            var nextAction = participant.Player.GetNextAction(hand, dealerHand);

            // if the nextAction is to split we need to create two hands and deal a new card to each hand
            if (nextAction == HandAction.Split) {
                _logger.LogLine($"action = split. Hand={hand}");

                var newHand = new Hand(hand.Bet, _logger);
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
        protected void PlayNextAction(Hand hand, DealerHand dealerHand, Participant participant, CardDeck cards) {
            // next action at this point shouldn't be split.
            // splits should have already been taken care of at this point
            var nextAction = participant.Player.GetNextAction(hand, dealerHand);
            _logger.LogLine($"  action: {nextAction}, Hand={hand}");
            switch (nextAction) {
                case HandAction.Split: throw new ApplicationException("no splits here");
                case HandAction.Stand:
                    hand.MarkHandAsClosed();
                    break;
                case HandAction.Hit:
                    hand.ReceiveCard(cards.GetCardAndMoveNext()!);
                    // note: recursion below
                    PlayNextAction(hand, dealerHand, participant, cards);
                    break;
                case HandAction.Double:
                    hand.ReceiveCard(cards.GetCardAndMoveNext()!);
                    hand.MarkHandAsClosed();
                    break;
                default:
                    throw new ApplicationException($"unknown value for nextAction:'{nextAction}'");
            }
        }
    }

}
