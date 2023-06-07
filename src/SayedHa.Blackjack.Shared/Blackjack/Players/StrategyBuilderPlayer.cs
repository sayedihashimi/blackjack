using SayedHa.Blackjack.Shared.Betting;
using SayedHa.Blackjack.Shared.Blackjack.Strategy;
using SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree;
using SayedHa.Blackjack.Shared.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Players {
    public class StrategyBuilderPlayer : Player {
        public StrategyBuilderPlayer(BlackjackStrategyTree strategyTree) {
            StrategyTree = strategyTree;
        }

        public BlackjackStrategyTree StrategyTree { get; init; }
        public override HandActionAndReason GetNextAction(Hand hand, DealerHand dealerHand, int dollarsRemaining) {
            var opCardNumbers = new CardNumber[hand.DealtCards.Count];
            for(int i = 0; i < hand.DealtCards.Count; i++) {
                opCardNumbers[i] = hand.DealtCards[i].Number;
            }

            if(opCardNumbers.Length == 2) {
                return new HandActionAndReason(StrategyTree.GetNextHandAction(dealerHand.DealersVisibleCard!.Number, opCardNumbers[0], opCardNumbers[1]));
            }
            else {
                return new HandActionAndReason(StrategyTree.GetNextHandAction(dealerHand.DealersVisibleCard!.Number, opCardNumbers));
            }
        }
    }

    public class StrategyBuilderParticipantFactory : ParticipantFactory {
        public StrategyBuilderParticipantFactory(BlackjackStrategyTree strategyTree, BettingStrategy bettingStrategy, StrategyBuilderSettings settings, ILogger logger) : 
            base(bettingStrategy, OpponentPlayStrategy.Custom, logger) {

            StrategyTree = strategyTree;
            Settings = settings;
        }
        public override Participant CreateNewOpponent(OpponentPlayStrategy strategy, ILogger logger) => 
            new StrategyBuilderParticipant(
                ParticipantRole.Player,
                new StrategyBuilderPlayer(StrategyTree),
                new Bankroll(Settings.InitialBankroll, NullLogger.Instance));

        protected internal BlackjackStrategyTree StrategyTree { get; init; }
        protected internal StrategyBuilderSettings Settings { get; init; }
    }
    public class StrategyBuilderParticipant : Participant {
        public StrategyBuilderParticipant(ParticipantRole role, Player player, Bankroll bankroll) : base(role, player, new FixedBettingStrategy(bankroll, 5)) {

        }
    }
}
