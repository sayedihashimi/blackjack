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
using SayedHa.Blackjack.Shared.Players;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;

namespace SayedHa.Blackjack.Shared {
    public class GameRunner {
        public GameRunner(ILogger logger) {
            _logger = logger;
            BasicStrategyPlayer = new BasicStrategyPlayer(_logger);
        }

        private ILogger _logger = NullLogger.Instance;
        private GameFactory gameFactory = new GameFactory();

        public event EventHandler NextActionSelected;
        public event EventHandler DealerHasBlackjack;
        public event EventHandler PlayerHasBlackjack;
        public event EventHandler BetAmountConfigured;
        public event EventHandler CardReceived;
        public event EventHandler ShufflingCards;
        public event EventHandler WrongNextActionSelected;

        private Game CurrentGame { get; set; }
        private Player BasicStrategyPlayer { get; init; }
        public Game CreateNewGame(int numDecks, int numOpponents, ParticipantFactory participantFactory, bool discardFirstCard) {
            var game = gameFactory.CreateNewGame(numDecks, numOpponents, participantFactory, BlackjackSettings.GetBlackjackSettings().ShuffleThresholdPercent, _logger);

            if (discardFirstCard) {
                // very first action for a new deck is to discard one card.
                game.Cards.DiscardACard();
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
                // _logger.LogLine("**** shuffling cards");
                ShufflingCards?.Invoke(this, new ShufflingCardsEventArg(game));
                game.Cards.ShuffleCards();
                // _logger.LogLine($"cards: [{game.Cards}]");
                // discard the first card after every shuffle
                game.Cards.DiscardACard();
            }
        }

