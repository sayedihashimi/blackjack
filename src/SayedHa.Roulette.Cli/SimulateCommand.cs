using System.CommandLine;
using System.CommandLine.Invocation;

namespace SayedHa.Roulette.Cli {
    public class SimulateCommand : CommandBase {
        private IReporter _reporter;
        public SimulateCommand(IReporter reporter) {
            _reporter = reporter;
        }
        public override Command CreateCommand() =>
            new Command(name: "simulate", description: "roulette simulator") {
                CommandHandler.Create<string, bool>(async (paramname, verbose) => {
                    _reporter.EnableVerbose = verbose;
                    _reporter.WriteLine(VsAscii);
                    _reporter.WriteLine(string.Empty);
                    _reporter.WriteLine($"paramname: {paramname}");
                    _reporter.WriteLine($"verbose: {verbose}");
                    _reporter.WriteVerbose("verbose message here");
                    // added here to avoid async/await warning
                    await Task.Delay(1000);
                }),
                OptionPackages(),
                OptionVerbose(),
            };
        protected Option OptionPackages() =>
            new Option(new string[] { "--paramname" }, "TODO: update param description") {
                Argument = new Argument<string>(name: "paramname")
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
