﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Roulette {
    /// <summary>
    /// Ref: https://edge.twinspires.com/casino-news/martingale-betting-system-in-roulette-and-blackjack-a-complete-guide/
    /// 'Following the Martingale system in roulette is simple. Start with a low bet amount of, for example, $1. 
    /// If you win, repeat. But if you lose, bet $2 next time. You keep doubling your bet amount until you win again. 
    /// Once you do, you go back to $1 and start over.'
    /// </summary>
    public class MartingaleBettingRecorder : GameRecorderBase {
        public MartingaleBettingRecorder(string outputPath, string filenamePrefix, GameCellColor selectedColor, int minimumBet, long initialBankroll, bool enableCsvWriter) {
            OutputPath = outputPath;
            FilenamePrefix = filenamePrefix;
            MinimumBet = minimumBet;
            CurrentBet = MinimumBet;
            SelectedColor = selectedColor;
            MaxBet = 0;
            InitialBankroll = initialBankroll;
            CurrentBankroll = InitialBankroll;
            BankrollOnLastWin = InitialBankroll;
            MaxBankroll = InitialBankroll;
            AverageBankroll = InitialBankroll;
            EnableCsvWriter = enableCsvWriter;
        }
        public bool EnableCsvWriter { get; set; } = false;

        protected int MinimumBet { get; init; } = 1;
        protected int BetMultiplier { get; init; } = 2;
        protected long MaxBet { get; set; }

        protected int CurrentNumConsecutiveLosses { get; set; }
        protected int MaxNumConsecutiveLosses { get; set; }
        protected int CurrentNumConsecutiveWins { get; set; }
        protected int MaxNumConsecutiveWins { get; set; }

        protected long MaxAmountWon { get; set; }
        protected long MaxAmountLost { get; set; }

        protected long CurrentBet { get; set; }
        protected long InitialBankroll { get; init; }
        protected long CurrentBankroll { get; set; } = 0;
        protected long BankrollOnLastWin { get; set; }
        protected long MaxBankroll { get; set; }
        public long MinBankroll { get; set; }
        public long AverageBankroll { get; set; }
        protected GameCellColor SelectedColor { get; init; }
        protected long SpinWhenLostAllMoney { get; set; }
        protected long CurrentNumSpins { get; set; }

        protected bool IsInitalized { get; set; } = false;

        public virtual string GetFilepath() => Path.Combine(OutputPath,!string.IsNullOrEmpty(FilenamePrefix)?$"{FilenamePrefix}{GetMethodCompactName()}-{SelectedColor}.txt":$"{GetMethodCompactName()}-{SelectedColor}.txt");
        public virtual string GetCsvFilepath() => Path.Combine(OutputPath, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}{GetMethodCompactName()}-{SelectedColor}-details.csv" : $"{GetMethodCompactName()}-{SelectedColor}-details");

        protected StreamWriter? CsvWriter { get; set; }
        public long SumPreviousBankrolls { get; protected set; }

        public async Task InitalizeAsync() {
            if(!EnableFileOutput) { return; }

            IsInitalized = true;
            if (string.IsNullOrEmpty(GetCsvFilepath())) {
                throw new ArgumentException($"Cannot initalize martingale csv writer because the csv filepath is empty");
            }
            CsvWriter = new StreamWriter(GetCsvFilepath(), false);
            await WriterHeaderAsync();
        }
        public async Task WriterHeaderAsync() {
            if (!EnableFileOutput) { return; }
            await CsvWriter!.WriteLineAsync("'spin number','spin value',bankroll,bet,winorloss,payout");
        }
        public async Task WriteCsvLineAsync(GameCell currentSpin,long startBankroll, long startBet, WinOrLoss winOrLoss,long payout) {
            if (!EnableFileOutput || !EnableCsvWriter) { return; }
            if (!IsInitalized) {
                await InitalizeAsync();
            }
            await CsvWriter!.WriteLineAsync($"{CurrentNumSpins},{currentSpin.Text},{startBankroll},{startBet},{winOrLoss},{payout}");
        }

        public override async Task RecordSpinAsync(GameCell cell) {
            CurrentNumSpins++;
            SumPreviousBankrolls += CurrentBankroll;
            // if you win, repeat the bet
            // if you lose, double the bet
            // once you win, reset bet amount to the initial bet amount

            // collect the values that we need to write the csv line
            long startBankroll = CurrentBankroll;
            long startBet = CurrentBet;
            var winOrLoss = WinOrLoss.Loss;
            var payout = (long)0;
            if (cell.Color == SelectedColor) {
                // won the bet
                winOrLoss = WinOrLoss.Win;
                // reset the bet amount back to the initial bet
                payout = GetPayoutForWin(CurrentBet);
                CurrentBankroll += payout;
                BankrollOnLastWin = CurrentBankroll;

                MaxAmountWon = MaxAmountWon > CurrentBet ? MaxAmountWon : CurrentBet;

                // CurrentBet = MinimumBet;

                CurrentNumConsecutiveWins++;
                MaxNumConsecutiveWins = MaxNumConsecutiveWins < CurrentNumConsecutiveWins ? CurrentNumConsecutiveWins : MaxNumConsecutiveWins;

                CurrentNumConsecutiveLosses = 0;
            }
            else {
                // lost the bet
                winOrLoss = WinOrLoss.Loss;
                payout = 0;
                // double the bet
                CurrentBankroll -= CurrentBet;
                if(SpinWhenLostAllMoney == 0 && CurrentBankroll < 0) {
                    SpinWhenLostAllMoney = CurrentNumSpins;
                }

                MaxAmountLost = MaxAmountLost < CurrentBet ? CurrentBet : MaxAmountLost;
                // CurrentBet *= BetMultiplier;

                CurrentNumConsecutiveLosses++;
                MaxNumConsecutiveLosses = MaxNumConsecutiveLosses < CurrentNumConsecutiveLosses ? CurrentNumConsecutiveLosses : MaxNumConsecutiveLosses;

                CurrentNumConsecutiveWins = 0;
            }

            CurrentBet = GetNextBetAmount(winOrLoss, CurrentBet, InitialBankroll, startBankroll);

            // TODO: Should refactor this becuase this needs to be copied to most sub-classes
            if(MaxBankroll < CurrentBankroll) {
                MaxBankroll = CurrentBankroll;
            }
            if(MinBankroll > CurrentBankroll) {
                MinBankroll = CurrentBankroll;
            }
            AverageBankroll = SumPreviousBankrolls / CurrentNumSpins;
            if(MaxBet < CurrentBet) {
                MaxBet = CurrentBet;
            }

            if (EnableCsvWriter) {
                await WriteCsvLineAsync(cell, startBankroll, startBet,winOrLoss,payout);
            }
        }
        protected virtual long GetPayoutForWin(long currentBet) => currentBet;

        protected virtual long GetNextBetAmount(WinOrLoss spinResult, long currentBet, long intialBankroll, long currentBankroll) =>
            spinResult switch {
                WinOrLoss.Win => MinimumBet,
                WinOrLoss.Loss => currentBet*2,
                _ => throw new ArgumentException(nameof(spinResult))
            };
        protected virtual string GetMethodDisplayName() => "Martingale betting method";
        protected virtual string GetMethodCompactName() => "martingale";
        // write the summary file now
        public override async Task GameCompleted() {
            if (!EnableFileOutput) { return; }

            var writer = new StreamWriter(GetFilepath(), true);

            await writer.WriteLineAsync($"{GetMethodDisplayName()} ".PadRight(60));
            await writer.WriteLineAsync($"  initial bet:                       ${MinimumBet:N0}");
            await writer.WriteLineAsync($"  initial bankroll:                  ${InitialBankroll:N0}");
            await writer.WriteLineAsync($"  current bankroll:                  ${CurrentBankroll:N0}");
            await writer.WriteLineAsync($"  max bankroll:                      ${MaxBankroll:N0}");
            await writer.WriteLineAsync($"  min bankroll:                      ${MinBankroll:N0}");
            await writer.WriteLineAsync($"  average bankroll:                  ${AverageBankroll:N0}");
            await writer.WriteLineAsync($"  max bet won:                       ${MaxAmountWon:N0}");
            await writer.WriteLineAsync($"  max bet lost:                      ${MaxAmountLost:N0}");
            await writer.WriteLineAsync($"  maximum bet played:                ${MaxBet:N0}");
            await writer.WriteLineAsync($"  maximum num consecutive wins:      {MaxNumConsecutiveWins:N0}");
            await writer.WriteLineAsync($"  maximum num consecutive losses:    {MaxNumConsecutiveLosses:N0}");
            await writer.WriteLineAsync($"  spin when went bankrupt:           {SpinWhenLostAllMoney:N0}");

            await writer.FlushAsync();

            if(EnableCsvWriter && CsvWriter != null) {
                await CsvWriter.FlushAsync();
            }
        }
    }
    public enum WinOrLoss {
        Win,
        Loss,
        NotSet
    }
}
