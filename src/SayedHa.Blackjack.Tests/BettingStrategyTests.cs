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
using SayedHa.Blackjack.Shared.Betting;
using SayedHa.Blackjack.Shared.Players;
using System.Collections.Generic;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class BettingStrategyTests {
        private Bankroll CreateTestBankroll() {
            return new Bankroll(1000, NullLogger.Instance);
        }

        private Game CreateTestGame() {
            var bankroll = CreateTestBankroll();
            var bettingStrategy = new FixedBettingStrategy(bankroll, 10);
            var player = new TestBettingPlayer();
            var dealer = new Dealer(player, bettingStrategy);
            var opponent = new Opponent(player, bettingStrategy);
            var opponents = new List<Participant> { opponent };
            var cardDeck = new CardDeckFactory().CreateCardDeck(1, true);
            
            return new Game(cardDeck, dealer, opponents, 25);
        }

        [Fact]
        public void Test_FixedBettingStrategy_Constructor_With_Amount() {
            var bankroll = CreateTestBankroll();
            var strategy = new FixedBettingStrategy(bankroll, 25);
            
            Assert.Same(bankroll, strategy.Bankroll);
            Assert.Equal(25, strategy.BetAmount);
        }

        [Fact]
        public void Test_FixedBettingStrategy_Constructor_Without_Amount() {
            var bankroll = CreateTestBankroll();
            
            // This test requires BlackjackSettings to be properly initialized
            var strategy = new FixedBettingStrategy(bankroll);
            
            Assert.Same(bankroll, strategy.Bankroll);
            Assert.True(strategy.BetAmount > 0); // Should use default from BlackjackSettings
        }

        [Fact]
        public void Test_FixedBettingStrategy_GetNextBetAmount_Returns_Fixed_Amount() {
            var bankroll = CreateTestBankroll();
            var strategy = new FixedBettingStrategy(bankroll, 50);
            var game = CreateTestGame();
            
            var betAmount1 = strategy.GetNextBetAmount(game);
            var betAmount2 = strategy.GetNextBetAmount(game);
            var betAmount3 = strategy.GetNextBetAmount(game);
            
            Assert.Equal(50, betAmount1);
            Assert.Equal(50, betAmount2);
            Assert.Equal(50, betAmount3);
        }

        [Fact]
        public void Test_FixedBettingStrategy_GetNextBetAmount_Consistent_Regardless_Of_Game_State() {
            var bankroll = CreateTestBankroll();
            var strategy = new FixedBettingStrategy(bankroll, 100);
            var game1 = CreateTestGame();
            var game2 = CreateTestGame();
            
            var betAmount1 = strategy.GetNextBetAmount(game1);
            var betAmount2 = strategy.GetNextBetAmount(game2);
            
            Assert.Equal(100, betAmount1);
            Assert.Equal(100, betAmount2);
            Assert.Equal(betAmount1, betAmount2);
        }

        [Fact]
        public void Test_BettingStrategy_CreateNewDefaultBettingStrategy_With_Logger() {
            var logger = NullLogger.Instance;
            
            var strategy = BettingStrategy.CreateNewDefaultBettingStrategy(logger);
            
            Assert.NotNull(strategy);
            // The default strategy depends on BlackjackSettings configuration
            Assert.NotNull(strategy.Bankroll);
        }

        [Fact]
        public void Test_BettingStrategy_CreateNewDefaultBettingStrategy_With_Bankroll() {
            var bankroll = CreateTestBankroll();
            
            var strategy = BettingStrategy.CreateNewDefaultBettingStrategy(bankroll);
            
            Assert.NotNull(strategy);
            // The default strategy depends on BlackjackSettings configuration
            Assert.Same(bankroll, strategy.Bankroll);
        }

        [Fact]
        public void Test_FixedBettingStrategy_Zero_Bet_Amount() {
            var bankroll = CreateTestBankroll();
            var strategy = new FixedBettingStrategy(bankroll, 0);
            var game = CreateTestGame();
            
            var betAmount = strategy.GetNextBetAmount(game);
            
            Assert.Equal(0, betAmount);
        }

        [Fact]
        public void Test_FixedBettingStrategy_Large_Bet_Amount() {
            var bankroll = CreateTestBankroll();
            var strategy = new FixedBettingStrategy(bankroll, 10000);
            var game = CreateTestGame();
            
            var betAmount = strategy.GetNextBetAmount(game);
            
            Assert.Equal(10000, betAmount);
        }

        [Fact]
        public void Test_FixedBettingStrategy_Negative_Bet_Amount() {
            var bankroll = CreateTestBankroll();
            var strategy = new FixedBettingStrategy(bankroll, -50);
            var game = CreateTestGame();
            
            var betAmount = strategy.GetNextBetAmount(game);
            
            Assert.Equal(-50, betAmount);
        }
    }

    // Test helper class for betting strategy tests
    internal class TestBettingPlayer : Player {
        public override HandActionAndReason GetNextAction(Hand hand, DealerHand dealerHand, int dollarsRemaining) {
            return new HandActionAndReason(HandAction.Stand);
        }
    }
}
