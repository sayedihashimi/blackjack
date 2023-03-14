using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Extensions;
using SayedHa.Blackjack.Shared.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class BasicStrategyPlayerTests {
        [Fact]
        public void Test_NoDouble_AfterMoreThanTwo_Cards_Dealt01() {
            var bsplayer = new BasicStrategyPlayer(NullLogger.Instance);

            int numDecks = 4;
            var cardList = CreateDeckForDoubleHand(numDecks);
            var cardDeck = new CardDeck(NullLogger.Instance, cardList.ConvertToLinkedList(), numDecks);

            var hand = new Hand(5, NullLogger.Instance);
            hand.ReceiveCard(new Card { Number = CardNumber.Five, Suit = CardSuit.Diamond });
            hand.ReceiveCard(new Card { Number = CardNumber.Three, Suit = CardSuit.Club });
            hand.ReceiveCard(new Card { Number = CardNumber.Three, Suit = CardSuit.Diamond });

            var dealerHand = new DealerHand(NullLogger.Instance);
            dealerHand.ReceiveCard(new Card { Number = CardNumber.Six, Suit=CardSuit.Club });
            dealerHand.ReceiveCard(new Card { Number = CardNumber.Six, Suit = CardSuit.Club });

            var nextAction = bsplayer.GetNextAction(hand, dealerHand, 10000);
            Assert.NotEqual(HandAction.Double, nextAction.HandAction);
        }

        public static List<Card> CreateDeckForDoubleHand(int numDecks) {
            var standardDeck = new List<Card>();

            for (var i = 0; i < numDecks; i++) {
                // preserve order to keep tests passing.
                standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Ace });
                standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Six });
                standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Five });

                standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Two });
                standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Three });
                standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Four });

                standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Seven });
                standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Eight });
                standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Nine });
                standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Ten });
                standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Jack });
                standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.Queen });
                standardDeck.Add(new Card { Suit = CardSuit.Heart, Number = CardNumber.King });
            }

            return standardDeck;
        }
    }
}
