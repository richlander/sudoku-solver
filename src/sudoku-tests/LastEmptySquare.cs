using System;
using sudoku_solver;
using Xunit;
using System.Linq;
using sudoku_solver;

namespace sudoku_tests
{
    public class LastEmptySquare
    {
        [Fact]
        public void CompletePuzzle()
        {
            var board = "2564891733746159829817234565932748617128.6549468591327635147298127958634849362715";
            var puzzle = new Puzzle(board);
            var solved = puzzle.Solve(solver);
            Assert.True(solution.Solved, "Puzzle should be solved.");
        }
    }
}
