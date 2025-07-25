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
using System.Linq;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class BankrollTests {
        [Fact]
        public void Test_Constructor_Sets_Initial_Values() {
            var logger = NullLogger.Instance;
            var bankroll = new Bankroll(1000, logger);
            
            Assert.Equal(1000f, bankroll.InitialBankroll);
            Assert.Equal(1000f, bankroll.DollarsRemaining);
            Assert.Empty(bankroll.Transactions);
        }

        [Fact]
        public void Test_AddToDollarsRemaining_Positive_Amount() {
            var logger = NullLogger.Instance;
            var bankroll = new Bankroll(1000, logger);
            
            var result = bankroll.AddToDollarsRemaining(250f, "TestPlayer");
            
            Assert.Equal(1250f, result);
            Assert.Equal(1250f, bankroll.DollarsRemaining);
            Assert.Single(bankroll.Transactions);
            Assert.Equal(250f, bankroll.Transactions[0]);
        }

        [Fact]
        public void Test_AddToDollarsRemaining_Negative_Amount() {
            var logger = NullLogger.Instance;
            var bankroll = new Bankroll(1000, logger);
            
            var result = bankroll.AddToDollarsRemaining(-100f, "TestPlayer");
            
            Assert.Equal(900f, result);
            Assert.Equal(900f, bankroll.DollarsRemaining);
            Assert.Single(bankroll.Transactions);
            Assert.Equal(-100f, bankroll.Transactions[0]);
        }

        [Fact]
        public void Test_Multiple_Transactions() {
            var logger = NullLogger.Instance;
            var bankroll = new Bankroll(1000, logger);
            
            bankroll.AddToDollarsRemaining(50f, "Player1");
            bankroll.AddToDollarsRemaining(-25f, "Player1");
            bankroll.AddToDollarsRemaining(100f, "Player1");
            
            Assert.Equal(1125f, bankroll.DollarsRemaining);
            Assert.Equal(3, bankroll.Transactions.Count);
            Assert.Equal(50f, bankroll.Transactions[0]);
            Assert.Equal(-25f, bankroll.Transactions[1]);
            Assert.Equal(100f, bankroll.Transactions[2]);
        }

        [Fact]
        public void Test_Zero_Amount_Transaction() {
            var logger = NullLogger.Instance;
            var bankroll = new Bankroll(1000, logger);
            
            var result = bankroll.AddToDollarsRemaining(0f, "TestPlayer");
            
            Assert.Equal(1000f, result);
            Assert.Equal(1000f, bankroll.DollarsRemaining);
            Assert.Single(bankroll.Transactions);
            Assert.Equal(0f, bankroll.Transactions[0]);
        }

        [Fact]
        public void Test_CreateNewDefaultBankroll() {
            var logger = NullLogger.Instance;
            
            // Note: This test depends on BlackjackSettings being properly initialized
            // In a real test environment, you might want to mock this or set up the settings
            var bankroll = Bankroll.CreateNewDefaultBankroll(logger);
            
            Assert.NotNull(bankroll);
            // The initial amount comes from BlackjackSettings, may be 0 in test environment
            Assert.True(bankroll.InitialBankroll >= 0);
            Assert.Equal(bankroll.InitialBankroll, bankroll.DollarsRemaining);
        }

        [Fact]
        public void Test_Bankroll_Can_Go_Negative() {
            var logger = NullLogger.Instance;
            var bankroll = new Bankroll(100, logger);
            
            bankroll.AddToDollarsRemaining(-150f, "TestPlayer");
            
            Assert.Equal(-50f, bankroll.DollarsRemaining);
            Assert.Equal(100f, bankroll.InitialBankroll); // Initial should not change
        }

        [Fact]
        public void Test_Large_Transaction_Amounts() {
            var logger = NullLogger.Instance;
            var bankroll = new Bankroll(1000, logger);
            
            bankroll.AddToDollarsRemaining(999999.99f, "HighRoller");
            
            Assert.Equal(1000999.99f, bankroll.DollarsRemaining);
            Assert.Single(bankroll.Transactions);
            Assert.Equal(999999.99f, bankroll.Transactions[0]);
        }

        [Fact]
        public void Test_Transaction_History_Preserved() {
            var logger = NullLogger.Instance;
            var bankroll = new Bankroll(500, logger);
            
            var transactions = new[] { 100f, -50f, 25f, -10f, 200f };
            foreach (var transaction in transactions) {
                bankroll.AddToDollarsRemaining(transaction, "Player");
            }
            
            Assert.Equal(transactions.Length, bankroll.Transactions.Count);
            for (int i = 0; i < transactions.Length; i++) {
                Assert.Equal(transactions[i], bankroll.Transactions[i]);
            }
            
            var expectedTotal = 500f + transactions.Sum();
            Assert.Equal(expectedTotal, bankroll.DollarsRemaining);
        }
    }
}
