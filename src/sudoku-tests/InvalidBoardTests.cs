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

        string _board = "11...............................................................................";

        [Fact]
        public void RepeatingValues()
        {
            var puzzle = new Puzzle(_board);
            var solved = puzzle.IsSolved();
            Assert.False(solved, "Puzzle should be rejected.");
        }
    }
}