        public GameResult PlayGame(Game game) {
            CurrentGame = game;
            Debug.Assert(game != null);
            Debug.Assert(game.Cards != null);

            game.Status = GameStatus.InPlay;
            // need to reset the hands
            game.Dealer.Hands.Clear();
            foreach(var op in game.Opponents) {
                op.Hands.Clear();
            }
            // check to see if the deck needs to be shuffled or not
            ShuffleCardsIfNeeded(game);

            if (game == null || game.Cards == null) {
                throw new ApplicationException();
            }

            Card? tempCard = null;
            // deal two cards to each opponent
            // TODO: move index to participant, maybe make a name property or something
            int index = 1;
            foreach (var opponent in game.Opponents) {
                int betAmount = opponent.BettingStrategy.GetNextBetAmount(game);
                BetAmountConfigured?.Invoke(this, new BetAmountConfiguredEventArgs(game,betAmount));
                var newhand = new Hand(betAmount, _logger);

                newhand.ReceiveCards(game.Cards.GetCardAndMoveNext()!, game.Cards.GetCardAndMoveNext()!);
                CardReceived?.Invoke(this, new CardReceivedEventArgs(game));
                CardReceived?.Invoke(this, new CardReceivedEventArgs(game));

                // _logger.LogLine($"Dealing to {opponent.Name}: {newhand}, bet = ${betAmount:F0}");
                opponent.Hands.Add(newhand);

                index++;
            }

            // deal two cards to the dealer
            var dealerHand = new DealerHand(_logger);
            game.Dealer.Hands.Add(dealerHand);

            dealerHand.ReceiveCards(game.Cards.GetCardAndMoveNext()!, game.Cards.GetCardAndMoveNext()!);
            CardReceived?.Invoke(this, new CardReceivedEventArgs(game));
            CardReceived?.Invoke(this, new CardReceivedEventArgs(game));

            // _logger.LogLine($"Dealing to Dealer: {dealerHand} (2nd card visible)");
            

            // TODO: Change how the flow works should be more like:
            //  1) Does dealer have blackjack? => game over
            //  2) Does player have blackjack? => 3:2 payout to that hand
            //  3) Regular play

            var bjPayoutMultiplier = BlackjackSettings.GetBlackjackSettings().BlackjackPayoutMultplier;
            // check to see if the dealer has blackjack and if so end the game.
            if (!dealerHand.DoesDealerHaveBlackjack()) {
                // play each opponent now
                foreach (var opponent in game.Opponents) {
                    PlayForParticipant(opponent, game.Dealer, game.Cards);
                }

                // now play for the dealer
                game.Status = GameStatus.DealerPlaying;

                bool allHandsBusted = true;
                bool allHandsBlackjack = true;

                // check all hands for being busted as well as blackjack
                foreach(var op in game.Opponents) {
                    foreach(var hand in op.Hands) {
                        if (hand.GetScore() > BlackjackSettings.GetBlackjackSettings().MaxScore) {
                            hand.SetHandResult(HandResult.DealerWon, hand.Bet * -1F);
                        }
                        else {
                            allHandsBusted = false;
                        }

                        if (hand.DoesHandHaveBlackjack()) {
                            hand.SetHandResult(HandResult.OpponentWon, hand.Bet * bjPayoutMultiplier);
                        }
                        else {
                            allHandsBlackjack = false;
                        }
                    }
                }

                if (allHandsBusted || allHandsBlackjack) {
                    game.Status = GameStatus.Finished;
                }
                else {
                    PlayForParticipant(game.Dealer, game.Dealer, game.Cards, true);
                    // now determine the results of the game
                    var dealerScore = game.Dealer.Hands[0].GetScore();

                    foreach (var opponent in game.Opponents) {
                        var sb = new StringBuilder();
                        sb.Append("Result: ");
                        foreach (var hand in opponent.Hands) {
                            var handScore = hand.GetScore();
                            if (handScore > 21) {
                                hand.SetHandResult(HandResult.DealerWon, hand.Bet * 1F);
                                opponent.BettingStrategy.Bankroll.AddToDollarsRemaining(hand.BetResult!.Value, opponent.Name);
                                game.Dealer.BettingStrategy.Bankroll.AddToDollarsRemaining(hand.Bet * -1F, game.Dealer.Name);
                                sb.Append($"↓Busted ${hand.Bet:F0} ");
                            }
                            else if (handScore == dealerScore) {
                                hand.SetHandResult(HandResult.Push, 0);
                                // no change to any bankroll on a push
                                sb.Append("=Push ");
                            }
                            else if (handScore > dealerScore || dealerScore > 21) {
                                hand.SetHandResult(HandResult.OpponentWon, hand.Bet);
                                float betMultiplier = hand.DoesHandHaveBlackjack() ? bjPayoutMultiplier : 1;
                                var amtToAdd = hand.Bet * betMultiplier;
                                opponent.BettingStrategy.Bankroll.AddToDollarsRemaining(amtToAdd, opponent.Name);
                                game.Dealer.BettingStrategy.Bankroll.AddToDollarsRemaining(amtToAdd * -1F, game.Dealer.Name);
                                sb.Append($"↑Win ${amtToAdd:F0}");
                            }
                            else {
                                hand.SetHandResult(HandResult.DealerWon, hand.Bet * -1F);
                                opponent.BettingStrategy.Bankroll.AddToDollarsRemaining(hand.Bet * -1F, opponent.Name);
                                game.Dealer.BettingStrategy.Bankroll.AddToDollarsRemaining(hand.Bet, game.Dealer.Name);
                                sb.Append($"↓Lose ${hand.Bet:F0} ");
                            }
                        }
                        // _logger.LogLine(sb.ToString());
                    }
                }
            }
            else {
                game.Status = GameStatus.Finished;
                DealerHasBlackjack?.Invoke(this, new DealerHasBlackjackEventArgs(game));
                foreach (var op in game.Opponents) {
                    foreach (var hand in op.Hands) {
                        // check to see if the opponent hand has bj, if so it's a push
                        if (hand.DoesHandHaveBlackjack()) {
                            hand.SetHandResult(HandResult.Push, 0);
                            // _logger.LogLine($"Push both player and dealer have blackjack ${hand.Bet:F0} ");
                        }
                        else {
                            hand.SetHandResult(HandResult.DealerWon, hand.Bet * -1F);
                            op.BettingStrategy.Bankroll.AddToDollarsRemaining(hand.Bet * -1, op.Name);
                            game.Dealer.BettingStrategy.Bankroll.AddToDollarsRemaining(hand.Bet, game.Dealer.Name);
                            // _logger.LogLine($"Lose(bj) ${hand.Bet:F0} ");
                        }
                    }
                }
            }

            var allHands = new List<Hand>();
            foreach (var op in game.Opponents) {
                allHands.AddRange(op.Hands);

                foreach(var hand in op.Hands) {
                    op.AddToAllHands(hand);
                }
            }

            game.Status = GameStatus.Finished;
            return new GameResult(game.Dealer.Hands[0], allHands,game.Dealer, game.Opponents);
        }

