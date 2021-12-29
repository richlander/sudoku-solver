using System;
using System.Linq;
using sudoku_solver;
using Xunit;

namespace sudoku_tests
{
    public class NakedMultiplesTests
    {
        // These tests use a puzzle from: 
        // http://sudopedia.enjoysudoku.com/Valid_Test_Cases.html
        // That page is licensed with the GNU Free Documentation License
        // https://www.gnu.org/copyleft/fdl.html

        // This puzzle is composed of "naked multiples".
        // 79458213626..31.......76..268.71.32443286......724386..2.6574.3.4.1286.7.763942.8
        // It requires removing candidates based on cells in the same unit with only those candidates.
        // For example, two cells might have only 5 and 9 as candidates. 
        // That means that 5 and 9 can be removed as candidates from all other cells in the unit. 

        // Example:
        // Solved cell: r4:c3; 2
        // Solved by: NakedSinglesSolver
        //     *
        // 3 0 5 | 4 2 0 | 8 1 0
        // 4 8 7 | 9 0 1 | 5 0 6
        // 0 2 9 | 0 5 6 | 3 7 4
        // ------+-------+------
        // 8 5 2 | 7 9 3 | 0 4 1*
        // 6 1 3 | 2 0 8 | 9 5 7
        // 0 7 4 | 0 6 5 | 2 8 0
        // ------+-------+------
        // 2 4 1 | 3 0 9 | 0 6 5
        // 5 0 8 | 6 7 0 | 1 9 2
        // 0 9 6 | 5 1 2 | 4 0 8

        string _board = "79458213626..31.......76..268.71.32443286......724386..2.6574.3.4.1286.7.763942.8";
        string _nextSolution = "79458213626..317......76..268.71.32443286......724386..2.6574.3.4.1286.7.763942.8";

        [Fact]
        public void FindNextSolution()
        {
            Puzzle puzzle = new(_board);
            puzzle.AddSolver(new NakedMultiplesSolver());
            Assert.True(puzzle.Solve() && puzzle.ToString() == _nextSolution, "A solved solution should be returned.");
        }
    }
}
