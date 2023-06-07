using System.Security.Cryptography;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    public class RandomHelper {
        
        private Random random = new Random();

        public static RandomHelper Instance { get; private set; } = new RandomHelper();
        public bool UseRandomNumberGenerator { get; set; } = true;

        public int GetRandomIntBetween(int fromInclusive, int toExclusive) => UseRandomNumberGenerator switch {
            true => RandomNumberGenerator.GetInt32(fromInclusive, toExclusive),
            false => random.Next(fromInclusive, toExclusive)
        };
        public bool GetRandomBool() => UseRandomNumberGenerator switch {
            true => RandomNumberGenerator.GetInt32(2) == 0,
            false => random.Next(2) == 0
        };
        public (int num1, int num2) GetTwoRandomNumbersBetween(int fromInclusive, int toExclusive) {
            int num1;

            num1 = GetRandomIntBetween(fromInclusive, toExclusive);
            int num2 = num1;
            while(num2 == num1) {
                num2 = GetRandomIntBetween(fromInclusive, toExclusive);
            }

            return (num1, num2);
        }
        public void RandomizeArray(int[,] array,int valueFromInclusive,int valueToExclusive) {
            for(var i = 0; i < array.GetLength(0); i++) {
                for(var j = 0;j<array.GetLength(1); j++) {
                    array[i,j] = GetRandomIntBetween(valueFromInclusive, valueToExclusive);
                }
            }
        }
    }
}
