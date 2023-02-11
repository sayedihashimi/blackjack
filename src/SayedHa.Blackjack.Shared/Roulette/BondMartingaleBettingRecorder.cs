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
using System.ComponentModel.DataAnnotations;
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
        public BondMartingaleBettingRecorder(string outputPath,string filenamePrefix, int minimumBet, long initialBankroll, bool enableCsvWriter) :
            base(outputPath, filenamePrefix, GameCellColor.Green, minimumBet, initialBankroll, enableCsvWriter) {
            // Note: GameCellColor.Green will be ignored in the base class
        }
        public override string GetMethodDisplayName() => "Bond martingale";
        public override string GetMethodCompactName() => "bondmartingale";

        public override string GetFilepath() => Path.Combine(OutputPath!, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}{GetMethodCompactName()}.txt" : $"{GetMethodCompactName()}.txt");
        public override string GetCsvFilepath() => Path.Combine(OutputPath!, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}{GetMethodCompactName()}-details.csv" : $"{GetMethodCompactName()}-details");

        public override async Task RecordSpinAsync(GameCell cell) {
            if (StopWhenBankrupt && IsBankrupt) {
                //if (SpinWhenLostAllMoney == 0 && CurrentBankroll <= 0) {
                //    SpinWhenLostAllMoney = CurrentNumSpins;
                //}
                // ignore the spin
                return;
            }
            CurrentNumSpins++;
            SumPreviousBankrolls += CurrentBankroll;
            CurrentBet = CurrentBet < MaximumBet? CurrentBet : MaximumBet;
            
            if(!AllowNegativeBankroll && (CurrentBankroll - CurrentBet) < 0) {
                CurrentBet = CurrentBankroll;
            }

            double percentOfBetThatPaidOut = 0;
            int betMultiplierOfWinningBet = 0;
            // determine if it was a win or loss first
            switch (cell.Value) {
                case >=19 and <=36:
                    percentOfBetThatPaidOut = 0.7;
                    betMultiplierOfWinningBet = 1;
                    break;
                case >= 13 and <= 18:
                    percentOfBetThatPaidOut = 0.25;
                    betMultiplierOfWinningBet = 5;
                    break;
            }
            switch (cell.Color) {
                case GameCellColor.Green:
                    percentOfBetThatPaidOut = 0.05;
                    betMultiplierOfWinningBet = 17;
                    break;
                default:
                    break;
            }

            var winOrLoss = percentOfBetThatPaidOut > 0 ? WinOrLoss.Win : WinOrLoss.Loss;
            var payout = winOrLoss == WinOrLoss.Win ? ((int)Math.Floor(percentOfBetThatPaidOut * CurrentBet)) * betMultiplierOfWinningBet : 0;
            long startBankroll = CurrentBankroll;
            long startBet = CurrentBet;

            // bet amount needs to be deducted here because there are multiple bets
            // in win section we will give the bet that won back to the player
            CurrentBankroll -= CurrentBet;

            if (winOrLoss == WinOrLoss.Win) {
                CurrentNumConsecutiveWins++;
                CurrentBankroll += payout;
                // give back the bet on the cell that hit
                CurrentBankroll += ((int)Math.Floor(percentOfBetThatPaidOut * CurrentBet));
                MaxAmountWon = payout > MaxAmountWon ? payout : MaxAmountWon;

                MaxNumConsecutiveWins = CurrentNumConsecutiveWins > MaxNumConsecutiveWins ? CurrentNumConsecutiveWins : MaxNumConsecutiveWins;
                CurrentNumConsecutiveLosses = 0;
                CurrentBet = MinimumBet;
            }
            else {
                CurrentNumConsecutiveLosses++;
                MaxNumConsecutiveLosses = MaxNumConsecutiveLosses < CurrentNumConsecutiveLosses ? CurrentNumConsecutiveLosses : MaxNumConsecutiveLosses;
                CurrentNumConsecutiveWins = 0;
                MaxAmountLost = MaxAmountLost < CurrentBet ? CurrentBet : MaxAmountLost;
                CurrentBet = CurrentBet*2<MaximumBet?CurrentBet*2:MaximumBet;
            }

            if (SpinWhenLostAllMoney == 0 && CurrentBankroll <= 0) {
                SpinWhenLostAllMoney = CurrentNumSpins;
            }
            if (CurrentBankroll > MaxBankroll) {
                MaxBankroll = CurrentBankroll;
            }
            if (CurrentBankroll < MinBankroll) {
                MinBankroll = CurrentBankroll;
            }
            AverageBankroll = SumPreviousBankrolls / CurrentNumSpins;
            if (MaxBetPlayed < startBet) {
                MaxBetPlayed = startBet;
            }

            if (EnableCsvWriter) {
                await WriteCsvLineAsync(cell, startBankroll, startBet, winOrLoss,payout);
            }
        }
    }
}
