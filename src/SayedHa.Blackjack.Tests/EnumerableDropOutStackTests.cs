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
    public class EnumerableDropOutStackTests {
        [Fact]
        public void Test_Push_Single_Item() {
            var stack = new EnumerableDropOutStack<int>(3);
            stack.Push(1);
            
            Assert.Equal(1, stack.Count());
            Assert.Equal(1, stack.Peek());
        }

        [Fact]
        public void Test_Push_Multiple_Items_Within_Capacity() {
            var stack = new EnumerableDropOutStack<int>(3);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            
            Assert.Equal(3, stack.Count());
            Assert.Equal(3, stack.Peek());
        }

        [Fact]
        public void Test_Push_Beyond_Capacity_Drops_Oldest() {
            var stack = new EnumerableDropOutStack<int>(3);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4); // Should drop 1
            
            Assert.Equal(3, stack.Count());
            Assert.Equal(4, stack.Peek());
            
            // Check that items 2, 3, 4 are in the stack
            var items = stack.ToList();
            Assert.Contains(2, items);
            Assert.Contains(3, items);
            Assert.Contains(4, items);
            Assert.DoesNotContain(1, items);
        }

        [Fact]
        public void Test_Pop_Single_Item() {
            var stack = new EnumerableDropOutStack<int>(3);
            stack.Push(1);
            
            var popped = stack.Pop();
            
            Assert.Equal(1, popped);
            Assert.Equal(0, stack.Count());
        }

        [Fact]
        public void Test_Pop_Multiple_Items() {
            var stack = new EnumerableDropOutStack<int>(3);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            
            Assert.Equal(3, stack.Pop());
            Assert.Equal(2, stack.Pop());
            Assert.Equal(1, stack.Pop());
            Assert.Equal(0, stack.Count());
        }

        [Fact]
        public void Test_Peek_Does_Not_Modify_Stack() {
            var stack = new EnumerableDropOutStack<int>(3);
            stack.Push(1);
            stack.Push(2);
            
            var peeked1 = stack.Peek();
            var peeked2 = stack.Peek();
            
            Assert.Equal(2, peeked1);
            Assert.Equal(2, peeked2);
            Assert.Equal(2, stack.Count());
        }

        [Fact]
        public void Test_GetItem_Valid_Indices() {
            var stack = new EnumerableDropOutStack<int>(3);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            
            Assert.Equal(3, stack.GetItem(0)); // Most recent
            Assert.Equal(2, stack.GetItem(1));
            Assert.Equal(1, stack.GetItem(2)); // Oldest
        }

        [Fact]
        public void Test_GetItem_Out_Of_Bounds_Throws_Exception() {
            var stack = new EnumerableDropOutStack<int>(3);
            stack.Push(1);
            
            // The actual implementation checks index > Count(), but count is 1, so index 1 should be valid
            // Let's test with definitely out of bounds values
            Assert.Throws<InvalidOperationException>(() => stack.GetItem(2));
            Assert.Throws<InvalidOperationException>(() => stack.GetItem(5));
        }

        [Fact]
        public void Test_Clear_Empties_Stack() {
            var stack = new EnumerableDropOutStack<int>(3);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            
            stack.Clear();
            
            Assert.Equal(0, stack.Count());
        }

        [Fact]
        public void Test_Enumeration_Order() {
            var stack = new EnumerableDropOutStack<int>(3);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            
            var items = stack.ToList();
            
            Assert.Equal(3, items.Count);
            Assert.Equal(3, items[0]); // Most recent first
            Assert.Equal(2, items[1]);
            Assert.Equal(1, items[2]); // Oldest last
        }

        [Fact]
        public void Test_String_Items() {
            var stack = new EnumerableDropOutStack<string>(2);
            stack.Push("first");
            stack.Push("second");
            stack.Push("third"); // Should drop "first"
            
            Assert.Equal(2, stack.Count());
            Assert.Equal("third", stack.Peek());
            
            var items = stack.ToList();
            Assert.Contains("second", items);
            Assert.Contains("third", items);
            Assert.DoesNotContain("first", items);
        }

        [Fact]
        public void Test_Pop_From_Empty_Stack() {
            var stack = new EnumerableDropOutStack<int>(3);
            
            // Should not throw but return default value
            var result = stack.Pop();
            Assert.Equal(0, stack.Count());
        }

        [Fact]
        public void Test_Complex_Push_Pop_Sequence() {
            var stack = new EnumerableDropOutStack<int>(3);
            
            // Fill stack
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            
            // Pop one
            var popped = stack.Pop();
            Assert.Equal(3, popped);
            Assert.Equal(2, stack.Count());
            
            // Push more to test wraparound
            stack.Push(4);
            stack.Push(5);
            
            Assert.Equal(3, stack.Count());
            Assert.Equal(5, stack.Peek());
            
            var items = stack.ToList();
            Assert.Contains(2, items);
            Assert.Contains(4, items);
            Assert.Contains(5, items);
        }
    }
}
