﻿namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    public class StrategyBuilderSettings
    {
        public int NumDecks { get; set; } = 4;
        // TODO: Get this from somewhere.
        public bool UseRandomNumberGenerator { get; set; } = true;
        public int NumStrategiesForFirstGeneration { get; set; } = 500;
        // half the population, the other half will be offspring
        public int NumStrategiesToGoToNextGeneration {get;set;} = 250;
        public int NumHandsToPlayForEachStrategy { get; set; } = 100;
        public int InitialBankroll { get; set; } = 10000;
        public int BetAmount { get; set; } = 5;
        public int MaxNumberOfGenerations{get;set;} = 2;
    }
}
