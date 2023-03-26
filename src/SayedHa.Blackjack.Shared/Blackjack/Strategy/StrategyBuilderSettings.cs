namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    public class StrategyBuilderSettings
    {
        public int NumDecks { get; set; } = 4;
        // TODO: Get this from somewhere.
        public bool UseRandomNumberGenerator { get; set; } = true;
        public int NumStrategiesForFirstGeneration { get; set; } = 10000;
        // half the population, the other half will be offspring
        public int NumStrategiesToGoToNextGeneration {get;set;} = 5000;
        public int NumHandsToPlayForEachStrategy { get; set; } = 2500;
        public int InitialBankroll { get; set; } = 10000;
        public int BetAmount { get; set; } = 5;
        public int MaxNumberOfGenerations{get;set;} = 5;
        public int InitialMutationRate { get; set; } = 75;
        public int MinMutationRate { get; set; } = 20;
        /// <summary>
        /// The percent to reduce the mutation by each generation. Must be >=0 and <= 100.
        /// </summary>
        public int MutationRateChangePerGeneration { get; set; } = 1;
    }
}
