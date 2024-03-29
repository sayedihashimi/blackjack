﻿// This file is part of SayedHa.Blackjack.
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
    public class MartingaleBettingRecorder : GameRecorderBase, IGameRollupRecorder,IConsoleSummaryGameRecorder {
        public MartingaleBettingRecorder(string outputPath, string filenamePrefix, GameCellColor selectedColor, int minimumBet, long initialBankroll, bool enableCsvWriter) {
            OutputPath = outputPath;
            FilenamePrefix = filenamePrefix;
            MinimumBet = minimumBet;
            CurrentBet = MinimumBet;
            SelectedColor = selectedColor;
            MaxBetPlayed = 0;
            InitialBankroll = initialBankroll;
            CurrentBankroll = InitialBankroll;
            BankrollOnLastWin = InitialBankroll;
            MaxBankroll = InitialBankroll;
            MinBankroll = InitialBankroll;
            AverageBankroll = InitialBankroll;
            EnableCsvWriter = enableCsvWriter;
        }
        private bool _enableCsvWriter = true;
        public bool EnableCsvWriter {
            get {
                return EnableFileOutput && _enableCsvWriter;
            }
            set {
                _enableCsvWriter = value;
            }
        }

        protected int BetMultiplier { get; init; } = 2;
        protected long MaxBetPlayed { get; set; }

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

        public virtual string GetFilepath() => Path.Combine(OutputPath!, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}{GetMethodCompactName()}.txt" : $"{GetMethodCompactName()}.txt");
        public virtual string GetCsvFilepath() => Path.Combine(OutputPath!, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}{GetMethodCompactName()}-details.csv" : $"{GetMethodCompactName()}-details");

        protected StreamWriter? CsvWriter { get; set; }
        public long SumPreviousBankrolls { get; protected set; }

        public override bool IsBankrupt => CurrentBankroll <= 0;

        public override void Reset() {
            base.Reset();
            CurrentNumSpins = 0;
            CurrentBet = MinimumBet;
            MaxBetPlayed = 0;
            CurrentBankroll = InitialBankroll;
            BankrollOnLastWin = 0;
            MaxBankroll = InitialBankroll;
            MinBankroll = InitialBankroll;
            AverageBankroll = InitialBankroll;
            SumPreviousBankrolls = 0;
            MaxAmountLost = 0;
            MaxAmountWon = 0;
            SpinWhenLostAllMoney = 0;
            MaxNumConsecutiveLosses = 0;
            MaxNumConsecutiveWins = 0;
            CurrentNumConsecutiveWins = 0;
            CurrentNumConsecutiveLosses = 0;
        }

        public async Task InitalizeAsync() {
            if (!EnableFileOutput) { return; }

            IsInitalized = true;
            if (string.IsNullOrEmpty(GetCsvFilepath())) {
                throw new ArgumentException($"Cannot initalize martingale csv writer because the csv filepath is empty");
            }
            CsvWriter = new StreamWriter(GetCsvFilepath(), false);
            await WriterHeaderAsync();
        }
        public async Task WriterHeaderAsync() {
            if (!EnableFileOutput) { return; }
            await CsvWriter!.WriteLineAsync("'spin number','spin value',bankroll,bet,'win or loss',payout");
        }
        public async Task WriteCsvLineAsync(GameCell currentSpin, long startBankroll, long startBet, WinOrLoss winOrLoss, long payout) {
            if (!EnableFileOutput || !EnableCsvWriter) { return; }
            if (!IsInitalized) {
                await InitalizeAsync();
            }
            await CsvWriter!.WriteLineAsync($"{CurrentNumSpins},{currentSpin.Text},{startBankroll},{startBet},{winOrLoss},{payout}");
        }

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
            CurrentBet = CurrentBet < MaximumBet ? CurrentBet : MaximumBet;
            if (!AllowNegativeBankroll && (CurrentBankroll - CurrentBet) < 0) {
                CurrentBet = CurrentBankroll;
            }
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

                MaxAmountWon = payout > MaxAmountWon ? payout : MaxAmountWon;

                CurrentNumConsecutiveLosses = 0;
                CurrentNumConsecutiveWins++;
                MaxNumConsecutiveWins = MaxNumConsecutiveWins < CurrentNumConsecutiveWins ? CurrentNumConsecutiveWins : MaxNumConsecutiveWins;
            }
            else {
                // lost the bet
                winOrLoss = WinOrLoss.Loss;
                payout = -1*CurrentBet;
                CurrentBankroll -= CurrentBet;
                if (SpinWhenLostAllMoney == 0 && CurrentBankroll <= 0) {
                    SpinWhenLostAllMoney = CurrentNumSpins;
                }

                MaxAmountLost = MaxAmountLost < CurrentBet ? CurrentBet : MaxAmountLost;
                // CurrentBet *= BetMultiplier;

                CurrentNumConsecutiveWins = 0;
                CurrentNumConsecutiveLosses++;
                MaxNumConsecutiveLosses = MaxNumConsecutiveLosses < CurrentNumConsecutiveLosses ? CurrentNumConsecutiveLosses : MaxNumConsecutiveLosses;
            }

            CurrentBet = GetNextBetAmount(winOrLoss, CurrentBet, InitialBankroll, startBankroll);

            // TODO: Should refactor this becuase this needs to be copied to most sub-classes
            if (CurrentBankroll > MaxBankroll) {
                MaxBankroll = CurrentBankroll;
            }
            if (CurrentBankroll < MinBankroll) {
                MinBankroll = CurrentBankroll;
            }
            AverageBankroll = SumPreviousBankrolls / CurrentNumSpins;
            if (MaxBetPlayed < CurrentBet) {
                MaxBetPlayed = CurrentBet;
            }

            if (EnableCsvWriter) {
                await WriteCsvLineAsync(cell, startBankroll, startBet, winOrLoss, payout);
            }
        }
        protected virtual long GetPayoutForWin(long currentBet) => currentBet;

        protected virtual long GetNextBetAmount(WinOrLoss spinResult, long currentBet, long intialBankroll, long currentBankroll) =>
            spinResult switch {
                WinOrLoss.Win => MinimumBet,
                WinOrLoss.Loss => currentBet * 2 < MaximumBet ? currentBet * 2 : MaximumBet,
                _ => throw new ArgumentException(nameof(spinResult))
            };
        public virtual string GetMethodDisplayName() => "Martingale betting method";
        public virtual string GetMethodCompactName() => $"martingale-{SelectedColor}";
        //string IGameRollupRecorder.GetMethodDisplayName() => "Martingale betting method";

        //string IGameRollupRecorder.GetMethodCompactName() => "martingale";
        // write the summary file now
        public override async Task GameCompleted() {
            if (!EnableFileOutput) { return; }

            var writer = new StreamWriter(GetFilepath(), true);

            await WriteTextSummaryToAsync(writer);

            if (EnableCsvWriter && CsvWriter != null) {
                await CsvWriter.FlushAsync();
            }
        }

        public async Task WriteGameSummaryToAsync(StreamWriter writer) {
            await writer.WriteAsync($"{MinimumBet},");
            await writer.WriteAsync($"{MaxBetPlayed},");
            await writer.WriteAsync($"{InitialBankroll},");
            await writer.WriteAsync($"{CurrentBankroll},");
            await writer.WriteAsync($"{MaxBankroll},");
            await writer.WriteAsync($"{MinBankroll},");
            await writer.WriteAsync($"{AverageBankroll},");
            await writer.WriteAsync($"{MaxAmountWon},");
            await writer.WriteAsync($"{MaxAmountLost},");
            await writer.WriteAsync($"{MaxBetPlayed},");
            await writer.WriteAsync($"{MaxNumConsecutiveWins},");
            await writer.WriteAsync($"{MaxNumConsecutiveLosses},");
            await writer.WriteAsync($"{SpinWhenLostAllMoney}");
            await writer.WriteLineAsync(string.Empty);
        }

        public async Task WriteGameSummaryHeaderToAsync(StreamWriter writer) {
            await writer.WriteLineAsync("minimumBet,maximumBetPlayed,initialBankroll,endingBankroll,maxBankroll,minBankroll,averageBankroll,maxBetWon,maxBetLost,maxBetPlayed,maxNumConsecutiveWins,maxNumConsecutiveLosses,spinWhenGoneBust");
        }

        public async Task WriteTextSummaryToAsync(StreamWriter writer) {
            await writer.WriteLineAsync($"{GetMethodDisplayName()} - # spins: {CurrentNumSpins:N0} ".PadRight(60));
            await writer.WriteLineAsync($"  initial bet:                       ${MinimumBet:N0}");
            await writer.WriteLineAsync($"  max bet placed:                    ${MaxBetPlayed:N0}");
            await writer.WriteLineAsync($"  initial bankroll:                  ${InitialBankroll:N0}");
            await writer.WriteLineAsync($"  current bankroll:                  ${CurrentBankroll:N0}");
            await writer.WriteLineAsync($"  max bankroll:                      ${MaxBankroll:N0}");
            await writer.WriteLineAsync($"  min bankroll:                      ${MinBankroll:N0}");
            await writer.WriteLineAsync($"  average bankroll:                  ${AverageBankroll:N0}");
            await writer.WriteLineAsync($"  max payout:                        ${MaxAmountWon:N0}");
            await writer.WriteLineAsync($"  max bet lost:                      ${MaxAmountLost:N0}");
            await writer.WriteLineAsync($"  maximum bet played:                ${MaxBetPlayed:N0}");
            await writer.WriteLineAsync($"  maximum num consecutive wins:      {MaxNumConsecutiveWins:N0}");
            await writer.WriteLineAsync($"  maximum num consecutive losses:    {MaxNumConsecutiveLosses:N0}");
            await writer.WriteLineAsync($"  spin when went bankrupt:           {SpinWhenLostAllMoney:N0}");

            await writer.FlushAsync();
        }
    }
    public enum WinOrLoss {
        Win,
        Loss,
        NotSet
    }
}
