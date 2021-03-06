namespace SayedHa.Blackjack.Shared.Roulette {
    public class Board {
        public List<GameCell>? Cells { get; set; }

        public static Board BuildBoard(GameSettings settings) {
            var factory = new GameCell.GameCellFactory();
            var cells = new List<GameCell>();
            // build a list of GameCells starting with 1 - 36 alternating red and black, then add special cells

            // 1 is red
            var currentColor = GameCellColor.Red;
            for (int i = 1; i <= 36; i++) {
                cells.Add(factory.NewCell(i, currentColor));
                currentColor = currentColor == GameCellColor.Black ? GameCellColor.Red : GameCellColor.Black;
            }

            // add special cells as specified in settings
            if (settings?.SpecialCells?.Length > 0) {
                int currentValue = 37;
                foreach (var special in settings.SpecialCells) {
                    cells.Add(factory.NewCell(currentValue++, special, GameCellColor.Green));
                }
            }

            return new Board { Cells = cells };
        }
    }
}
