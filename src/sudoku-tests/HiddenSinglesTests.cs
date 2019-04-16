using System;
using sudoku_solver;
using Xunit;
using System.Linq;

namespace sudoku_tests
{
    public class HiddenSinglesTests
    {
        // this puzzle should be solvable with a "hidden singles solver".
        // From: http://sudopedia.enjoysudoku.com/Valid_Test_Cases.html
        private string _board = "..2.3...8.....8....31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..";

        [Fact]
        public void FullCourtPress()
        {
            var board = "..2.3...8.....8....31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..";
            var puzzle = new Puzzle(board);
            ISolver solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.FindSolution().First();
            Assert.True(solution.Solved, "Box should be solved.");
        }

        [Fact]
        public void HorizontalOnlyOneEmptySlotInRow()
        {
            var puzzle = new Puzzle(_board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.Solve(0);
            var expectedValue = 8;
            Assert.True(solution.Solved && solution.Value == expectedValue, "Box should be solved.");
        }

        [Fact]
        public void AllSlotsEmptyInRow()
        {
            var puzzle = new Puzzle(_board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.Solve(2);
            var expectedValue = 3;
            Assert.True(solution.Solved && solution.Value == expectedValue, "Box should be solved.");
        }

        [Fact]
        public void TwoSlotsEmptyInRowAndColumn()
        {
            var puzzle = new Puzzle(_board);
            var solver = new HiddenSinglesSolver(puzzle);
            Solution solution;
            solution = solver.Solve(2);
            puzzle.Update(solution);
            solution = solver.Solve(2);
            var expectedValue = 1;
            Assert.True(solution.Solved && solution.Value == expectedValue, "Box should be solved.");
        }
    }
}
