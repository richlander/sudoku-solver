using System;
using sudoku_solver;
using Xunit;

namespace sudoku_tests
{
    public class NakedSingles
    {
        [Fact]
        public void HorizontalOnly()
        {
            var board = "..2.3...8.....8....31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..";

            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.Solve(0);
            //var solved = puzzle.IsSolved();
            Assert.True(solution.Solved, "Puzzle should  be solved.");
        }
    }
}
