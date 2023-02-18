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
using SayedHa.Blackjack.Shared.Roulette;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class RouletteBoardTests {
        [Fact]
        public void CreateBoard_Default() {
            var player = new RoulettePlayer();

            var board = Board.BuildBoard(new GameSettings());

            Assert.NotNull(board);
            Assert.NotNull(board.Cells);
            Assert.Equal(38, board.Cells!.Count);
            // 1 is red
            Assert.Equal(GameCellColor.Red, board.Cells[0].Color);
            // 2 is black
            Assert.Equal(GameCellColor.Black, board.Cells[1].Color);
            // 3 is red
            Assert.Equal(GameCellColor.Red, board.Cells[2].Color);
            // last two are green
            Assert.Equal(GameCellColor.Green, board.Cells[^1].Color);
            Assert.Equal(GameCellColor.Green, board.Cells[^2].Color);
        }
    }
}
