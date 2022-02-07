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

            var result = randomPlayer.GetNextAction(playerHand, dealerHand);
            Assert.Equal(HandAction.Stand, result);
            
        }
    }
}
