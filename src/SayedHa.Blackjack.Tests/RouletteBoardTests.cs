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

            var board = player.BuildBoard(new GameSettings());

            Assert.NotNull(board);
            Assert.NotNull(board.Cells);
            Assert.Equal(38, board.Cells.Count);
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
