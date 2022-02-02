using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared {
    public interface ILogger {
        void LogLine(string message);
    }

    public class Logger : ILogger {
        public void LogLine(string message) {
            Console.WriteLine(message);
        }
    }
}
