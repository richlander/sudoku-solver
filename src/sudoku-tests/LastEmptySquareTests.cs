using System;
using sudoku_solver;
using Xunit;
using System.Linq;

namespace sudoku_tests
{
    public class LastEmptySquare
    {
        // These tests use a puzzle from: 
        // http://sudopedia.enjoysudoku.com/Valid_Test_Cases.html
        // That page is licensed with the GNU Free Documentation License
        // https://www.gnu.org/copyleft/fdl.html

        // The targeted puzzle has one empty square and should be solvable by any solver
        // Puzzle: 2564891733746159829817234565932748617128.6549468591327635147298127958634849362715

        string _board = "2564891733746159829817234565932748617128.6549468591327635147298127958634849362715";
        string _completeBoard = "256489173374615982981723456593274861712836549468591327635147298127958634849362715";

        [Fact]
        public void CompletePuzzle()
        {
            var puzzle = new Puzzle(_board);
            var solver = new NakedSinglesSolver(puzzle);
            var solved = puzzle.Solve(solver);
            Assert.True(solved && puzzle.ToString() == _completeBoard, "Puzzle should be solved.");
        }

        [Fact]
        public void IsCompletePuzzle()
        {
            var puzzle = new Puzzle(_board);
            var solved = puzzle.IsSolved();
            Assert.False(solved, "Puzzle should not be solved.");
        }
    }
}
