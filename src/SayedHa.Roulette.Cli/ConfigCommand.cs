using SayedHa.Blackjack.Shared.Roulette;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Roulette.Cli {
    public class ConfigCommand : CommandBase {

        private IReporter _reporter;
        public ConfigCommand(IReporter reporter) {
            _reporter = reporter;
        }

        public override Command CreateCommand() =>
            new Command(name: "config", description: "enables you to set config settings that will be persisted in a temp file") {


                // TODO: revisit how this is implemented, currently all settings are set
                //       better would be to only apply the changes to values that are passed in.
                CommandHandler.Create<ConfigCommandArgs>((config) => {
                //CommandHandler.Create<string,long,int,int,int,bool,bool>((rouletteType,initialBankroll,numSpins,minBet,maxBet,stopWhenBankrupt,verbose) => {
                    if(!Enum.TryParse(config.rouletteType,out RouletteType rouletteTypeConverted)) {
                        throw new ArgumentOutOfRangeException(nameof(config.rouletteType));
                    }
                    var settings = new GameSettings() {
                        RouletteType = rouletteTypeConverted,
                        InitialBankroll = config.initialBankroll,
                        NumberOfSpins = config.numSpins,
                        StopWhenBankrupt = config.stopWhenBankrupt,
                        EnableNumberDetails = config.enableReportNumberDetails,
                        EnableMartingale = config.enablePlayerMartingale,
                        EnableBondMartingale = config.enablePlayerBondMartingale,
                        EnableGreen = config.enablePlayerGreen,
                        EnableConsoleLogger = config.enableConsoleLogger,
                        EnableCsvFileOutput = config.enableCsvFileOutput
                    };

                    string json = new GameSettingsFactory().GetJsonFor(settings);

                    Console.WriteLine($"rouletteType: {config.rouletteType}");
                    Console.WriteLine($"settings:\n{json}");
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
                Argument = new Argument<int>(name:"maxBet",description:"maximum bet for the table", getDefaultValue:()=>int.MaxValue)
            };
        public Option OptionStopWhenBankrupt() =>
            new Option(new string[] { "--stopWhenBankrupt" }) {
                Argument = new Argument<bool>(name: "stopWhenBankrupt", description: "stop when the bankroll gets to zero", getDefaultValue: () => true)
            };
        public Option EnableReportNumberDetails() =>
            new Option(new string[] { "--enableReportNumberDetails" }) {
                Argument = new Argument<bool>(name: "enableReportNumberDetails", description: "if true number details will be include when executed", getDefaultValue: () => false)
            };
        public Option EnablePlayerMartingale() =>
            new Option(new string[] { "--enablePlayerMartingale" }) {
                Argument = new Argument<bool>(name: "enablePlayerMartingale", description:"sets the default value if the martingale player will be used",getDefaultValue:()=>false)
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

        internal class ConfigCommandArgs {
            public string rouletteType { get; set; }
            public long initialBankroll { get; set; }
            public int numSpins { get; set; }
            public int minBet { get; set; }
            public int maxBet { get; set; }
            public bool stopWhenBankrupt { get; set; }
            public bool enableReportNumberDetails { get; set; }
            public bool enablePlayerMartingale { get; internal set; }
            public bool enablePlayerBondMartingale { get; internal set; }
            public bool enablePlayerGreen { get; internal set; }
            public bool enableConsoleLogger { get; internal set; }
            public bool enableCsvFileOutput { get; internal set; }
            public bool verbose { get; set; }
        }
    }
}
