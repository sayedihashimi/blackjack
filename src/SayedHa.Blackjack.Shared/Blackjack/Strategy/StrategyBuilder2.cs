using SayedHa.Blackjack.Shared.Betting;
using SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    public class StrategyBuilder2 : IStrategyBuilder {
        public StrategyBuilder2() : this(new StrategyBuilderSettings()) { }
        public StrategyBuilder2(StrategyBuilderSettings settings) {
            Settings = settings;
        }
        public StrategyBuilderSettings Settings { 
            get; 
            init; 
        }

        public List<BlackjackStrategyTree> FindBestStrategies(int numToReturn) {
            throw new NotImplementedException();
        }

        public void PlayAndEvaluate(int numGamesToPlay, List<BlackjackStrategyTree> strategies, GameRunner gameRunner, Bankroll bankroll, BettingStrategy bettingStrategy) {
            throw new NotImplementedException();
        }
    }
}
