using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Roulette {
    public class GameSettings {
        // array because some games have 0,0 + 00 and 0 + 00 + OTHER
        public string[] SpecialCells { get; protected set; } = new string[2] { "0", "00" };

        public int NumberOfSpins { get; set; } = 100;
        public bool EnableConsoleLogger { get; set; } = true;
        public RouletteType RouletteType { get; protected set; }
        public bool StopWhenBankrupt { get; set; } = true;

        public void SetRouletteType(RouletteType rouletteType) {
            RouletteType = rouletteType;
            switch (rouletteType) {
                case RouletteType.American:
                    SpecialCells = new string[2] { "0", "00" };
                    break;
                case RouletteType.European:
                    SpecialCells = new string[1] { "0" };
                    break;
                case RouletteType.Custom:
                    throw new ArgumentOutOfRangeException($"For RouletteType.Custom call SetCustomRouletteType");
                default:
                    throw new ArgumentOutOfRangeException(nameof(rouletteType));
            }
        }
        public void SetCustomRouletteType(string[] specialCells) {
            RouletteType = RouletteType.Custom;
            SpecialCells = specialCells;
        }
    }
}