        protected void PlayForParticipant(Participant participant, Participant dealer, CardDeck cards, bool playForDealer = false) {
            Debug.Assert(participant != null);
            Debug.Assert(participant.Hands != null);
            Debug.Assert(participant.Hands.Count == 1);
            // we need to play the hand for the opponent now
            // if a split occurs, we need to create a new hand and play each hand seperately
            // _logger.LogLine($"playing for {participant.Role}");
            if(!playForDealer) {
                // TODO: Check for blackjack and pay it out. The dealer shouldn't have blackjack here, it should have been detected already.
                PlayHand(participant.Hands[0], (dealer.Hands[0] as DealerHand)!, participant, cards);
            }
            else {
                PlayHand(participant.Hands[0], (dealer.Hands[0] as DealerHand)!, dealer, cards);
            }
        }

        protected HandActionAndReason GetValidatedNextAction(Participant participant, Hand hand, DealerHand dealerHand, bool isDealerHand) {
            int maxCancels = 1000;
            int numCancels = 0;
            HandActionAndReason? nextAction = null;
            if (participant.ValidateNextAction && !isDealerHand) {
                bool isPlayCorrect = false;
                var correctAction = BasicStrategyPlayer.GetNextAction(hand, dealerHand, (int)Math.Floor(participant.BettingStrategy.Bankroll.DollarsRemaining));
                do {
                    nextAction = participant.Player.GetNextAction(hand, dealerHand, (int)Math.Floor(participant.BettingStrategy.Bankroll.DollarsRemaining));
                    isPlayCorrect = nextAction.HandAction == correctAction.HandAction;

                    if (!isPlayCorrect) {
                        WrongNextActionSelected?.Invoke(this, new WrongNextActionSelected(CurrentGame, nextAction.HandAction, correctAction, true));
                    }

                    numCancels++;
                    if (numCancels++ > maxCancels) {
                        throw new ApplicationException($"Max iterations reached for isPlayCorrect");
                    }
                } while (!isPlayCorrect);
            }

            if (nextAction == null) {
                nextAction = participant.Player.GetNextAction(hand, dealerHand, (int)Math.Floor(participant.BettingStrategy.Bankroll.DollarsRemaining));
            }

            return nextAction;
        }

