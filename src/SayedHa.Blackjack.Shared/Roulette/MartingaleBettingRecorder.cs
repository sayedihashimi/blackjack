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
        public MartingaleBettingRecorder(string filepath, GameCellColor selectedColor, int initialBet, long initialDollarAmount) {
            InitialBet = initialBet;
            CurrentBet = InitialBet;
            SelectedColor = selectedColor;
            MaxBet = 0;
            Filepath = filepath;
            InitialDollarAmount = initialDollarAmount;
            CurrentDollarAmount = InitialDollarAmount;
        }
        private bool disposedValue;
        protected string Filepath { get; set; }
        protected int InitialBet { get; init; } = 1;
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
        protected GameCellColor SelectedColor { get; init; }
        protected long SpinWhenLostAllMoney { get; set; }
        protected long CurrentNumSpins { get; set; }
        public override async Task RecordSpinAsync(GameCell cell) {
            CurrentNumSpins++;
            // if you win, repeat the bet
            // if you lose, double the bet
            // once you win, reset bet amount to the initial bet amount

            if (cell.Color == SelectedColor) {
                // won the bet
                // reset the bet amount back to the initial bet
                CurrentDollarAmount += CurrentBet;

                MaxAmountWon = MaxAmountWon > CurrentBet ? MaxAmountWon : CurrentBet;

                CurrentBet = InitialBet;

                CurrentNumConsecutiveWins++;
                MaxNumConsecutiveWins = MaxNumConsecutiveWins < CurrentNumConsecutiveWins ? CurrentNumConsecutiveWins : MaxNumConsecutiveWins;

                CurrentNumConsecutiveLosses = 0;
            }
            else {
                // lost the bet
                // double the bet
                CurrentDollarAmount -= CurrentBet;
                if(SpinWhenLostAllMoney == 0 && CurrentDollarAmount < 0) {
                    SpinWhenLostAllMoney = CurrentNumSpins;
                }

                MaxAmountLost = MaxAmountLost < CurrentBet ? CurrentBet : MaxAmountLost;
                CurrentBet *= BetMultiplier;

                CurrentNumConsecutiveLosses++;
                MaxNumConsecutiveLosses = MaxNumConsecutiveLosses < CurrentNumConsecutiveLosses ? CurrentNumConsecutiveLosses : MaxNumConsecutiveLosses;

                CurrentNumConsecutiveWins = 0;
            }

            if(MaxBet < CurrentBet) {
                MaxBet = CurrentBet;
            }
        }
        // write the summary file now
        public override async Task GameCompleted() {
            var writer = new StreamWriter(Filepath, true);

            await writer.WriteLineAsync($"Martingale betting method ".PadRight(60));
            await writer.WriteLineAsync($"  initial bet:                       ${InitialBet:N0}");
            await writer.WriteLineAsync($"  initial bankroll:                  ${InitialDollarAmount:N0}");
            await writer.WriteLineAsync($"  current bankroll:                  ${CurrentDollarAmount:N0}");
            await writer.WriteLineAsync($"  max bet won:                       ${MaxAmountWon:N0}");
            await writer.WriteLineAsync($"  max bet lost:                      ${MaxAmountLost:N0}");
            await writer.WriteLineAsync($"  maximum bet played:                ${MaxBet:N0}");
            await writer.WriteLineAsync($"  maximum num consecutive wins:      {MaxNumConsecutiveWins:N0}");
            await writer.WriteLineAsync($"  maximum num consecutive losses:    {MaxNumConsecutiveLosses:N0}");
            await writer.WriteLineAsync($"  spin when went bankrupt:           {SpinWhenLostAllMoney:N0}");

            await writer.FlushAsync();
        }

    }
}
