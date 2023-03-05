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
using System.Security.Cryptography;

namespace SayedHa.Blackjack.Shared.Extensions {
    // https://stackoverflow.com/a/1262619/105999
    public static class ListExtensions {
        private static Random rng = new Random();
        private static RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();

        public static void Shuffle<T>(this IList<T> list, bool useRandomNumberGenerator = false) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = useRandomNumberGenerator switch {
                    true => RandomNumberGenerator.GetInt32(n+1),
                    false => rng.Next(n + 1),
                };

                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static LinkedList<T> ConvertToLinkedList<T>(this List<T> cards) {
            var result = new LinkedList<T>();
            foreach (var item in cards) {
                result.AddLast(item);
            }
            return result;
        }
    }
}
