using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Roulette {
    public class GameSettings {
        // array because some games have 0,0 + 00 and 0 + 00 + OTHER
        public string [] SpecialCells { get; set; } = new string[2] { "0", "00" };

    }
}
