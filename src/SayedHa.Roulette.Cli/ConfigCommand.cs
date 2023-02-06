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
        public ConfigCommand(IReporter reporter) {
            _reporter = reporter;
        }

        public override Command CreateCommand() =>
            new Command(name: "config", description: "enables you to set config settings that will be persisted in a temp file") {
                // TODO: revisit how this is implemented, currently all settings are set
                //       better would be to only apply the changes to values that are passed in.
                CommandHandler.Create<ConfigCommandArgs>(async (config) => {
                    var settings = await GetOrCreateExistingConfigSettingsFileAsync();
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
                    await SaveGameSettingsConfigAsync(settings);

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

        internal protected async Task<GameSettings> GetOrCreateExistingConfigSettingsFileAsync() {
            string settingsFilepath = GetPathToSettingsFile();
            Debug.Assert(!string.IsNullOrEmpty(settingsFilepath));

            var settings = new GameSettings();
            if (File.Exists(settingsFilepath)) {
                // var contents = await File.ReadAllTextAsync(settingsFilepath);

                try {
                    var gsf = new GameSettingsFactory();
                    settings = await gsf.ReadFromJsonFileAsync(settingsFilepath);
                }
                catch (JsonException je) {
                    _reporter.WriteLine($"unable to read settings from file, loading default settings. filepath='{settingsFilepath}'.\njson Error:{je}");
                    settings = new GameSettings();
                }
                catch (Exception ex) {
                    _reporter.WriteLine($"unable to read settings from file, loading default settings. filepath='{settingsFilepath}'.\nError:{ex}");
                    settings = new GameSettings();
                }

                //try {
                //    settings = JsonConvert.DeserializeObject<GameSettings>(contents);
                //}
                //catch (JsonException je) {
                //    Console.WriteLine($"unable to read settings from file, loading default settings. filepath='{settingsFilepath}'.\njson Error:{je.ToString()}");
                //    settings = new GameSettings();
                //}
                //catch (Exception ex) {
                //    Console.WriteLine($"unable to read settings from file, loading default settings. filepath='{settingsFilepath}'.\nError:{ex.ToString()}");
                //    settings = new GameSettings();
                //}
            }

            return settings;
        }
        internal protected async Task SaveGameSettingsConfigAsync(GameSettings configSettings) {
            Debug.Assert(configSettings != null);
            // TODO: This should be passed in
            var gsf = new GameSettingsFactory();
            await gsf.SaveSettingsToJsonFileAsync(GetPathToSettingsFile(), configSettings);
        }

        internal protected string GetPathToSettingsFile() {
            // should work xplat, see: https://developers.redhat.com/blog/2018/11/07/dotnet-special-folder-api-linux#environment_getfolderpath
            string appdata = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify), "sayedha.roulette");
            // ensure that the directory exists
            Directory.CreateDirectory(appdata);

            return Path.Combine(appdata, "roulette.settings.json");
        }

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
                Argument = new Argument<string>(name: "rouletteType", getDefaultValue: () => "American")
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
