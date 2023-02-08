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
