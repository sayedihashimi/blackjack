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
        public GreenMethodRecorder(string outputPath, string filenamePrefix, int minimumBet, long initialBankroll, bool enableCsvWriter) :
            base(outputPath,filenamePrefix, GameCellColor.Green, minimumBet, initialBankroll,enableCsvWriter) {
        }
        protected override long GetNextBetAmount(WinOrLoss spinResult, long currentBet, long intialBankroll, long currentBankroll) {
            long nextBetAmount = MinimumBet * ((int)Math.Ceiling(Math.Abs(((double)CurrentBankroll - BankrollOnLastWin) / (17 * (double)MinimumBet))));
            return spinResult switch {
                WinOrLoss.Win => MinimumBet,
                WinOrLoss.Loss => nextBetAmount < MaximumBet ? nextBetAmount : MaximumBet,
                _ => throw new ArgumentException(nameof(spinResult))
            };
        }
        public override string GetMethodDisplayName() => "Green only betting method";
        public override string GetMethodCompactName() => "green";

        protected override long GetPayoutForWin(long currentBet) => currentBet * 17;
        public override string GetFilepath() => Path.Combine(OutputPath, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}{GetMethodCompactName()}.txt" : $"{GetMethodCompactName()}.txt");
        public override string GetCsvFilepath() => Path.Combine(OutputPath, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}{GetMethodCompactName()}-details.csv" : $"{GetMethodCompactName()}-details");
    }

    public class GreenAgressiveMethodRecorder : GreenMethodRecorder {
        public GreenAgressiveMethodRecorder(string outputPath,string filenamePrefix, int minimumBet, long initialBankroll, bool enableCsvWriter) :
            base(outputPath, filenamePrefix, minimumBet, initialBankroll, enableCsvWriter) {
        }

        public override string GetMethodDisplayName() => "Green aggressive only betting method";
        public override string GetMethodCompactName() => "greenagro";


        protected override long GetNextBetAmount(WinOrLoss spinResult, long currentBet, long intialBankroll, long currentBankroll) {
            var betAmount = spinResult switch {
                WinOrLoss.Win => getWinBet(),
                WinOrLoss.Loss => getMultiplier() * MinimumBet * ((int)Math.Ceiling(Math.Abs(((double)CurrentBankroll - BankrollOnLastWin) / (17 * (double)MinimumBet)))),
                _ => throw new ArgumentException(nameof(spinResult))
            };

            long getWinBet() {
                if (CurrentBankroll < InitialBankroll) {
                    return MinBankroll;
                }

                // compute a bet multiplier based on how much has been gained
                int multiplier = (int)Math.Floor((double)CurrentBankroll / InitialBankroll);
                // for every $500 profit, increase bet by MinimumBet
                multiplier = (int)Math.Floor(((double)CurrentBankroll - InitialBankroll) / 1000);
                multiplier = multiplier > 0 ? multiplier : 1;
                var amt = CurrentBankroll - InitialBankroll > 0 ? MinimumBet * multiplier : MinimumBet;

                return amt;
            }

            int getMultiplier() {
                int multiplier = (int)Math.Floor(((double)CurrentBankroll - InitialBankroll) / 1000);
                multiplier = multiplier > 0 ? multiplier : 1;
                return multiplier;
            }

            betAmount = betAmount < MaximumBet ? betAmount : MaximumBet;

            return betAmount;
        }
    }

}
