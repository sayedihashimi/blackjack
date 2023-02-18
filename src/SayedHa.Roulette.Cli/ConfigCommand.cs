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
using Newtonsoft.Json;
using SayedHa.Blackjack.Shared.Roulette;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Versioning;
using SayedHa.Blackjack.Shared;

namespace SayedHa.Roulette.Cli {
    public class ConfigCommand : CommandBase {
        private readonly IReporter _reporter;
        private readonly IDefaultGameSettingsFile _defaultGameSettingsFile;

        public ConfigCommand(IReporter reporter, IDefaultGameSettingsFile defaultGameSettingsFile) {
            _reporter = reporter;
            _defaultGameSettingsFile = defaultGameSettingsFile;
        }

        public override Command CreateCommand() =>
            new Command(name: "config", description: "enables you to set config settings that will be persisted in a temp file") {
                // TODO: revisit how this is implemented, currently all settings are set
                //       better would be to only apply the changes to values that are passed in.
                CommandHandler.Create<ConfigCommandArgs>(async (config) => {
                    var settings = await _defaultGameSettingsFile.GetOrCreateGameSettingsFileAsync();
                    // only set properties which have a non-null value
                    if(config.rouletteType != null) {
                        if(!Enum.TryParse(config.rouletteType,out RouletteType rouletteTypeConverted)) {
                            throw new ArgumentOutOfRangeException(nameof(config.rouletteType));
                        }
                        settings.RouletteType = rouletteTypeConverted;
                    }
                    settings.InitialBankroll = config.initialBankroll ?? settings.InitialBankroll;
                    settings.NumberOfSpins = config.numSpins ?? settings.NumberOfSpins;
                    settings.StopWhenBankrupt = config.stopWhenBankrupt ?? settings.StopWhenBankrupt;
                    settings.EnableNumberDetails = config.enableReportNumberDetails ?? settings.EnableNumberDetails;
                    settings.EnableMartingale = config.enablePlayerMartingale ?? settings.EnableMartingale;
                    settings.EnableBondMartingale = config.enablePlayerBondMartingale ?? settings.EnableBondMartingale;
                    settings.EnableGreen = config.enablePlayerGreen ?? settings.EnableGreen;
                    settings.EnableConsoleLogger = config.enableConsoleLogger ?? settings.EnableConsoleLogger;
                    settings.EnableCsvFileOutput = config.enableCsvFileOutput ?? settings.EnableCsvFileOutput;

                    // save the settings now
                    await _defaultGameSettingsFile.SaveGameSettingsConfigAsync(settings);

                    var wasSettingPassed = config.initialBankroll == null && config.numSpins == null && config.stopWhenBankrupt == null && config.enableReportNumberDetails == null &&
                                            config.enablePlayerMartingale == null && config.enablePlayerBondMartingale == null && config.enablePlayerGreen == null &&
                                            config.enableConsoleLogger == null && config.enableCsvFileOutput == null;
                    if (wasSettingPassed) {
                        _reporter.WriteLine($"settings saved");
                    }

                    if (config.printSettings != null && (bool)config.printSettings) {
                        _reporter.WriteLine($"settings:\n{new GameSettingsFactory().GetJsonFor(settings)}");
                    }

                }),
                OptionRouletteType(),
                OptionInitialBankroll(),
                OptionNumberOfSpins(),
                OptionMinBet(),
                OptionMaxBet(),
                OptionStopWhenBankrupt(),
                EnableReportNumberDetails(),
                EnablePlayerMartingale(),
                EnablePlayerBondMartingale(),
                EnablePlayerGreen(),
                EnableConsoleLogger(),
                EnableCsvFileOutput(),
                OptionPrintSettings(),
                OptionVerbose(),
            };

        public Option OptionNumberOfSpins() =>
            new Option(new string[] { "--numSpins" }, "number of spins") {
                Argument = new Argument<int>(name:"numSpins", getDefaultValue:()=>10000)
            };
        public Option OptionInitialBankroll() =>
            new Option(new string[] { "--initialBankroll" }, "initial bankroll (amount of money to play)") {
                Argument = new Argument<long>(name: "initialBankroll", getDefaultValue: () => 3000)
            };
        // Option with a default value, and the value needs to be selected from a listed of allowed values.
        public Option OptionRouletteType() {
            var opt = new Option(new String[] { "--rouletteType" }) {
                Argument = new Argument<string>(name: "rouletteType")
            };

            opt.Argument.FromAmong(new string[] { "American", "European" });

            return opt;
        }
        public Option OptionMinBet() =>
            new Option(new string[] { "--minBet" }) {
                Argument = new Argument<int>(name: "minBet", description: "minimum bet for the table", getDefaultValue: () => 5)
            };

        public Option OptionMaxBet() =>
            new Option(new string[] { "--maxBet" }) {
                Argument = new Argument<int>(name:"maxBet",description:"maximum bet for the table")
            };
        public Option OptionStopWhenBankrupt() =>
            new Option(new string[] { "--stopWhenBankrupt" }) {
                Argument = new Argument<bool>(name: "stopWhenBankrupt", description: "stop when the bankroll gets to zero")
            };
        public Option EnableReportNumberDetails() =>
            new Option(new string[] { "--enableReportNumberDetails" }) {
                Argument = new Argument<bool>(name: "enableReportNumberDetails", description: "if true number details will be include when executed")
            };
        public Option EnablePlayerMartingale() =>
            new Option(new string[] { "--enablePlayerMartingale" }) {
                Argument = new Argument<bool>(name: "enablePlayerMartingale", description:"sets the default value if the martingale player will be used")
            };
        public Option EnablePlayerBondMartingale() =>
            new Option(new string[] { "--enablePlayerBondMartingale" }) {
                Argument = new Argument<bool>(name:"enablePlayerBondMartingale",description:"sets the default value if the bond margingale player will be used")
            };
        public Option EnablePlayerGreen() =>
            new Option(new string[] { "--enablePlayerGreen" }) {
                Argument = new Argument<bool>(name:"enablePlayerGreen",description:"sets the default value if the green player will be used")
            };

        public Option EnableConsoleLogger() =>
            new Option(new string[] { "--enableConsoleLogger" }) {
                Argument = new Argument<bool>(name: "enableConsoleLogger",description:"sets the default value if the console logger will be enabled")
            };
        public Option EnableCsvFileOutput() =>
            new Option(new string[] { "--enableCsvFileOutput" }) {
                Argument = new Argument<bool>(name:"enableCsvFileOutput",description:"sets the default value if the CSV file output will be enabled")
            };
        public Option OptionPrintSettings() =>
            new Option(new string[] { "--printSettings" }) {
                Argument = new Argument<bool>(name: "printSettings", description: "settings will be printed out")
            };

        internal class ConfigCommandArgs {
            public string rouletteType { get; set; }
            public long? initialBankroll { get; set; }
            public int? numSpins { get; set; }
            public int? minBet { get; set; }
            public int? maxBet { get; set; }
            public bool? stopWhenBankrupt { get; set; }
            public bool? enableReportNumberDetails { get; set; }
            public bool? enablePlayerMartingale { get; internal set; }
            public bool? enablePlayerBondMartingale { get; internal set; }
            public bool? enablePlayerGreen { get; internal set; }
            public bool? enableConsoleLogger { get; internal set; }
            public bool? enableCsvFileOutput { get; internal set; }
            public bool? printSettings { get; internal set; }
            public bool? verbose { get; set; }
        }
    }
}
