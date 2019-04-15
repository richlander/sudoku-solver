using System;
using sudoku_solver;
using Xunit;

namespace sudoku_tests
{
    public class CompletedTest
    {
        [Fact]
        public void CheckCompletedPuzzle()
        {
            var board = "974236158638591742125487936316754289742918563589362417867125394253649871491873625";
            var puzzle = new Puzzle(board);
            var solved = puzzle.IsSolved();
            Assert.True(solved,"Puzzle should be solved.");
        }

        [Fact]
        public void AttemptToSolvedCompletedPuzzle()
        {
            var board = "974236158638591742125487936316754289742918563589362417867125394253649871491873625";
            var puzzle = new Puzzle(board);
            var solver = new NakedSinglesSolver(puzzle);
            var solved = puzzle.Solve(solver);
            Assert.True(solved, "Puzzle should be solved.");
        }
    }
}
