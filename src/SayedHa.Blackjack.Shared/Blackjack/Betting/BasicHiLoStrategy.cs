// This file is part of SayedHa.Blackjack.
//
// SayedHa.Blackjack is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SayedHa.Blackjack is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with SayedHa.Blackjack.  If not, see <https://www.gnu.org/licenses/>.
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
        protected internal HiLoCount GetCount(CardDeck cards) {
            Debug.Assert(cards != null);
            Debug.Assert(cards.DiscardedCards != null);
            // todo: is there a better way to do this?
            int currentCount = 0;
            foreach(var card in cards.DiscardedCards) {
                // only count cards that the players saw
                if (!card.WasCardBurned) {
                    switch (card.Number.GetValues()[0]) {
                        case 11:
                        case 10:
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
            }

            var numDiscardedDecks = cards.DiscardedCards.Count / 52F;
            var numRemainingDecks = cards.NumDecks - numDiscardedDecks;
            var trueCount = (float)currentCount / numRemainingDecks;

            return new HiLoCount {
                RunningCount = currentCount,
                TrueCount = trueCount,
            };
        }
    }
    public class HiLoCount {
        public int RunningCount { get; set; }
        public float TrueCount { get; set; }
    }
}
