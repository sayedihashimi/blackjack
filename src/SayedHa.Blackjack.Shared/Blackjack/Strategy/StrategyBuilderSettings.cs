namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    public class StrategyBuilderSettings
    {
        public int NumDecks { get; set; } = 4;
        // TODO: Get this from somewhere.
        public bool UseRandomNumberGenerator { get; set; } = true;
        public int NumStrategiesForFirstGeneration { get; set; } = 100;
        // half the population, the other half will be offspring
        public int NumStrategiesToGoToNextGeneration {get;set;} = 10;
        public int NumHandsToPlayForEachStrategy { get; set; } = 1000;
        public int InitialBankroll { get; set; } = 10000;
        public int BetAmount { get; set; } = 5;
        public int MaxNumberOfGenerations{get;set;} = 3;
        public int InitialMutationRate { get; set; } = 75;
        /// <summary>
        /// The percent to reduce the mutation by each generation. Must be >=0 and <= 100.
        /// </summary>
        public int MutationRateChangePerGeneration { get; set; } = 2;
    }
}
