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
        protected override string GetMethodDisplayName() => "Bond martingale";
        protected override string GetMethodCompactName() => "bondmartingale";
        public override string GetFilepath() => Path.Combine(OutputPath, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}{GetMethodCompactName()}.txt" : $"{GetMethodCompactName()}.txt");
        public override string GetCsvFilepath() => Path.Combine(OutputPath, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}{GetMethodCompactName()}-details.csv" : $"{GetMethodCompactName()}-details");
        protected override long GetNextBetAmount(WinOrLoss spinResult, long currentBet, long intialBankroll, long currentBankroll) =>
            spinResult switch {
                WinOrLoss.Loss => currentBet * 2,
                WinOrLoss.Win => MinimumBet,
                _ => throw new ArgumentOutOfRangeException(nameof(spinResult))
            };
        public override async Task RecordSpinAsync(GameCell cell) {
            CurrentNumSpins++;
            SumPreviousBankrolls += CurrentBankroll;

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

            CurrentBankroll -= CurrentBet;
            CurrentBankroll += payout;

            if(winOrLoss == WinOrLoss.Win) {
                MaxAmountWon = MaxAmountWon > CurrentBet ? MaxAmountWon : CurrentBet;
                CurrentNumConsecutiveWins++;
                MaxNumConsecutiveWins = CurrentNumConsecutiveWins > MaxNumConsecutiveWins ? CurrentNumConsecutiveWins : MaxNumConsecutiveWins;
                CurrentNumConsecutiveLosses = 0;
            }
            else {
                CurrentNumConsecutiveLosses++;
                MaxNumConsecutiveLosses = MaxNumConsecutiveLosses < CurrentNumConsecutiveLosses ? CurrentNumConsecutiveLosses : MaxNumConsecutiveLosses;
                CurrentNumConsecutiveWins = 0;
            }

            if (SpinWhenLostAllMoney == 0 && CurrentBankroll < 0) {
                SpinWhenLostAllMoney = CurrentNumSpins;
            }
            if (MaxBankroll < CurrentBankroll) {
                MaxBankroll = CurrentBankroll;
            }
            if (MinBankroll > CurrentBankroll) {
                MinBankroll = CurrentBankroll;
            }
            AverageBankroll = SumPreviousBankrolls / CurrentNumSpins;
            if (MaxBet < CurrentBet) {
                MaxBet = CurrentBet;
            }

            if (EnableCsvWriter) {
                await WriteCsvLineAsync(cell, startBankroll, startBet, winOrLoss,payout);
            }
        }

    }
}
