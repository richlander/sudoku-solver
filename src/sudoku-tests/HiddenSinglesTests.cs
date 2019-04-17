using System;
using sudoku_solver;
using Xunit;
using System.Linq;

namespace sudoku_tests
{
    public class HiddenSinglesTests
    {
        // These tests use a puzzle from: 
        // http://sudopedia.enjoysudoku.com/Valid_Test_Cases.html
        // That page is licensed with the GNU Free Documentation License
        // https://www.gnu.org/copyleft/fdl.html

        // The targeted puzzle should be solvable with a "hidden singles solver"
        // Puzzle: ..2.3...8.....8....31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..


        [Fact]
        public void FindASolution()
        {
            var board = "..2.3...8.....8....31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..";
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.FindSolution().First();
            Assert.True(solution.Solved, "A solved solution should be returned.");
        }

        [Fact]
        public void HorizontalOnlyOneEmptySlotInRow()
        {
            var board = "..2.3...8.....8....31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..";
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.Solve(0);
            var expectedValue = 8;
            Assert.True(solution.Solved && solution.Value == expectedValue, "Box should be solved.");
        }

        [Fact]
        public void AllSlotsEmptyInRow()
        {
            var board = "..2.3...8.....8....31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..";
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.Solve(2);
            var expectedValue = 3;
            Assert.True(solution.Solved && solution.Value == expectedValue, "Box should be solved.");
        }

        [Fact]
        public void TwoSlotsEmptyInRowAndColumn()
        {
            var board = "..2.3...8.....83...31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..";
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.Solve(2);
            var expectedValue = 1;
            Assert.True(solution.Solved && solution.Value == expectedValue, "Box should be solved.");
        }
    }
}
