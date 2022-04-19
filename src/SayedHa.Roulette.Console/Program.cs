using SayedHa.Blackjack.Shared.Roulette;
using System.Diagnostics;

int numSpins = 100;
bool enableCsvFileOutput = true;
bool enableSummaryFileOutput = true;
bool enableNumberDetails = true;
bool enableMartingale = true;
bool enableGreen = true;
bool enableBondMartingale = true;

int minimumBet = 20;
int initialBankroll = 5000;

var rouletteType = RouletteType.European;

if(args.Length == 1) {
    numSpins = int.Parse(args[0]);
}

RoulettePlayer player = new RoulettePlayer();
var settings = new GameSettings {
    EnableConsoleLogger = false,
    NumberOfSpins = numSpins
};

// need to change this to support custom if needed later
settings.SetRouletteType(rouletteType);

var recorders = new List<IGameRecorder>();
if (settings.EnableConsoleLogger) {
    recorders.Add(new ConsoleGameRecorder());
}

var timestamp = DateTime.Now.ToString("yyyy.MM.dd-hhmmss.ff");
var outputPath = $@"C:\temp\roulette";

var csvGameFilename = Path.Combine(outputPath, $"r-{numSpins}-{timestamp}.csv");
var statsFilename = Path.Combine(outputPath, $"r-{numSpins}-{timestamp}-stats.csv");
var csvSummaryFilename = Path.Combine(outputPath, $"r-{numSpins}-{timestamp}-summary.txt");
var csvRecorder = new CsvGameRecorder(outputPath, $"r-{numSpins}-{timestamp}-");


var csvWithStatsRecorder = new CsvWithStatsGameRecorder(outputPath, $"r-{numSpins}-{timestamp}-");
csvWithStatsRecorder.EnableWriteCsvFile = enableCsvFileOutput;

var numberDetailsFilename = Path.Combine(outputPath, $"r-{numSpins}-{timestamp}-number-details.txt");
var numberDetailsRecorder = new NumberDetailsRecorder(settings, numberDetailsFilename);

var martingaleRedFilepath = Path.Combine(outputPath, $"r-{numSpins}-{timestamp}-martingale-red.txt");
var martingaleRed = new MartingaleBettingRecorder(martingaleRedFilepath, null, GameCellColor.Red, 1, 1000);
var martingaleBlackFilepath = Path.Combine(outputPath, $"r-{numSpins}-{timestamp}-martingale-black.txt");
var martingaleBlackDetailsFilepath = Path.Combine(outputPath, $"r-{numSpins}-{timestamp}-martingale-black-details.csv");
var martingaleBlack = new MartingaleBettingRecorder(martingaleBlackFilepath, martingaleBlackDetailsFilepath, GameCellColor.Black, 1, 1000);
martingaleBlack.EnableCsvWriter = true;

var greenFilepath = Path.Combine(outputPath, $"r-{numSpins}-{timestamp}-greens.txt");
var greenCsvFilepath = Path.Combine(outputPath, $"r-{numSpins}-{timestamp}-greens.csv");
var greenRecorder = new GreenMethodRecorder(greenFilepath, greenCsvFilepath, minimumBet, initialBankroll);

var greenAgroFilepath = Path.Combine(outputPath, $"r-{numSpins}-{timestamp}-greens-agro.txt");
var greenAgroCsvFilepath = Path.Combine(outputPath, $"r-{numSpins}-{timestamp}-greens-agro.csv");
var greenAgroRecorder = new GreenAgressiveMethodRecorder(greenAgroFilepath, greenAgroCsvFilepath, minimumBet, initialBankroll);


var bondFilepath = Path.Combine(outputPath, $"r-{numSpins}-{timestamp}-bondmartingale.txt");
var bondDetailsFilepath = Path.Combine(outputPath, $"r-{numSpins}-{timestamp}-bondmartingale-details.csv");
var bondMartingale = new BondMartingaleBettingRecorder(bondFilepath,bondDetailsFilepath,minimumBet, initialBankroll);

if (enableCsvFileOutput) {
    recorders.Add(csvRecorder);
}
if (enableNumberDetails) {
    recorders.Add(numberDetailsRecorder);
}
if (enableMartingale) {
    recorders.Add(martingaleBlack);
    recorders.Add(martingaleRed);
}
if (enableGreen) {
    recorders.Add(greenRecorder);
    recorders.Add(greenAgroRecorder);
}
if (enableBondMartingale) {
    recorders.Add(bondMartingale);
}

// csv with stats always needs to be added for the summary
recorders.Add(csvWithStatsRecorder);

var watch = Stopwatch.StartNew();
var board = Board.BuildBoard(settings);
await player.PlayAsync(board, settings, recorders);
watch.Stop();

foreach (var recorder in recorders) {
    await recorder.GameCompleted();
    recorder.Dispose();
}

Console.WriteLine($"num spins: {settings.NumberOfSpins:N0}\ntime: {watch.Elapsed.TotalSeconds}\nfilenames: '{csvGameFilename}','{statsFilename}'");