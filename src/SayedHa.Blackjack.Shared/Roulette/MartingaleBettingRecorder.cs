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
    public class MartingaleBettingRecorder : GameRecorderBase {
        public MartingaleBettingRecorder(string filepath, string csvFilepath, GameCellColor selectedColor, int minimumBet, long initialDollarAmount) {
            MinimumBet = minimumBet;
            CurrentBet = MinimumBet;
            SelectedColor = selectedColor;
            MaxBet = 0;
            Filepath = filepath;
            InitialDollarAmount = initialDollarAmount;
            CurrentDollarAmount = InitialDollarAmount;
            DollarAmountOnLastWin = InitialDollarAmount;
            MaximumDollarAmount = InitialDollarAmount;
            CsvFilepath = csvFilepath;
            if (!string.IsNullOrEmpty(CsvFilepath)) {
                EnableCsvWriter = true;
            }

            EnableCsvWriter = string.IsNullOrEmpty(CsvFilepath) == false;
        }
        private bool disposedValue;
        public bool EnableCsvWriter { get; set; } = false;
        protected string Filepath { get; set; }
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
        protected long InitialDollarAmount { get; init; }
        protected long CurrentDollarAmount { get; set; } = 0;
        protected long DollarAmountOnLastWin { get; set; }
        protected long MaximumDollarAmount { get; set; }
        public long MinimumDollarAmount { get; set; }
        protected GameCellColor SelectedColor { get; init; }
        protected long SpinWhenLostAllMoney { get; set; }
        protected long CurrentNumSpins { get; set; }

        protected bool IsInitalized { get; set; } = false;
        protected string CsvFilepath { get; set; }
        protected StreamWriter? CsvWriter { get; set; }
        

        public async Task InitalizeAsync() {
            IsInitalized = true;
            if (string.IsNullOrEmpty(CsvFilepath)) {
                throw new ArgumentNullException(nameof(CsvFilepath));
            }
            CsvWriter = new StreamWriter(CsvFilepath, false);
            await WriterHeaderAsync();
        }
        public async Task WriterHeaderAsync() {
            await CsvWriter!.WriteLineAsync("'spin number','spin value',bankroll,bet,winorloss,payout");
        }
        public async Task WriteCsvLineAsync(GameCell currentSpin,long startDollarAmount, long startBet, WinOrLoss winOrLoss,long payout) {
            if (!IsInitalized) {
                await InitalizeAsync();
            }
            await CsvWriter!.WriteLineAsync($"{CurrentNumSpins},{currentSpin.Text},{startDollarAmount},{startBet},{winOrLoss},{payout}");
        }

        public override async Task RecordSpinAsync(GameCell cell) {
            CurrentNumSpins++;
            // if you win, repeat the bet
            // if you lose, double the bet
            // once you win, reset bet amount to the initial bet amount

            // collect the values that we need to write the csv line
            long startDollarAmount = CurrentDollarAmount;
            long startBet = CurrentBet;
            var winOrLoss = WinOrLoss.Loss;
            var payout = (long)0;
            if (cell.Color == SelectedColor) {
                // won the bet
                winOrLoss = WinOrLoss.Win;
                // reset the bet amount back to the initial bet
                // CurrentDollarAmount += CurrentBet;
                payout = GetPayoutForWin(CurrentBet);
                CurrentDollarAmount += payout;
                DollarAmountOnLastWin = CurrentDollarAmount;

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
                CurrentDollarAmount -= CurrentBet;
                if(SpinWhenLostAllMoney == 0 && CurrentDollarAmount < 0) {
                    SpinWhenLostAllMoney = CurrentNumSpins;
                }

                MaxAmountLost = MaxAmountLost < CurrentBet ? CurrentBet : MaxAmountLost;
                // CurrentBet *= BetMultiplier;

                CurrentNumConsecutiveLosses++;
                MaxNumConsecutiveLosses = MaxNumConsecutiveLosses < CurrentNumConsecutiveLosses ? CurrentNumConsecutiveLosses : MaxNumConsecutiveLosses;

                CurrentNumConsecutiveWins = 0;
            }

            CurrentBet = GetNextBetAmount(winOrLoss, CurrentBet, InitialDollarAmount, startDollarAmount);

            // TODO: Should refactor this becuase this needs to be copied to most sub-classes
            if(MaximumDollarAmount < CurrentDollarAmount) {
                MaximumDollarAmount = CurrentDollarAmount;
            }
            if(MinimumDollarAmount > CurrentDollarAmount) {
                MinimumDollarAmount = CurrentDollarAmount;
            }

            if(MaxBet < CurrentBet) {
                MaxBet = CurrentBet;
            }

            if (EnableCsvWriter) {
                await WriteCsvLineAsync(cell, startDollarAmount, startBet,winOrLoss,payout);
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
        // write the summary file now
        public override async Task GameCompleted() {
            var writer = new StreamWriter(Filepath, true);

            await writer.WriteLineAsync($"{GetMethodDisplayName()} ".PadRight(60));
            await writer.WriteLineAsync($"  initial bet:                       ${MinimumBet:N0}");
            await writer.WriteLineAsync($"  initial bankroll:                  ${InitialDollarAmount:N0}");
            await writer.WriteLineAsync($"  current bankroll:                  ${CurrentDollarAmount:N0}");
            await writer.WriteLineAsync($"  max bankroll:                      ${MaximumDollarAmount:N0}");
            await writer.WriteLineAsync($"  min bankroll:                      ${MinimumDollarAmount:N0}");
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
