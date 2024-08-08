namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    public class StrategyBuilderSettings
    {
        public int NumDecks { get; set; } = 4;
        // TODO: Get this from somewhere.
        public bool UseRandomNumberGenerator { get; set; } = true;
        public int NumStrategiesForFirstGeneration { get; set; } = 2000;
        public int NumStrategiesToGoToNextGeneration {get;set;} = 500;
        public int NumHandsToPlayForEachStrategy { get; set; } = 50000;
        public int InitialBankroll { get; set; } = 10000;
        public int BetAmount { get; set; } = 5;
        public int MaxNumberOfGenerations{get;set;} = 100;
        public int InitialMutationRate { get; set; } = 50;
        public int MinMutationRate { get; set; } = 5;
        /// <summary>
        /// The percent to reduce the mutation by each generation. Must be >=0 and <= 100.
        /// </summary>
        public int MutationRateChangePerGeneration { get; set; } = 1;

        // introducing a different way to do mutations, currently it visits every cell
        // these settings will be for that new method.
        public float CellMutationNumCellsToChangePerChart { get; set; } = 20F;
        public float CellMutationMinNumCellsToChangePerChart { get; set; } = 8F;
        public float CellMutationRateChangePerGeneration { get; set; } = 0.2F;
        public bool CreateSmartRandomStrategies { get; set; } = true;
        public int NumOfBestStrategyClonesToMakePerGeneration { get; set; } = 30;

        public bool EnableMultiThreads { get; set; } = true;
        public int MtMaxNumThreads { get; set; } = 72;
        // public int MtMaxNumThreads { get; set; } = 24;
        public bool AllConsoleOutputDisabled { get; set; } = false;

        public int TournamentSize { get; set; } = 3;
    }
}
