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
using SayedHa.Blackjack.Shared.Players;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace SayedHa.Blackjack.Shared {
    public class Game {
        public Game(
            CardDeck cards, 
            Participant dealer, 
            List<Participant> opponents, 
            int shuffleThresholdPercent) {

            Cards = cards;
            Dealer = dealer;
            Opponents = opponents;
            ShuffleThresholdPercent = shuffleThresholdPercent;
        }

        public GameStatus Status { get; set; } = GameStatus.InPlay;

        public CardDeck Cards { get; internal init; }
        public Participant Dealer { get; protected internal init; }
        public List<Participant> Opponents { get; protected internal init; }
        public int ShuffleThresholdPercent { get; internal init; }
    }
    public enum GameStatus {
        InPlay,
        DealerPlaying,
        Finished
    }

    public class GameFactory {

        public Game CreateNewGame(int numDecks,
                int numOpponents,
                ParticipantFactory participantFactory,
                int shuffleThresholdPercent,
                ILogger? logger = null) {
            return CreateNewGame(numDecks, numOpponents, participantFactory, participantFactory.OpponentPlayStrategy,BlackjackSettings.GetBlackjackSettings().ShuffleThresholdPercent);
        }
        public Game CreateNewGame(
                int numDecks,
                int numOpponents,
                ParticipantFactory participantFactory,
                OpponentPlayStrategy opponentPlayStrategy,
                int shuffleThresholdPercent,
                ILogger? logger = null) {

            logger = logger ?? NullLogger.Instance;
            Debug.Assert(numDecks > 0);
            Debug.Assert(numOpponents > 0);

            var cards = new CardDeckFactory().CreateCardDeck(numDecks, true);
            var dealerPlayer = participantFactory.GetDefaultDealer();
            var opponents = new List<Participant>();

            var bankRoll = participantFactory.BettingStrategy.Bankroll;

            for (var i = 0; i < numOpponents; i++) {
                opponents.Add(participantFactory.CreateNewOpponent(opponentPlayStrategy, logger));
            }

            return new Game(cards, dealerPlayer, opponents, BlackjackSettings.GetBlackjackSettings().ShuffleThresholdPercent);
        }
        public Game CreateNewGame(
                CardDeck cards,
                int numOpponents,
                ParticipantFactory participantFactory,
                OpponentPlayStrategy opponentPlayStrategy,
                int shuffleThresholdPercent,
                ILogger? logger = null) {

            logger = logger ?? NullLogger.Instance;
            Debug.Assert(cards != null);
            Debug.Assert(numOpponents > 0);

            var dealerPlayer = participantFactory.GetDefaultDealer();
            var opponents = new List<Participant>();

            for (var i = 0; i < numOpponents; i++) {
                opponents.Add(participantFactory.CreateNewOpponent(opponentPlayStrategy, logger));
            }

            // discard the first card - TODO: make into setting to enable/disable
            cards.DiscardACard();
            return new Game(cards, dealerPlayer, opponents, BlackjackSettings.GetBlackjackSettings().ShuffleThresholdPercent);
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
        Random,
        UserInput,
        Custom
    }
}
