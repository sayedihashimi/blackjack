using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Betting;
using SayedHa.Blackjack.Shared.Extensions;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class BasicHiLoStrategyTests {
        [Fact]
        public void Test_BasicHiLoCount() {
            var bankroll = new Bankroll(1000, new NullLogger());
            BasicHiLoStrategy bs = new BasicHiLoStrategy(bankroll, 5, 20);

            var cardList = CardDeckHelper.CreateListOfAStandardDeckOfCards(4);
            int numDecks = 4;
            var cardDeck = new CardDeck(new NullLogger(), cardList.ConvertToLinkedList(), numDecks);
            // first 20 cards are: Ace,2,3,4,5,6,7,8,9,10,J,Q,K,A,2,3,4,5,6,7
            // first card is a discard and is not seen by the players
            // count: 5
            // true count for 4 decks: 5/( (4*52-20)/(4*52) )

            var expectedRunningCount = 5F;
            var expectedTrueCount = expectedRunningCount / ((numDecks * 52F - 20F) / (numDecks * 52F));

            expectedTrueCount = expectedRunningCount / ((float)numDecks - 20F / 52F);

            // burning the first card
            cardDeck.DiscardACard();
            // deal 19 cards, the first card should be discarded automatically
            var numCardsToDeal = 19;
            for (var i = 0; i < numCardsToDeal; i++) {
                cardDeck.GetCardAndMoveNext();
            }

            var cardCount = bs.GetCount(cardDeck);
            Assert.Equal(20, cardDeck.DiscardedCards.Count);
            Assert.Equal(expectedRunningCount, cardCount.RunningCount);
            Assert.Equal(expectedTrueCount, cardCount.TrueCount);
        }
    }
}
