// This file is part of SayedHa.Blackjack.
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
