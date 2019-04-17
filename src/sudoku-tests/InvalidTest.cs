using System;
using sudoku_solver;
using Xunit;

namespace sudoku_tests
{
    public class InvalidTest
    {
        // These tests use a puzzle from: 
        // http://sudopedia.enjoysudoku.com/Valid_Test_Cases.html
        // That page is licensed with the GNU Free Documentation License
        // https://www.gnu.org/copyleft/fdl.html

        // The targeted puzzle is invalid and should be detected as such
        // Puzzle: 11...............................................................................

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
