using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Roulette {
    /// <summary>
    /// Three bets are placed at once:
    ///  1. 70% of bet on 19 - 36 
    ///     (variations include any other bet that covers half the numbers)
    ///  2. 25% of bet on 13 - 18 (or any bet that covers 6 other numbers)
    ///  3. 5% of bet on 0 (or 0 & 00 for American)
    ///  
    /// If you lose, double the bet
    /// If you win, go back to the original bet
    /// </summary>
    public class BondMartingaleBettingRecorder : MartingaleBettingRecorder {
        // TODO: Consider making this recorder configurable for what bets are being used for the first two bets
        //       Need to be a bit careful with this so it doesn't impact perf much
        public BondMartingaleBettingRecorder(string filepath, string csvFilepath, GameCellColor selectedColor, int minimumBet, long initialDollarAmount) : base(filepath, csvFilepath, selectedColor, minimumBet, initialDollarAmount) {
            
        }
        protected override string GetMethodDisplayName() => "Bond martingale";
        protected override long GetNextBetAmount(WinOrLoss spinResult, long currentBet, long intialBankroll, long currentBankroll) =>
            spinResult switch {
                WinOrLoss.Loss => currentBet * 2,
                WinOrLoss.Win => MinimumBet,
                _ => throw new ArgumentOutOfRangeException(nameof(spinResult))
            };
        public override Task RecordSpinAsync(GameCell cell) {
            CurrentNumSpins++;

            // determine if it was a win or loss first
            switch (cell.Text) {
                
            }



            return base.RecordSpinAsync(cell);
        }

    }
}
