namespace SayedHa.Blackjack.Shared.Extensions {
    // https://stackoverflow.com/a/1262619/105999
    public static class ListExtensions {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
