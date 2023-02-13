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
namespace SayedHa.Blackjack.Shared {
    public interface IReporter : ILogger {
        bool EnableVerbose { get; set; }
        void Write(string output);
        void WriteLine(string output);
        void WriteLine();
        void WriteLine(string output, string prefix);
        void WriteVerbose(string output);
        void WriteVerboseLine(string output, bool includePrefix = true);
        void WriteVerboseLine();
    }

    public class ConsoleReporter : IReporter {
        public bool EnableVerbose { get; set; }
        public bool EnableConsoleLogger { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void WriteLine() {
            Console.WriteLine();
        }
        public void WriteLine(string output) {
            Console.WriteLine(output);
        }
        public void Write(string output) {
            Console.Write(output);
        }
        public void WriteLine(string output, string prefix) {
            Console.Write(prefix);
            Console.WriteLine(output);
        }
        public void WriteVerboseLine() {
            if (EnableVerbose) {
                WriteLine();
            }
        }
        public void WriteVerboseLine(string output, bool includePrefix = true) {
            if (EnableVerbose) {
                if (includePrefix) {
                    Write("verbose: ");
                }
                Write(output);
                WriteLine();
            }
        }
        public void WriteVerbose(string output) {
            if (EnableVerbose) {
                Write(output);
            }
        }

        public void LogLine(string message) {
            WriteVerboseLine(message);
        }

        public void Log(string message) {
            WriteVerbose(message);
        }

        public void CloseFileLogger() {
            // do nothing
        }
    }
    public class NullReporter : IReporter {
        public bool EnableVerbose { get; set; } = false;
        public bool EnableConsoleLogger { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void CloseFileLogger() {
            // do nothing
        }

        public void Log(string message) {
            // do nothing
        }

        public void LogLine(string message) {
            // do nothing
        }

        public void Write(string output) {
            // do nothing
        }

        public void WriteLine(string output) {
            // do nothing
        }

        public void WriteLine() {
            // do nothing
        }

        public void WriteLine(string output, string prefix) {
            // do nothing
        }

        public void WriteVerbose(string output) {
            // do nothing
        }

        public void WriteVerboseLine(string output, bool includePrefix = true) {
            // do nothing
        }

        public void WriteVerboseLine() {
            // do nothing
        }
    }
}
