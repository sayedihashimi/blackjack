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
namespace SayedHa.Blackjack.Shared.Roulette {

    // some stats that we want to gather
    //  spins since last red/black/green
    public class CsvWithStatsGameRecorder : CsvGameRecorder,IGameRollupRecorder,IConsoleSummaryGameRecorder {
        public CsvWithStatsGameRecorder(string outputPath) : base(outputPath) {
            groupSpinsSince = new Dictionary<GameCellGroup, int>();
            groupConsecutive = new Dictionary<GameCellGroup, int>();
            groupSpinsSinceSum = new Dictionary<GameCellGroup, long>();
            groupConsecutiveSum = new Dictionary<GameCellGroup, long>();
            maxSpinsSince = new Dictionary<GameCellGroup, int>();
            maxConsecutive = new Dictionary<GameCellGroup, int>();
            groupSpinsSinceSumOfSquares = new();

            foreach (GameCellGroup group in EnumHelper.GetHelper().GetAllGameCellGroup()) {
                groupSpinsSince.Add(group, 0);
                groupConsecutive.Add(group, 0);
                groupSpinsSinceSum.Add(group, 0);
                groupConsecutiveSum.Add(group, 0);
                maxSpinsSince.Add(group, 0);
                maxConsecutive.Add(group, 0);
                groupSpinsSinceSumOfSquares.Add(group, 0);
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
        public CsvWithStatsGameRecorder(string outputPath, string filenamePrefix) : this(outputPath) {
            FilenamePrefix = filenamePrefix;
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
        Dictionary<GameCellGroup, int> groupSpinsSinceSumOfSquares { get; init; }

        private bool _enableWriteCsvFile = true;
        public bool EnableWriteCsvFile {
            get {
                return EnableFileOutput && _enableWriteCsvFile;
            }
            set {
                _enableWriteCsvFile = value;
            }
        }

        List<GameCellGroup> groupOuputOrder { get; init; }
        public override string GetCsvFilePath() => Path.Combine(OutputPath!, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}stats.csv" : $"stats.csv");
        public string GetCsvSummaryFilepath() => Path.Combine(OutputPath!, !string.IsNullOrEmpty(FilenamePrefix) ? $"{FilenamePrefix}summary.txt" : $"summary.txt");
        protected override async Task WriteHeaderAsync() {
            if (!EnableWriteCsvFile || !EnableFileOutput) {
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
            if (!EnableWriteCsvFile || !EnableFileOutput) {
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

                groupSpinsSinceSumOfSquares[group] += (groupSpinsSince[group] * groupSpinsSince[group]);
            }

            await WriteLineForAsync(cell);
        }
        public override void Reset() {
            base.Reset();
            groupSpinsSince.Clear();
            groupConsecutive.Clear();
            groupSpinsSinceSum.Clear();
            groupConsecutiveSum.Clear();
            maxSpinsSince.Clear();
            maxConsecutive.Clear();
            groupSpinsSinceSumOfSquares.Clear();

            foreach (GameCellGroup group in EnumHelper.GetHelper().GetAllGameCellGroup()) {
                groupSpinsSince.Add(group, 0);
                groupConsecutive.Add(group, 0);
                groupSpinsSinceSum.Add(group, 0);
                groupConsecutiveSum.Add(group, 0);
                maxSpinsSince.Add(group, 0);
                maxConsecutive.Add(group, 0);
                groupSpinsSinceSumOfSquares.Add(group, 0);
            }
        }
        public async Task WriteTextSummaryToAsync(StreamWriter writer) {
            await writer.WriteLineAsync($"Number of spins: {_numberOfSpins:N0}\n");
            await writer.WriteLineAsync($"* Legend ********************************************************************************************");
            await writer.WriteLineAsync($"*  last since = The number of spins since this group was last hit.                                  *");
            await writer.WriteLineAsync($"*  max consecutive = The maximum number of spins which this group hit consecutively (in a row).     *");
            await writer.WriteLineAsync($"*****************************************************************************************************\n");
            var nextThree = groupOuputOrder.Take(3);
            int index = 0;
            while (nextThree != null && nextThree.Count() > 0) {
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
                    var mean = groupSpinsSinceSum[group] / _numberOfSpins;
                    await writer.WriteAsync($"last since - mean: {mean}".PadRight(30));
                }
                await writer.WriteLineAsync("");
                foreach (var group in nextThree) {
                    // calculate the standard deviation using the average and count
                    await writer.WriteAsync($"last since - max: {maxSpinsSince[group]}".PadRight(30));
                }

                await writer.WriteLineAsync("");
                foreach (var group in nextThree) {
                    await writer.WriteAsync($"max consecutive:  {maxConsecutive[group]}".PadRight(30));
                }

                await writer.WriteLineAsync("");
                foreach (var group in nextThree) {
                    var sumOfSquares = groupSpinsSinceSumOfSquares[group];
                    var mean = groupSpinsSinceSum[group] / _numberOfSpins;
                    var variance = Math.Sqrt(sumOfSquares / _numberOfSpins - mean * mean);
                    await writer.WriteAsync($"stddev:  {variance:F2}".PadRight(30));
                }

                await writer.WriteLineAsync("");
                await writer.WriteLineAsync("");

                index += 3;
                nextThree = groupOuputOrder.Skip(index).Take(3);
            }
        }
        public async Task CreateSummaryFileAsync() {
            if(!EnableFileOutput) { return; }

            using var writer = new StreamWriter(GetCsvSummaryFilepath(), false);

            await WriteTextSummaryToAsync(writer);
        }
        public override async Task GameCompleted() {
            await CreateSummaryFileAsync();
        }

        public async Task WriteGameSummaryHeaderToAsync(StreamWriter writer) {
            await writer.WriteLineAsync("group,avgLastSince,maxSpinsSince,maxConsecutive");
        }

        public async Task WriteGameSummaryToAsync(StreamWriter writer) {
            foreach (var group in groupOuputOrder) {
                double avgLastSince = (double)groupSpinsSinceSum[group] / _numberOfSpins;
                await writer.WriteAsync($"{group},");
                await writer.WriteAsync($"{avgLastSince},");
                await writer.WriteAsync($"{maxSpinsSince[group]},");
                await writer.WriteAsync($"{maxConsecutive[group]}");
                await writer.WriteLineAsync(string.Empty);
            }
        }

        string IGameRollupRecorder.GetMethodDisplayName() => "Group stats";

        string IGameRollupRecorder.GetMethodCompactName() => "groupstats";
    }
}
