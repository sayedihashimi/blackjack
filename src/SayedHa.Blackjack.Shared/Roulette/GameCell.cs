namespace SayedHa.Blackjack.Shared.Roulette {
    public class GameCell {
        private GameCell() { }
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
        private List<GameCellGroup>? _groups;
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

            if( cell.Color != GameCellColor.Green && cell.Value != int.MaxValue && cell.Value != int.MinValue) {
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
            }

            // colors
            switch (cell.Color) {
                case GameCellColor.Red:
                    result.Add(GameCellGroup.Red);
                    break;
                case GameCellColor.Black:
                    result.Add(GameCellGroup.Black);
                    break;
                case GameCellColor.Green:
                    result.Add(GameCellGroup.Green);
                    break;
                default: throw new ArgumentOutOfRangeException(cell.Color.ToString());
            }

            return result;
        }

        public override bool Equals(object? obj) {
            return obj is GameCell cell &&
                   Value == cell.Value &&
                   Color == cell.Color &&
                   Text == cell.Text;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Value, Color, Text);
        }

        public class GameCellFactory {
            public GameCell NewCell(int value, GameCellColor color) => new GameCell {
                Value = value,
                Text = value.ToString(),
                Color = color
            };
            public GameCell NewCell(int value,string text, GameCellColor color) => new GameCell {
                Value = value,
                Text = text,
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
        Red,
        Black,
        Green,
        First12,
        Second12,
        Third12,
        First18,
        Second18,
        FirstColumn,
        SecondColumn,
        ThirdColumn
    }
}
