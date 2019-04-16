using System;
using sudoku_solver;
using Xunit;

namespace sudoku_tests
{
    public class InvalidTest
    {
        [Fact]
        public void RepeatingValues()
        {
            var board = "11...............................................................................";
            var puzzle = new Puzzle(board);
            var solved = puzzle.IsSolved();
            Assert.False(solved, "Puzzle should not be solved.");
        }
    }
}
