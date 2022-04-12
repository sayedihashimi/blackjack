using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SayedHa.Blackjack.Shared.Extensions;

namespace SayedHa.Blackjack.Shared.Roulette {
    public interface IPlayer {
        public void PlaceBets();
        public void SpinWheel();
        public void IdentifyWinningAndLosingBets();
    }

    public class GameCell {
        private GameCell() { }
        /// <summary>
        /// When creating a green 
        /// </summary>
        /// <param name="value">ignored if color is Green</param>
        public GameCell(int id, int value, GameCellColor color) {
            Id = id;
            Value = value;
            Color = color;

            if (color is GameCellColor.Green) {
                Value = int.MinValue;
            }
        }
        /// <summary>
        /// Use this to create the Green cells
        /// </summary>
        public GameCell(int id, GameCellColor color, string text) {
            Text = text;
        }

        /// <summary>
        /// This is a unique ID, it will be used to index the array of all the cells
        /// Doesn't need to be any special value, just unqiue to each cell
        /// </summary>
        public int Id { get; init; }
        /// <summary>
        /// Numeric value 1 - 36.
        /// If the cell is Green this value will be set to int.MinValue
        /// </summary>
        public int Value { get; init; }
        public GameCellColor Color { get; init; }
        public string? Text { get; init; }

        public override string ToString() {
            return $"{Text} {Color}";
        }
        private List<GameCellGroup> _groups;
        public List<GameCellGroup> CellGroups {
            get {
                if (_groups is null) {
                    _groups = GetGroups(this);
                }
                return _groups;
            }
        }

        public bool IsInGroup(GameCellGroup group) =>
            CellGroups.Contains(group);


        protected List<GameCellGroup> GetGroups(GameCell cell) {
            var result = new List<GameCellGroup>();
            if (cell.Value == int.MaxValue || cell.Value == int.MinValue) {
                return result;
            }

            // first twelve
            if (cell.Value >= 1 && cell.Value <= 12)
                result.Add(GameCellGroup.First12);
            if (cell.Value >= 13 && cell.Value <= 24)
                result.Add(GameCellGroup.Second12);
            if (cell.Value >= 25 && cell.Value <= 36)
                result.Add(GameCellGroup.Third12);

            // 1st and 2nd 18
            if (cell.Value >= 1 && cell.Value <= 18)
                result.Add(GameCellGroup.First18);
            if (cell.Value >= 19 && cell.Value <= 36)
                result.Add(GameCellGroup.Second18);

            // columns
            switch (cell.Value % 3) {
                case 1:
                    result.Add(GameCellGroup.FirstColumn);
                    break;
                case 2:
                    result.Add(GameCellGroup.SecondColumn);
                    break;
                case 0:
                    result.Add(GameCellGroup.ThirdColumn);
                    break;
                default: throw new ArgumentOutOfRangeException(cell.Value.ToString());

            }

            return result;
        }
        public class GameCellFactory {
            public GameCell NewGreenCell(int id, string text) => new GameCell {
                Id = id,
                Value = int.MinValue,
                Text = text,
                Color = GameCellColor.Green
            };
            public GameCell NewCell(int id, int value, GameCellColor color) => new GameCell {
                Id = id,
                Value = value,
                Text = value.ToString(),
                Color = color
            };
        }
    }

    public enum GameCellColor {
        Red,
        Black,
        Green
    }
    public enum GameCellGroup {
        First12,
        Second12,
        Third12,
        First18,
        Second18,
        FirstColumn,
        SecondColumn,
        ThirdColumn
    }

    //public class GameRoll {
    //    public GameCell CellHit { get; set; }
    //}
    public class GameRecord {
        public LinkedList<GameCell>? CellsHit { get; set; } = new LinkedList<GameCell>();
    }

    public class Board {
        public List<GameCell>? Cells { get; set; }
    }

    /// <summary>
    /// GameRecorders are responsbile for capturing the results of the game.
    /// If the game results are to be persisted anywhere, the game recorder
    /// should be the one persisting the data as well.
    /// </summary>
    public interface IGameRecorder : IDisposable {
        public Task RecordSpinAsync(GameCell cell);
    }

