using SayedHa.Blackjack.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class CardNumberExtensionTests {
        [Fact]
        public void Test_Sort_NoFaceCards() {
            (var c1, var c2) = CardNumber.Five.Sort(CardNumber.Six);

            Assert.Equal(CardNumber.Six, c1);
            Assert.Equal(CardNumber.Five, c2);
        }

        [Fact]
        public void Test_Sort_TwoFaceCards01() {
            (var c1, var c2) = CardNumber.Jack.Sort(CardNumber.King);

            Assert.Equal(CardNumber.King, c1);
            Assert.Equal(CardNumber.Jack, c2);
        }
        [Fact]
        public void Test_Sort_TwoFaceCards02() {
            (var c1, var c2) = CardNumber.King.Sort(CardNumber.Queen);

            Assert.Equal(CardNumber.King, c1);
            Assert.Equal(CardNumber.Queen, c2);
        }
        [Fact]
        public void Test_Sort_TwoFaceCards03() {
            (var c1, var c2) = CardNumber.King.Sort(CardNumber.Ace);

            Assert.Equal(CardNumber.King, c1);
            Assert.Equal(CardNumber.Ace, c2);
        }
        [Fact]
        public void Test_Sort_OneFaceCard01() {
            (var c1, var c2) = CardNumber.King.Sort(CardNumber.Two);

            Assert.Equal(CardNumber.King, c1);
            Assert.Equal(CardNumber.Two, c2);
        }
        [Fact]
        public void Test_Sort_OneFaceCard02() {
            (var c1, var c2) = CardNumber.Four.Sort(CardNumber.Ace);

            Assert.Equal(CardNumber.Four, c1);
            Assert.Equal(CardNumber.Ace, c2);
        }
    }
}
