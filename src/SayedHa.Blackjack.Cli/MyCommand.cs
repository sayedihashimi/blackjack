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
using System.CommandLine;
using System.CommandLine.Invocation;

namespace SayedHa.Blackjack.Cli {
    public class analyze : CommandBase {
        private IReporter _reporter;
        public analyze(IReporter reporter) {
            _reporter = reporter;
        }
        public override Command CreateCommand() =>
            new Command(name: "analyze", description: "will perform analysis with the specified parameters") {
                CommandHandler.Create<string, bool>(async (numGamesToPlay, verbose) => {
                    _reporter.EnableVerbose = verbose;
                    _reporter.WriteLine(VsAscii);
                    _reporter.WriteLine(string.Empty);
                    _reporter.WriteLine($"numGamesToPlay: {numGamesToPlay}");
                    _reporter.WriteLine($"verbose: {verbose}");
                    _reporter.WriteVerbose("verbose message here");
                    // added here to avoid async/await warning
                    await Task.Delay(1000);
                }),
                OptionPackages(),
                OptionVerbose(),
            };
        protected Option OptionPackages() =>
            new Option(new string[] { "--numGamesToPlay" }, "number of games to play") {
                Argument = new Argument<string>(name: "numGamesToPlay")
            };

        private string VsAscii = @"                                                                                
                                                                                
                                                    ******(*                    
                                                  ********/%%%#,                
                                                **********/%%%%%%%(.            
                                             .************/%%%%%%%%%%#/         
               ,(((((/                     .**************/%%%%%%%%%%%%%%#*     
            *(((((((((((*                ,****************/%%%%%%%%%%%%%%%%%#   
         /(((((((((((((((((.           ,******************/%%%%%%%%%%%%%%%%%#   
     ,(((((((((((((((((((((((*       *********************/%%%%%%%%%%%%%%%%%#   
   /****,*((((((((((((((((((((((   ***********************/%%%%%%%%%%%%%%%%%#   
   /********((((((((((((((((((((((/***********************/%%%%%%%%%%%%%%%%%#   
   /*********,(((((((((((((((((((((((********************,*##################   
   /***********,/((((((((((((((((((((((/***************   *##################   
   /************. /(((((((((((((((((((((((**********,     *##################   
   /************.   /((((((((((((((((((((((((*****        *##################   
   /************.     *((((((((((((((((((((((((/          *##################   
   /************.  .*****(((((((((((((((((((((((((*       *##################   
   /************. *********(((((((((((((((((((((((((/     *##################   
   /*************************/(((((((((((((((((((((((((*  *##################   
   /*********,*****************/(((((((((((((((((((((((((/*##################   
   /*****************************/(((((((((((((((((((((((//##################   
   /***************************,   *(((((((((((((((((((((//##################   
      ,**********************.       *(((((((((((((((((((//##################   
         .****************,            .(((((((((((((((((//##################   
             ***********                 .((((((((((((((///##################   
                ,****.                     ./(((((((((((///###############*     
                                              /(((((((((///###########/         
                                                *(((((((///#######(.            
                                                  *((((((//####,                
                                                    .((((/(/                    
                                                                                ";
    }
}
