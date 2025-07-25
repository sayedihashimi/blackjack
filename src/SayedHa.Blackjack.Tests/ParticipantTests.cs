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
using System.Linq;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class ParticipantTests {
        private Player CreateTestPlayer() {
            return new TestParticipantPlayer();
        }

        private BettingStrategy CreateTestBettingStrategy() {
            var bankroll = new Bankroll(1000, NullLogger.Instance);
            return new FixedBettingStrategy(bankroll, 10);
        }

        [Fact]
        public void Test_Participant_Constructor_Sets_Properties() {
            var player = CreateTestPlayer();
            var bettingStrategy = CreateTestBettingStrategy();
            
            var participant = new Participant(ParticipantRole.Player, player, bettingStrategy);
            
            Assert.Equal(ParticipantRole.Player, participant.Role);
            Assert.Equal("Player", participant.Name);
            Assert.Same(player, participant.Player);
            Assert.Same(bettingStrategy, participant.BettingStrategy);
            Assert.False(participant.ValidateNextAction);
            Assert.Empty(participant.Hands);
            Assert.Empty(participant.GetAllHands());
            Assert.Equal(0f, participant.AllHandsBetResult);
        }

        [Fact]
        public void Test_Participant_ValidateNextAction_Can_Be_Set() {
            var participant = new Participant(ParticipantRole.Player, CreateTestPlayer(), CreateTestBettingStrategy());
            
            participant.ValidateNextAction = true;
            
            Assert.True(participant.ValidateNextAction);
        }

        [Fact]
        public void Test_AddToAllHands_Adds_Hand() {
            var participant = new Participant(ParticipantRole.Player, CreateTestPlayer(), CreateTestBettingStrategy());
            var hand = new Hand(1, NullLogger.Instance);
            hand.BetResult = 25f;
            
            participant.AddToAllHands(hand);
            
            Assert.Single(participant.GetAllHands());
            Assert.Contains(hand, participant.GetAllHands());
            Assert.Equal(25f, participant.AllHandsBetResult);
        }

        [Fact]
        public void Test_AddToAllHands_Multiple_Hands() {
            var participant = new Participant(ParticipantRole.Player, CreateTestPlayer(), CreateTestBettingStrategy());
            var hand1 = new Hand(1, NullLogger.Instance) { BetResult = 10f };
            var hand2 = new Hand(2, NullLogger.Instance) { BetResult = -5f };
            var hand3 = new Hand(3, NullLogger.Instance) { BetResult = 15f };
            
            participant.AddToAllHands(hand1);
            participant.AddToAllHands(hand2);
            participant.AddToAllHands(hand3);
            
            Assert.Equal(3, participant.GetAllHands().Count);
            Assert.Equal(20f, participant.AllHandsBetResult); // 10 + (-5) + 15
        }

        [Fact]
        public void Test_Dealer_Constructor() {
            var player = CreateTestPlayer();
            var bettingStrategy = CreateTestBettingStrategy();
            
            var dealer = new Dealer(player, bettingStrategy);
            
            Assert.Equal(ParticipantRole.Dealer, dealer.Role);
            Assert.Same(player, dealer.Player);
            Assert.Same(bettingStrategy, dealer.BettingStrategy);
        }

        [Fact]
        public void Test_Dealer_DealerHand_Property_Empty() {
            var dealer = new Dealer(CreateTestPlayer(), CreateTestBettingStrategy());
            
            Assert.Null(dealer.DealerHand);
        }

        [Fact]
        public void Test_Dealer_DealerHand_Property_With_Hand() {
            var dealer = new Dealer(CreateTestPlayer(), CreateTestBettingStrategy());
            var dealerHand = new DealerHand(NullLogger.Instance);
            dealer.Hands.Add(dealerHand);
            
            Assert.NotNull(dealer.DealerHand);
            Assert.Same(dealerHand, dealer.DealerHand);
        }

        [Fact]
        public void Test_Opponent_Constructor() {
            var player = CreateTestPlayer();
            var bettingStrategy = CreateTestBettingStrategy();
            
            var opponent = new Opponent(player, bettingStrategy);
            
            Assert.Equal(ParticipantRole.Player, opponent.Role);
            Assert.Same(player, opponent.Player);
            Assert.Same(bettingStrategy, opponent.BettingStrategy);
        }

        [Fact]
        public void Test_Participant_Hands_Can_Be_Modified() {
            var participant = new Participant(ParticipantRole.Player, CreateTestPlayer(), CreateTestBettingStrategy());
            var hand1 = new Hand(1, NullLogger.Instance);
            var hand2 = new Hand(2, NullLogger.Instance);
            
            participant.Hands.Add(hand1);
            participant.Hands.Add(hand2);
            
            Assert.Equal(2, participant.Hands.Count);
            Assert.Contains(hand1, participant.Hands);
            Assert.Contains(hand2, participant.Hands);
        }

        [Fact]
        public void Test_GetAllHands_Returns_LinkedList() {
            var participant = new Participant(ParticipantRole.Player, CreateTestPlayer(), CreateTestBettingStrategy());
            
            var allHands = participant.GetAllHands();
            
            Assert.IsType<LinkedList<Hand>>(allHands);
        }

        [Fact]
        public void Test_AllHandsBetResult_Accumulates_Correctly() {
            var participant = new Participant(ParticipantRole.Player, CreateTestPlayer(), CreateTestBettingStrategy());
            
            // Test with positive values
            var hand1 = new Hand(1, NullLogger.Instance) { BetResult = 100f };
            participant.AddToAllHands(hand1);
            Assert.Equal(100f, participant.AllHandsBetResult);
            
            // Test with negative values
            var hand2 = new Hand(2, NullLogger.Instance) { BetResult = -50f };
            participant.AddToAllHands(hand2);
            Assert.Equal(50f, participant.AllHandsBetResult);
            
            // Test with zero
            var hand3 = new Hand(3, NullLogger.Instance) { BetResult = 0f };
            participant.AddToAllHands(hand3);
            Assert.Equal(50f, participant.AllHandsBetResult);
        }
    }

    // Test helper class
    internal class TestParticipantPlayer : Player {
        public override HandActionAndReason GetNextAction(Hand hand, DealerHand dealerHand, int dollarsRemaining) {
            return new HandActionAndReason(HandAction.Stand);
        }
    }
}
