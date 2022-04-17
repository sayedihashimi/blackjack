﻿namespace SayedHa.Blackjack.Shared.Roulette {

    public class EnumHelper {
        private EnumHelper() {
            _allGameCellGroup = ((GameCellGroup[])Enum.GetValues(typeof(GameCellGroup))).ToArray();
        }

        public GameCellGroup[] GetAllGameCellGroup() => _allGameCellGroup;

        private static EnumHelper _instance = new EnumHelper();
        private readonly GameCellGroup[] _allGameCellGroup;

        public static EnumHelper GetHelper() => _instance;
    }

    // some stats that we want to gather
    //  spins since last red/black/green
    public class CsvWithStatsGameRecorder : CsvGameRecorder {
        public CsvWithStatsGameRecorder(string csvFilePath) : base(csvFilePath) {
            groupSpinsSince = new Dictionary<GameCellGroup, int>();
            groupConsecutive = new Dictionary<GameCellGroup, int>();
            groupSpinsSinceSum = new Dictionary<GameCellGroup, long>();
            groupConsecutiveSum = new Dictionary<GameCellGroup, long>();
            maxSpinsSince = new Dictionary<GameCellGroup, int>();
            maxConsecutive = new Dictionary<GameCellGroup, int>();

            foreach (GameCellGroup group in EnumHelper.GetHelper().GetAllGameCellGroup()) {
                groupSpinsSince.Add(group, 0);
                groupConsecutive.Add(group, 0);
                groupSpinsSinceSum.Add(group, 0);
                groupConsecutiveSum.Add(group, 0);
                maxSpinsSince.Add(group, 0);
                maxConsecutive.Add(group, 0);
            }

            groupOuputOrder = new List<GameCellGroup> {
                GameCellGroup.Black,
                GameCellGroup.Red,
                GameCellGroup.Green,
                GameCellGroup.First12,
                GameCellGroup.Second12,
                GameCellGroup.Third12,
                GameCellGroup.FirstColumn,
                GameCellGroup.SecondColumn,
                GameCellGroup.ThirdColumn,
                GameCellGroup.First18,
                GameCellGroup.Second18
            };
        }

        protected int SpinsSinceLastBlack { get; set; }
        protected int SpinsSinceLastRed { get; set; }
        protected int SpinsSinceLastGreen { get; set; }

        protected int ConsecutiveBlack { get; set; }
        protected int ConsecutiveRed { get; set; }
        protected int ConsecutiveGreen { get; set; }

        Dictionary<GameCellGroup, int> groupSpinsSince { get; init; }
        Dictionary<GameCellGroup, int> groupConsecutive { get; init; }
        Dictionary<GameCellGroup, long> groupSpinsSinceSum { get; init; }
        Dictionary<GameCellGroup, long> groupConsecutiveSum { get; init; }
        Dictionary<GameCellGroup, int> maxSpinsSince { get; init; }
        Dictionary<GameCellGroup, int> maxConsecutive { get; init; }

        public bool EnableWriteCsvFile { get; set; } = true;

        List<GameCellGroup> groupOuputOrder { get; init; }
        protected override async Task WriteHeaderAsync() {
            if (!EnableWriteCsvFile) {
                return;
            }

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
            if (!EnableWriteCsvFile) {
                return;
            }

            if (!isInitalized) {
                await InitalizeAsync();
            }
            var groupStr = string.Join("|", cell.CellGroups);
            if (string.IsNullOrEmpty(groupStr)) {
                groupStr = "(none)";
            }

            await StreamWriter!.WriteAsync($"{cell.Text},{cell.Color},{SpinsSinceLastRed},{SpinsSinceLastBlack},{SpinsSinceLastGreen},{ConsecutiveRed},{ConsecutiveBlack},{ConsecutiveGreen},{groupStr},");
            // write out the groups
            for (int i = 0; i < groupOuputOrder.Count; i++) {
                var group = groupOuputOrder[i];
                await StreamWriter!.WriteAsync($"{groupSpinsSince[group]},{groupConsecutive[group]}");
                if (i < groupOuputOrder.Count - 1) {
                    await StreamWriter.WriteAsync(",");
                }
            }
            await StreamWriter!.WriteAsync("\n");
        }
        private long _numberOfSpins = 0;
        private object _spinsLock = new object();
        public override async Task RecordSpinAsync(GameCell cell) {
            _numberOfSpins++;
            lock (_spinsLock) {
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
            }

            // group analysis here
            foreach (GameCellGroup group in EnumHelper.GetHelper().GetAllGameCellGroup()) {
                // check to see if the current cell is in this group
                if (cell.IsInGroup(group)) {
                    lock (_spinsLock) {
                        groupSpinsSince[group] = 0;
                        groupConsecutive[group]++;
                    }
                }
                else {
                    lock (_spinsLock) {
                        groupSpinsSince[group]++;
                        groupConsecutive[group] = 0;
                    }
                }

                groupSpinsSinceSum[group] += groupSpinsSince[group];
                groupConsecutiveSum[group] += groupConsecutiveSum[group];

                if (groupSpinsSince[group] > maxSpinsSince[group]) {
                    maxSpinsSince[group] = groupSpinsSince[group];
                }
                if (groupConsecutive[group] > maxConsecutive[group]) {
                    maxConsecutive[group] = groupConsecutive[group];
                }
            }

            await WriteLineForAsync(cell);
        }

        public async Task CreateSummaryFileAsync(string filepath) {
            using var writer = new StreamWriter(filepath, false);
            //foreach(var group in groupOuputOrder) {
            //    await writer.WriteLineAsync($"--- {group} ---");
            //    double avgLastSince = (double)groupSpinsSinceSum[group] / _numberOfSpins;
            //    await writer.WriteLineAsync($"last since - avg: {avgLastSince:F}");
            //    await writer.WriteLineAsync($"last since - max: {maxSpinsSince[group]}");
            //    await writer.WriteLineAsync($"max consecutive:  {maxConsecutive[group]}");
            //    await writer.WriteLineAsync("");
            //}

            await writer.WriteLineAsync($"Number of spins: {_numberOfSpins:N0}\n");
            var nextThree = groupOuputOrder.Take(3);
            int index = 0;
            while(nextThree != null && nextThree.Count() > 0) {
                foreach (var group in nextThree) {
                    await writer.WriteAsync($"------ {group} ------".PadRight(30));
                }
                await writer.WriteLineAsync("");
                foreach (var group in nextThree) {
                    double avgLastSince = (double)groupSpinsSinceSum[group] / _numberOfSpins;
                    await writer.WriteAsync($"last since - avg: {avgLastSince:F}".PadRight(30));
                }
                await writer.WriteLineAsync("");
                foreach (var group in nextThree) {
                    await writer.WriteAsync($"last since - max: {maxSpinsSince[group]}".PadRight(30));
                }
                await writer.WriteLineAsync("");
                foreach (var group in nextThree) {
                    await writer.WriteAsync($"max consecutive:  {maxConsecutive[group]}".PadRight(30));
                }

                await writer.WriteLineAsync("");
                await writer.WriteLineAsync("");

                index += 3;
                nextThree = groupOuputOrder.Skip(index).Take(3);
            }
        }
    }
}