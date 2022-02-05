﻿using SayedHa.Blackjack.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class TestGameRunner {
        
        [Fact]
        public void Test_PlayGame_01() {
            var logger = new Logger();
            var playStrategy = OpponentPlayStrategy.BasicStrategy;
            var pf = new ParticipantFactory();

            var game = new Game(
                new CardDeckFactory().CreateCardDeck(GetSampleDeck01(), logger),
                pf.GetDefaultDealer(),
                new List<Participant> { pf.CreateNewOpponent(playStrategy, logger) },
                20);

            var gameRunner = new GameRunner(logger);
            var result = gameRunner.PlayGame(game);

            Assert.NotNull(result);
            Assert.Equal(17, game.Opponents[0].Hands[0].GetScore());
            Assert.Equal(17, game.Dealer.Hands[0].GetScore());
            Assert.Equal(HandResult.Push, game.Opponents[0].Hands[0].HandResult);
        }

        private List<Card> GetSampleDeck01() {
            var cards = new List<Card>();

            cards.Add(new Card { Number = CardNumber.Three, Suit = CardSuit.Spade });
            cards.Add(new Card { Number = CardNumber.Six, Suit = CardSuit.Club });
            cards.Add(new Card { Number = CardNumber.Five, Suit = CardSuit.Spade });
            cards.Add(new Card { Number = CardNumber.Three, Suit = CardSuit.Club });
            cards.Add(new Card { Number = CardNumber.Ten, Suit = CardSuit.Spade });
            cards.Add(new Card { Number = CardNumber.Six, Suit = CardSuit.Diamond });
            cards.Add(new Card { Number = CardNumber.Four, Suit = CardSuit.Heart });
            cards.Add(new Card { Number = CardNumber.Ace, Suit = CardSuit.Heart });
            cards.Add(new Card { Number = CardNumber.Ace, Suit = CardSuit.Diamond });
            cards.Add(new Card { Number = CardNumber.Four, Suit = CardSuit.Diamond });
            cards.Add(new Card { Number = CardNumber.Three, Suit = CardSuit.Diamond });
            cards.Add(new Card { Number = CardNumber.Six, Suit = CardSuit.Spade });
            cards.Add(new Card { Number = CardNumber.Queen, Suit = CardSuit.Spade });
            cards.Add(new Card { Number = CardNumber.Ten, Suit = CardSuit.Heart });
            cards.Add(new Card { Number = CardNumber.Three, Suit = CardSuit.Heart });
            cards.Add(new Card { Number = CardNumber.Four, Suit = CardSuit.Spade });
            cards.Add(new Card { Number = CardNumber.Two, Suit = CardSuit.Spade });
            cards.Add(new Card { Number = CardNumber.Jack, Suit = CardSuit.Spade });
            cards.Add(new Card { Number = CardNumber.Seven, Suit = CardSuit.Spade });
            cards.Add(new Card { Number = CardNumber.Seven, Suit = CardSuit.Club });
            cards.Add(new Card { Number = CardNumber.Queen, Suit = CardSuit.Diamond });
            cards.Add(new Card { Number = CardNumber.Queen, Suit = CardSuit.Club });
            cards.Add(new Card { Number = CardNumber.Ten, Suit = CardSuit.Diamond });
            cards.Add(new Card { Number = CardNumber.King, Suit = CardSuit.Spade });
            cards.Add(new Card { Number = CardNumber.Ace, Suit = CardSuit.Spade });
            cards.Add(new Card { Number = CardNumber.Six, Suit = CardSuit.Heart });
            cards.Add(new Card { Number = CardNumber.Two, Suit = CardSuit.Heart });
            cards.Add(new Card { Number = CardNumber.Two, Suit = CardSuit.Club });
            cards.Add(new Card { Number = CardNumber.Four, Suit = CardSuit.Club });
            cards.Add(new Card { Number = CardNumber.Eight, Suit = CardSuit.Diamond });
            cards.Add(new Card { Number = CardNumber.King, Suit = CardSuit.Diamond });
            cards.Add(new Card { Number = CardNumber.Jack, Suit = CardSuit.Diamond });
            cards.Add(new Card { Number = CardNumber.Eight, Suit = CardSuit.Club });
            cards.Add(new Card { Number = CardNumber.Nine, Suit = CardSuit.Heart });
            cards.Add(new Card { Number = CardNumber.Nine, Suit = CardSuit.Club });
            cards.Add(new Card { Number = CardNumber.Nine, Suit = CardSuit.Spade });
            cards.Add(new Card { Number = CardNumber.Five, Suit = CardSuit.Heart });
            cards.Add(new Card { Number = CardNumber.Eight, Suit = CardSuit.Spade });
            cards.Add(new Card { Number = CardNumber.Ace, Suit = CardSuit.Club });
            cards.Add(new Card { Number = CardNumber.Jack, Suit = CardSuit.Club });
            cards.Add(new Card { Number = CardNumber.Nine, Suit = CardSuit.Diamond });
            cards.Add(new Card { Number = CardNumber.King, Suit = CardSuit.Club });
            cards.Add(new Card { Number = CardNumber.Eight, Suit = CardSuit.Heart });
            cards.Add(new Card { Number = CardNumber.Two, Suit = CardSuit.Diamond });
            cards.Add(new Card { Number = CardNumber.King, Suit = CardSuit.Heart });
            cards.Add(new Card { Number = CardNumber.Ten, Suit = CardSuit.Club });
            cards.Add(new Card { Number = CardNumber.Queen, Suit = CardSuit.Heart });
            cards.Add(new Card { Number = CardNumber.Jack, Suit = CardSuit.Heart });
            cards.Add(new Card { Number = CardNumber.Five, Suit = CardSuit.Club });
            cards.Add(new Card { Number = CardNumber.Seven, Suit = CardSuit.Heart });
            cards.Add(new Card { Number = CardNumber.Seven, Suit = CardSuit.Diamond });
            cards.Add(new Card { Number = CardNumber.Five, Suit = CardSuit.Diamond });

            return cards;
        }
    }
}