    public abstract class GameRecorderBase : IGameRecorder {
        protected virtual void Dispose(bool disposing) {

        }
        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public abstract Task RecordSpinAsync(GameCell cell);
    }

    public class ConsoleGameRecorder : GameRecorderBase {
        public bool Enabled { get; set; } = true;

        public override async Task RecordSpinAsync(GameCell cell) {
            RecordSpin(cell);
        }
        public void RecordSpin(GameCell cell) {
            if (Enabled) {
                Console.WriteLine(cell.ToString());
            }
        }
    }
    public class CsvGameRecorder : IGameRecorder {
        public CsvGameRecorder(string csvFilepath) {
            Debug.Assert(!string.IsNullOrEmpty(csvFilepath));
            CsvFilePath = csvFilepath;
        }
        protected string CsvFilePath { get; init; }
        protected StreamWriter? StreamWriter { get; set; }
        protected bool isInitalized = false;
        private bool disposedValue;

        protected async Task InitalizeAsync() {
            isInitalized = true;
            StreamWriter = new StreamWriter(CsvFilePath, false);
            await WriteHeaderAsync();
        }
        protected virtual async Task WriteHeaderAsync() {
            await StreamWriter!.WriteLineAsync("text,color");
        }
        protected virtual async Task WriteLineForAsync(GameCell cell) {
            if (!isInitalized) {
                await InitalizeAsync();
            }
            await StreamWriter!.WriteLineAsync($"{cell.Text},{cell.Color}");
        }
        public virtual async Task RecordSpinAsync(GameCell cell) {
            await WriteLineForAsync(cell);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing && StreamWriter is not null) {
                    StreamWriter.Flush();
                    StreamWriter.Close();
                }
                disposedValue = true;
            }
        }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    // some stats that we want to gather
    //  spins since last red/black/green
    public class CsvWithStatsGameRecorder : CsvGameRecorder {
        public CsvWithStatsGameRecorder(string csvFilePath) : base(csvFilePath) {
            groupDictionarySpinsSince = new Dictionary<GameCellGroup, int>();
            groupDictionaryConsecutive = new Dictionary<GameCellGroup, int>();

            foreach (GameCellGroup group in (GameCellGroup[])Enum.GetValues(typeof(GameCellGroup))) {
                groupDictionarySpinsSince.Add(group, 0);
                groupDictionaryConsecutive.Add(group, 0);
            }

            groupOuputOrder = new List<GameCellGroup> {
                GameCellGroup.First12,
                GameCellGroup.Second12,
                GameCellGroup.Third12,
                GameCellGroup.First18,
                GameCellGroup.Second18,
                GameCellGroup.FirstColumn,
                GameCellGroup.SecondColumn,
                GameCellGroup.ThirdColumn
            };
        }

        protected int SpinsSinceLastBlack { get; set; }
        protected int SpinsSinceLastRed { get; set; }
        protected int SpinsSinceLastGreen { get; set; }

        protected int ConsecutiveBlack { get; set; }
        protected int ConsecutiveRed { get; set; }
        protected int ConsecutiveGreen { get; set; }

        Dictionary<GameCellGroup, int> groupDictionarySpinsSince { get; init; }
        Dictionary<GameCellGroup, int> groupDictionaryConsecutive { get; init; }
        List<GameCellGroup> groupOuputOrder { get; init;
        }
        protected override async Task WriteHeaderAsync() {
            await StreamWriter!.WriteAsync("text,color,sinceLastRed,sinceLastBlack,sinceLastGreen,consecRed,consecBlack,consecGreen,groups,");
            for (int i = 0; i < groupOuputOrder.Count; i++) {
                var group = groupOuputOrder[i];
                await StreamWriter!.WriteAsync($"sinceLast{group},consec{group}");
                if (i < groupOuputOrder.Count - 1) {
                    await StreamWriter.WriteAsync(",");
                }
            }
            await StreamWriter!.WriteAsync("\n");
        }
        protected override async Task WriteLineForAsync(GameCell cell) {
            if (!isInitalized) {
                await InitalizeAsync();
            }
            var groupStr = string.Join("|", cell.CellGroups);
            if (string.IsNullOrEmpty(groupStr)) {
                groupStr = "(none)";
            }

            var groupNames = ((GameCellGroup[])Enum.GetValues(typeof(GameCellGroup))).ToList();
            // sort names so that they are consistent in the 
            groupNames.Sort();

            await StreamWriter!.WriteAsync($"{cell.Text},{cell.Color},{SpinsSinceLastRed},{SpinsSinceLastBlack},{SpinsSinceLastGreen},{ConsecutiveRed},{ConsecutiveBlack},{ConsecutiveGreen},{groupStr},");
            // write out the groups
            for (int i = 0; i < groupOuputOrder.Count; i++) {
                var group = groupOuputOrder[i];
                await StreamWriter!.WriteAsync($"{groupDictionarySpinsSince[group]},{groupDictionaryConsecutive[group]}");
                if (i < groupOuputOrder.Count - 1) {
                    await StreamWriter.WriteAsync(",");
                }
            }
            await StreamWriter!.WriteAsync("\n");
        }
        public override async Task RecordSpinAsync(GameCell cell) {
            switch (cell.Color) {
                case GameCellColor.Black:
                    SpinsSinceLastBlack = 0;
                    SpinsSinceLastGreen++;
                    SpinsSinceLastRed++;
                    ConsecutiveBlack++;
                    ConsecutiveRed = 0;
                    ConsecutiveGreen = 0;
                    break;
                case GameCellColor.Red:
                    SpinsSinceLastRed = 0;
                    SpinsSinceLastBlack++;
                    SpinsSinceLastGreen++;
                    ConsecutiveRed++;
                    ConsecutiveBlack = 0;
                    ConsecutiveGreen = 0;
                    break;
                case GameCellColor.Green:
                    SpinsSinceLastGreen = 0;
                    SpinsSinceLastRed++;
                    SpinsSinceLastGreen++;
                    ConsecutiveGreen++;
                    ConsecutiveBlack = 0;
                    ConsecutiveRed = 0;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(cell.Color));
            }

            // group analysis here
            foreach (GameCellGroup group in (GameCellGroup[])Enum.GetValues(typeof(GameCellGroup))) {
                // check to see if the current cell is in this group
                if (cell.IsInGroup(group)) {
                    groupDictionarySpinsSince[group] = 0;
                    groupDictionaryConsecutive[group]++;
                }
                else {
                    groupDictionarySpinsSince[group]++;
                    groupDictionaryConsecutive[group] = 0;
                }
            }

            await WriteLineForAsync(cell);
        }
    }

    public class RoulettePlayer {
        public async Task PlayAsync(GameSettings settings, List<IGameRecorder> recorders) {
            // first build the board
            var board = BuildBoard(settings);
            var numCells = board.Cells!.Count;

            var numberOfSpins = settings.NumberOfSpins;
            for (int i = 0; i < numberOfSpins; i++) {
                var spinValue = GetRandomCellFrom(board);
                foreach (var recorder in recorders) {
                    await recorder.RecordSpinAsync(spinValue);
                }
            }
        }
        private Random _random = new Random();
        // TODO: does this need to be improved?
        protected GameCell GetRandomCellFrom(Board board) =>
            board.Cells![GetRandomNum(board.Cells!.Count)];
        public int GetRandomNum(int max) =>
            _random.Next(max);

        protected internal Board BuildBoard(GameSettings settings) {
            var factory = new GameCell.GameCellFactory();
            var cells = new List<GameCell>();
            // build a list of GameCells starting with 1 - 36 alternating red and black, then add special cells

            // 1 is red
            var currentColor = GameCellColor.Red;
            for (int i = 1; i <= 36; i++) {
                cells.Add(factory.NewCell(i, i, currentColor));
                currentColor = currentColor == GameCellColor.Black ? GameCellColor.Red : GameCellColor.Black;
            }

            int currentId = 37;
            // add special cells as specified in settings
            if (settings is not null && settings.SpecialCells is not null && settings.SpecialCells.Length > 0) {
                foreach (var special in settings.SpecialCells) {
                    cells.Add(factory.NewGreenCell(currentId++, special));
                }
            }

            return new Board { Cells = cells };
        }
    }
}
