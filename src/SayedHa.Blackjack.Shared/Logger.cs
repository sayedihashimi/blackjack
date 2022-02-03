using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared {
    public interface ILogger {
        void LogLine(string message);
        bool LoggerEnabled { get; set; }
    }

    public class Logger : ILogger {
        public Logger() { }

        public Logger(bool loggerEnabled) {
            LoggerEnabled = loggerEnabled;
        }

        public void LogLine(string message) {
            if (LoggerEnabled) {
                Console.WriteLine(message);
            }            
        }
        public bool LoggerEnabled { get; set; } = true;
    }
    public class NullLogger : ILogger {
        public bool LoggerEnabled { get; set; } = false;

        public void LogLine(string message) {
            // do nothing
        }
    }
}
