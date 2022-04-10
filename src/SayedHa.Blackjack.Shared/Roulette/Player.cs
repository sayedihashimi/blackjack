using System;
using System.Collections.Generic;
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
        public string Text { get; init; }

        public override string ToString() {
            return $"{Text} {Color}";
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
    public class GameRoll {
        public GameCell CellHit { get; set; }
    }
    public class GameRecord {
        public LinkedList<GameCell>? CellsHit { get; set; } = new LinkedList<GameCell>();
    }

    public class Board {
        public List<GameCell> Cells { get; set; }
    }

    public class RoulettePlayer {
        public void Play(GameSettings settings) {
            // first build the board
            var board = BuildBoard(settings);
            var numCells = board.Cells.Count();

            var numberOfSpins = 1000;
            for (int i = 0; i < numberOfSpins; i++) {
                // generate a random GameCell
                var spinValueIndex = GetRandomNum(numCells);
                var spinValue = board.Cells[spinValueIndex];
                Console.WriteLine($"{spinValue}");
            }
        }
        private Random _random = new Random();
        // TODO: does this need to be improved?
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
                foreach(var special in settings.SpecialCells) {
                    cells.Add(factory.NewGreenCell(currentId++, special));
                }
            }

            return new Board { Cells = cells };
        }
    }
}
