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
using SayedHa.Blackjack.Shared.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class PlayerTests {
        [Fact]
        public void RandomPlayer_TestStandOn21OrMore() {
            
            var bet = 1;
            var logger = new Logger();


            var dealerHand = new DealerHand(logger);
            dealerHand.ReceiveCard(new Card { Suit = CardSuit.Club, Number = CardNumber.Ace });
            dealerHand.ReceiveCard(new Card { Suit = CardSuit.Spade, Number = CardNumber.Nine });
            var playerHand = new Hand(bet, logger);
            playerHand.ReceiveCard(new Card { Suit=CardSuit.Club,Number = CardNumber.King });
            playerHand.ReceiveCard(new Card { Suit=CardSuit.Spade, Number = CardNumber.Ace});

            var randomPlayer = new RandomPlayer();

            var result = randomPlayer.GetNextAction(playerHand, dealerHand, 1000);
            Assert.Equal(HandAction.Stand, result.HandAction);
            
        }
    }
}
