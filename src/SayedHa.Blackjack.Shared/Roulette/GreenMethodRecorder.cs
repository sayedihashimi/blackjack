using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Roulette {
    /// <summary>
    /// Green betting method:
    /// 1. start with one unit bet on the green "0 & 00" bet which typically has 17:1 odds
    /// 2. if you lose, increase bet by one unit when a win at the current bet rate woulnd't recoup losses and a small gain
    /// 3. when you win, go back to 1 unit bet
    /// </summary>
    public class GreenMethodRecorder : MartingaleBettingRecorder {
        public GreenMethodRecorder(string filepath, string csvFilepath, int minimumBet, long initialDollarAmount) : 
            base(filepath, csvFilepath, GameCellColor.Green, minimumBet, initialDollarAmount) {
        }
        protected override long GetNextBetAmount(WinOrLoss spinResult, long currentBet, long intialBankroll, long currentBankroll) =>
            spinResult switch {
                WinOrLoss.Win => MinimumBet,
                WinOrLoss.Loss => MinimumBet * ((int)Math.Ceiling(Math.Abs(((double)CurrentDollarAmount - DollarAmountOnLastWin) /(17*(double)MinimumBet)))),
                _ => throw new ArgumentException(nameof(spinResult))
            };
        protected override string GetMethodDisplayName() => "Green only betting method";
        protected override long GetPayoutForWin(long currentBet) => currentBet * 17;
    }

    public class GreenAgressiveMethodRecorder : GreenMethodRecorder {
        public GreenAgressiveMethodRecorder(string filepath, string csvFilepath, int minimumBet, long initialDollarAmount) : base(filepath, csvFilepath, minimumBet, initialDollarAmount) {
        }

        protected override string GetMethodDisplayName() => "Green aggressive only betting method";
        protected override long GetNextBetAmount(WinOrLoss spinResult, long currentBet, long intialBankroll, long currentBankroll) {
            var betAmount = spinResult switch {
                WinOrLoss.Win => getWinBet(),
                WinOrLoss.Loss => getMultiplier() * MinimumBet * ((int)Math.Ceiling(Math.Abs(((double)CurrentDollarAmount - DollarAmountOnLastWin) / (17 * (double)MinimumBet)))),
                _ => throw new ArgumentException(nameof(spinResult))
            };

            // TODO: Not sure if I should be using DollarAmountOnLastWin or just InitialDollarAmount
            long getWinBet() {
                if (CurrentDollarAmount < InitialDollarAmount) {
                    return MinimumDollarAmount;
                }

                // compute a bet multiplier based on how much has been gained
                int multiplier = (int)Math.Floor((double)CurrentDollarAmount / InitialDollarAmount);
                // for every $500 profit, increase bet by MinimumBet
                multiplier = (int)Math.Floor(((double)CurrentDollarAmount - InitialDollarAmount) / 1000);
                multiplier = multiplier > 0 ? multiplier : 1;
                var amt = CurrentDollarAmount - InitialDollarAmount > 0 ? MinimumBet * multiplier : MinimumBet;

                return amt;
            }

            int getMultiplier() {
                int multiplier = (int)Math.Floor(((double)CurrentDollarAmount - InitialDollarAmount) / 1000);
                multiplier = multiplier > 0 ? multiplier : 1;
                return multiplier;
            }

            return betAmount;
        }
    }

}
