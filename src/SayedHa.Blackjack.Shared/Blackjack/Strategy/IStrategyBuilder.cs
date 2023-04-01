using SayedHa.Blackjack.Shared.Betting;
using SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    public interface IStrategyBuilder {
        StrategyBuilderSettings Settings { get; init; }

        List<BlackjackStrategyTree> FindBestStrategies(int numToReturn);
        void PlayAndEvaluate(int numGamesToPlay, List<BlackjackStrategyTree> strategies, GameRunner gameRunner, Bankroll bankroll, BettingStrategy bettingStrategy);
    }
}