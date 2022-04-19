using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Betting {
    public class BasicHiLoStrategy : BettingStrategy {
        public BasicHiLoStrategy(Bankroll bankroll, int betUnitValue, int maxBetSpread) : base(bankroll) {
            MaxBetSpread = maxBetSpread;
            BetUnitValue = betUnitValue;
        }
        public override int GetNextBetAmount(Game game) {
            Debug.Assert(game != null);
            Debug.Assert(game.Cards != null);

            var currentCount = GetCount(game.Cards);

            float betMultiplier = 1F;
            switch (currentCount.TrueCount) {
                case <=0:  // 1
                    break;
                case 1: 
                    betMultiplier = MaxBetSpread/6F;
                    break;
                case 2:
                    betMultiplier = MaxBetSpread/5F;
                    break;
                case 3: // 6
                    betMultiplier = MaxBetSpread/2F;
                    break;
                case 4:
                    betMultiplier = MaxBetSpread/1.66F;
                    break;
                case 5:
                    betMultiplier = MaxBetSpread/1.33F;
                    break;
                case >=6: // 12
                    betMultiplier = MaxBetSpread;
                    break;
            }

            float nextBet = betMultiplier * BetUnitValue;
            int proposedBet = (int)(Math.Floor(nextBet));
            return proposedBet >= 0 ? proposedBet : 1;
        }
        protected int MaxBetSpread { get; init; }
        protected float BetUnitValue { get; init; }
        // TODO: Don't count the very first discarded card, the opponent(s) didn't get a chance to see that card
        protected HiLoCount GetCount(CardDeck cards) {
            Debug.Assert(cards != null);
            Debug.Assert(cards.DiscardedCards != null);
            // todo: is there a better way to do this?
            int currentCount = 0;
            foreach(var card in cards.DiscardedCards) {
                switch (card.Number.GetValues()[0]) {
                    case 11:
                    case 10:
                    case 9:
                    case 8:
                        currentCount--;
                        break;
                    case 6:
                    case 5:
                    case 4:
                    case 3:
                    case 2:
                        currentCount++;
                        break;
                    default:
                        break;
                }
            }

            int numDiscardedDecks = (int)Math.Round(cards.DiscardedCards.Count / 52F);
            int numRemainingDecks = cards.NumDecks - numDiscardedDecks;
            int trueCount = (int)(Math.Floor((double)currentCount / (double)numRemainingDecks));

            return new HiLoCount {
                RunningCount = currentCount,
                TrueCount = trueCount,
            };
        }
    }
    public class HiLoCount {
        public int RunningCount { get; set; }
        public int TrueCount { get; set; }
    }
}
