namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    public class StrategyBuilderSettings
    {
        public int NumDecks { get; set; } = 4;
        // TODO: Get this from somewhere.
        public bool UseRandomNumberGenerator { get; set; } = true;
        public int NumStrategiesForFirstGeneration { get; set; } = 1000;
        public int NumStrategiesToGoToNextGeneration {get;set;} = 30;
        public int NumHandsToPlayForEachStrategy { get; set; } = 10000;
        public int InitialBankroll { get; set; } = 10000;
        public int BetAmount { get; set; } = 5;
        public int MaxNumberOfGenerations{get;set;} = 500;
        public int InitialMutationRate { get; set; } = 50;
        public int MinMutationRate { get; set; } = 5;
        /// <summary>
        /// The percent to reduce the mutation by each generation. Must be >=0 and <= 100.
        /// </summary>
        public int MutationRateChangePerGeneration { get; set; } = 1;

        public bool EnableMultiThreads { get; set; } = true;
        public int MtMaxNumThreads { get; set; } = 72;
        // public int MtMaxNumThreads { get; set; } = 24;
        public bool AllConsoleOutputDisabled { get; set; } = false;

        public int TournamentSize { get; set; } = 3;
    }
}
