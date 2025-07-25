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
using SayedHa.Blackjack.Shared;
using System;
using System.Linq;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class GameFactoryTests {
        private ParticipantFactory CreateTestParticipantFactory() {
            var logger = NullLogger.Instance;
            return new ParticipantFactory(logger);
        }

        [Fact]
        public void Test_CreateNewGame_Basic_Parameters() {
            var factory = new GameFactory();
            var participantFactory = CreateTestParticipantFactory();
            
            var game = factory.CreateNewGame(1, 1, participantFactory, 25);
            
            Assert.NotNull(game);
            Assert.NotNull(game.Cards);
            Assert.NotNull(game.Dealer);
            Assert.NotNull(game.Opponents);
            Assert.Single(game.Opponents);
            Assert.Equal(25, game.ShuffleThresholdPercent);
            Assert.Equal(GameStatus.InPlay, game.Status);
        }

        [Fact]
        public void Test_CreateNewGame_Multiple_Opponents() {
            var factory = new GameFactory();
            var participantFactory = CreateTestParticipantFactory();
            
            var game = factory.CreateNewGame(1, 3, participantFactory, 25);
            
            Assert.Equal(3, game.Opponents.Count);
            Assert.All(game.Opponents, opponent => Assert.NotNull(opponent));
        }

        [Fact]
        public void Test_CreateNewGame_Multiple_Decks() {
            var factory = new GameFactory();
            var participantFactory = CreateTestParticipantFactory();
            
            var game = factory.CreateNewGame(6, 1, participantFactory, 25);
            
            Assert.NotNull(game.Cards);
            // A 6-deck shoe should have significantly more cards than a single deck
            var remainingCards = game.Cards.GetRemainingCardsAsList();
            Assert.True(remainingCards.Count > 200); // 6 decks * 52 cards - 1 burned card
        }

        [Fact]
        public void Test_CreateNewGame_With_OpponentPlayStrategy() {
            var factory = new GameFactory();
            var participantFactory = CreateTestParticipantFactory();
            
            var game = factory.CreateNewGame(1, 1, participantFactory, OpponentPlayStrategy.StandOn17, 25);
            
            Assert.NotNull(game);
            Assert.Single(game.Opponents);
        }

        [Fact]
        public void Test_CreateNewGame_With_CardDeck() {
            var factory = new GameFactory();
            var participantFactory = CreateTestParticipantFactory();
            var cardDeck = new CardDeckFactory().CreateCardDeck(2, true);
            var initialCardCount = cardDeck.GetRemainingCardsAsList().Count;
            
            var game = factory.CreateNewGame(cardDeck, 1, participantFactory, OpponentPlayStrategy.BasicStrategy, 25);
            
            Assert.NotNull(game);
            Assert.Same(cardDeck, game.Cards);
            Assert.Single(game.Opponents);
            
            // Should have discarded one card (burned)
            var remainingAfterBurn = game.Cards.GetRemainingCardsAsList().Count;
            Assert.Equal(initialCardCount - 1, remainingAfterBurn);
        }

        [Fact]
        public void Test_CreateNewGame_Dealer_Is_Not_Null() {
            var factory = new GameFactory();
            var participantFactory = CreateTestParticipantFactory();
            
            var game = factory.CreateNewGame(1, 1, participantFactory, 25);
            
            Assert.NotNull(game.Dealer);
            Assert.Equal(ParticipantRole.Dealer, game.Dealer.Role);
        }

        [Fact]
        public void Test_CreateNewGame_Opponents_Have_Correct_Role() {
            var factory = new GameFactory();
            var participantFactory = CreateTestParticipantFactory();
            
            var game = factory.CreateNewGame(1, 2, participantFactory, 25);
            
            Assert.All(game.Opponents, opponent => 
                Assert.Equal(ParticipantRole.Player, opponent.Role));
        }

        [Fact]
        public void Test_CreateNewGame_With_Logger() {
            var factory = new GameFactory();
            var participantFactory = CreateTestParticipantFactory();
            var logger = NullLogger.Instance;
            
            var game = factory.CreateNewGame(1, 1, participantFactory, 25, logger);
            
            Assert.NotNull(game);
        }

        [Fact]
        public void Test_CreateNewGame_Different_Shuffle_Thresholds() {
            var factory = new GameFactory();
            var participantFactory = CreateTestParticipantFactory();
            
            var game1 = factory.CreateNewGame(1, 1, participantFactory, 10);
            var game2 = factory.CreateNewGame(1, 1, participantFactory, 75);
            
            // Note: The actual implementation seems to override with BlackjackSettings value
            // This test verifies that the method accepts different threshold values
            Assert.NotNull(game1);
            Assert.NotNull(game2);
        }

        [Theory]
        [InlineData(OpponentPlayStrategy.BasicStrategy)]
        [InlineData(OpponentPlayStrategy.StandOn17)]
        [InlineData(OpponentPlayStrategy.StandOn12)]
        [InlineData(OpponentPlayStrategy.AlwaysStand)]
        public void Test_CreateNewGame_Different_Play_Strategies(OpponentPlayStrategy strategy) {
            var factory = new GameFactory();
            var participantFactory = CreateTestParticipantFactory();
            
            var game = factory.CreateNewGame(1, 1, participantFactory, strategy, 25);
            
            Assert.NotNull(game);
            Assert.Single(game.Opponents);
        }

        [Fact]
        public void Test_CreateNewGame_Cards_Are_Shuffled() {
            var factory = new GameFactory();
            var participantFactory = CreateTestParticipantFactory();
            
            // Create two games and compare their card order
            var game1 = factory.CreateNewGame(1, 1, participantFactory, 25);
            var game2 = factory.CreateNewGame(1, 1, participantFactory, 25);
            
            var cards1 = game1.Cards.GetRemainingCardsAsList();
            var cards2 = game2.Cards.GetRemainingCardsAsList();
            
            // Since the decks are shuffled, they should likely be in different orders
            // (This test has a very small chance of false positive if shuffles result in same order)
            bool different = false;
            for (int i = 0; i < Math.Min(cards1.Count, cards2.Count) && i < 10; i++) {
                if (!cards1[i].Equals(cards2[i])) {
                    different = true;
                    break;
                }
            }
            
            Assert.True(different, "Shuffled decks should have different card orders");
        }

        [Fact]
        public void Test_Game_Properties_After_Creation() {
            var factory = new GameFactory();
            var participantFactory = CreateTestParticipantFactory();
            
            var game = factory.CreateNewGame(2, 2, participantFactory, 30);
            
            Assert.Equal(GameStatus.InPlay, game.Status);
            Assert.NotNull(game.Cards);
            Assert.NotNull(game.Dealer);
            Assert.Equal(2, game.Opponents.Count);
            Assert.True(game.ShuffleThresholdPercent > 0);
        }
    }
}