        // returns a list because a hand can be split
        protected List<Hand> PlayHand(Hand hand, DealerHand dealerHand, Participant participant, CardDeck cards) {
            Debug.Assert(hand != null);
            Debug.Assert(participant != null);
            Debug.Assert(cards != null);

            if (hand.DoesHandHaveBlackjack()) {
                var bjPayoutMultiplier = BlackjackSettings.GetBlackjackSettings().BlackjackPayoutMultplier;
                hand.SetHandResult(HandResult.OpponentWon, hand.Bet * bjPayoutMultiplier);
                participant.BettingStrategy.Bankroll.AddToDollarsRemaining(hand.BetResult!.Value, participant.Name);
                PlayerHasBlackjack?.Invoke(this, new PlayerHasBlackjackEventArgs(CurrentGame));
                // TODO: Remove the win amount from the dealer?
                return new List<Hand> { hand };
            }

            bool isDealerHand = hand as DealerHand is object;
            HandActionAndReason? nextAction = null;

            nextAction = GetValidatedNextAction(participant, hand, dealerHand, isDealerHand);

            NextActionSelected?.Invoke(this, new NextActionSelectedEventArgs(CurrentGame,hand, dealerHand, nextAction.HandAction, isDealerHand));
            // if the nextAction is to split we need to create two hands and deal a new card to each hand
            if (nextAction.HandAction == HandAction.Split) {
                // _logger.LogLine($"action = split. Hand={hand}");

                var newHand = new Hand(hand.Bet, _logger);
                newHand.ReceiveCard(hand.DealtCards[1]);
                hand.DealtCards.RemoveAt(1);
                participant.Hands.Add(newHand);

                // deal a card to each hand now
                hand.ReceiveCard(cards.GetCardAndMoveNext()!);
                CardReceived?.Invoke(this, new CardReceivedEventArgs(CurrentGame, updateUi: false));
                newHand.ReceiveCard(cards.GetCardAndMoveNext()!);
                CardReceived?.Invoke(this, new CardReceivedEventArgs(CurrentGame));

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
                PlayNextAction(nextAction.HandAction, hand, dealerHand, participant, cards);
            }

            return new List<Hand> { hand };
        }
        protected void PlayNextAction(HandAction nextAction, Hand hand, DealerHand dealerHand, Participant participant, CardDeck cards) {
            // next action at this point shouldn't be split.
            // splits should have already been taken care of at this point
            var isDealerHand = hand as DealerHand is object;
            // _logger.LogLine($"  {nextAction}, Hand={hand}");
            switch (nextAction) {
                case HandAction.Split: throw new ApplicationException("no splits here");
                case HandAction.Stand:
                    hand.MarkHandAsClosed();
                    NextActionSelected?.Invoke(this, new NextActionSelectedEventArgs(CurrentGame, hand, dealerHand, HandAction.Stand, isDealerHand));
                    break;
                case HandAction.Hit:
                    hand.ReceiveCard(cards.GetCardAndMoveNext()!);
                    CardReceived?.Invoke(this, new CardReceivedEventArgs(CurrentGame, !isDealerHand));

                    var newNextAction = GetValidatedNextAction(participant, hand, dealerHand, isDealerHand);

                    NextActionSelected?.Invoke(this, new NextActionSelectedEventArgs(CurrentGame, hand, dealerHand, newNextAction.HandAction, isDealerHand));
                    // note: recursion below
                    PlayNextAction(newNextAction.HandAction, hand, dealerHand, participant, cards);
                    break;
                case HandAction.Double:
                    var card = cards.GetCardAndMoveNext()!;
                    hand.ReceiveCard(card);
                    hand.Bet *= 2;
                    hand.MarkHandAsClosed();
                    CardReceived?.Invoke(this, new CardReceivedEventArgs(CurrentGame));
                    // _logger.LogLine($"    Hit, Hand={hand}, Bet=${hand.Bet:F0}");
                    NextActionSelected?.Invoke(this, new NextActionSelectedEventArgs(CurrentGame, hand, dealerHand, HandAction.Double, isDealerHand));
                    break;
                default:
                    throw new ApplicationException($"unknown value for nextAction:'{nextAction}'");
            }
        }
    }
    public class NextActionSelectedEventArgs : GameEventArgs {
        public NextActionSelectedEventArgs(Game game,Hand hand, Hand dealerHand, HandAction nextAction, bool isDealerHand) :base(game) {
            Hand = hand;
            DealerHand = dealerHand;
            NextAction = nextAction;
            IsDealerHand = isDealerHand;
        }

        public Hand Hand { get; private init; }
        public Hand DealerHand { get;private init; }
        public HandAction NextAction { get; private init; }
        public bool IsDealerHand { get; private init; }
    }
    public class GameEventArgs : EventArgs {
        public GameEventArgs(Game game) : this(game, true) { }
        public GameEventArgs(Game game, bool updateUi) {
            Game = game;
            UpdateUi = updateUi;
        }

        public Game Game { get; set; }
        public bool UpdateUi { get; set; }
    }
    public class DealerHasBlackjackEventArgs : GameEventArgs {
        public DealerHasBlackjackEventArgs(Game game): base(game) { }
    }
    public class PlayerHasBlackjackEventArgs : GameEventArgs {
        public PlayerHasBlackjackEventArgs(Game game) : base(game) { }
    }
    public class BetAmountConfiguredEventArgs : GameEventArgs {
        public BetAmountConfiguredEventArgs(Game game, int betAmount): base(game) {
            BetAmount = betAmount;
        }

        public int BetAmount { get; set; }
    }
    public class CardReceivedEventArgs : GameEventArgs {
        public CardReceivedEventArgs(Game game, bool updateUi = true) : base(game, updateUi) {
        }
    }
    public class ShufflingCardsEventArg: GameEventArgs {
        public ShufflingCardsEventArg(Game game, bool updateUi = true):base(game, updateUi) { }
    }
    public class WrongNextActionSelected : GameEventArgs {
        public WrongNextActionSelected(Game game, HandAction nextActionSelected, HandActionAndReason correctAction, bool updateUi = true) : base(game, updateUi) {
            CorrectAction = correctAction;
            NextActionSelected = nextActionSelected;
        }
        public HandActionAndReason CorrectAction { get; protected init; }
        public HandAction NextActionSelected { get; protected init; }


    }
}
