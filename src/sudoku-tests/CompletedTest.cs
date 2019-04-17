using System;
using sudoku_solver;
using Xunit;

namespace sudoku_tests
{
    public class CompletedTest
    {
        // These tests use a puzzle from: 
        // http://sudopedia.enjoysudoku.com/Valid_Test_Cases.html
        // That page is licensed with the GNU Free Documentation License
        // https://www.gnu.org/copyleft/fdl.html

        // The targeted puzzle is already complete and should be detected as such
        // Puzzle: 974236158638591742125487936316754289742918563589362417867125394253649871491873625

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
