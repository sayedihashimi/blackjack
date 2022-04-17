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
}
