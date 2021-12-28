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

        // This puzzle is invalid.
        // 11...............................................................................

        string _board = "11...............................................................................";

        [Fact]
        public void RepeatingValues()
        {
            string message = string.Empty;
            string expectedMessage = "Puzzle is not valid.";
            try
            {
                var puzzle = new Puzzle(_board);
            }
            catch(Exception e)
            {
                message = e.Message;
            }
            Assert.True(message == expectedMessage, "Puzzle should be rejected.");
        }
    }
}
