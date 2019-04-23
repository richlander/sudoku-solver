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

        string _board = "..2.3...8.....8....31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..";
        string _completedBoard = "672435198549178362831629547368951274917243856254867931193784625486592713725316489";

        [Fact]
        public void FindFirstSolution()
        {
            var puzzle = new Puzzle(_board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.FindSolution().First();
            Assert.True(solution.Solved, "A solved solution should be returned.");
        }

        [Fact]
        public void SolvePuzzle()
        {
            var puzzle = new Puzzle(_board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solved = puzzle.Solve(solver);
            Assert.False(solved && puzzle.ToString() == _completedBoard, "Puzzle should  be solved.");
        }

        [SolverStrategy(Strategy.RowSolver)]
        [Fact]
        public void OneEmptySlotInRowHorizontalOnly()
        {
            var puzzle = new Puzzle(_board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.Solve(0);
            var expectedValue = 8;
            Assert.True(solution.Solved && solution.Value == expectedValue, "Box should be solved.");
        }

        [SolverStrategy(Strategy.RowColumnSolver)]
        [Fact]
        public void AllSlotsEmpty()
        {
            var puzzle = new Puzzle(_board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.Solve(2);
            var expectedValue = 3;
            Assert.True(solution.Solved && solution.Value == expectedValue, "Box should be solved.");
        }

        [SolverStrategy(Strategy.ColumnSolver)]
        [Fact]
        public void LastColumnSlotEmpty()
        {
            var board = "..2.3.1.8...1.83..831.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..";
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.Solve(2);
            var expectedValue = 5;
            Assert.True(solution.Solved && solution.Value == expectedValue, "Box should be solved.");
        }

        [SolverStrategy(Strategy.Row2BlockedRow3CandidateColumnSolver)]
        [Fact]
        public void TwoColumnSlotsEmpty1()
        {
            var board = "..2.3...8.....83...31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..";
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.Solve(2);
            var expectedValue = 1;
            Assert.True(solution.Solved && solution.Value == expectedValue, "Box should be solved.");
        }

        [SolverStrategy(Strategy.Row2CandidateRow3BlockedColumnSolver)]
        [Fact]
        public void TwoColumnSlotsEmpty2()
        {
            var board = "..2.3.1.8...1.83..831.2.5...6..5127..1.....5.254.6..31....8.6.5.......13..531.4..";
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.Solve(4);
            var expectedValue = 3;
            Assert.True(solution.Solved && solution.Value == expectedValue, "Box should be solved.");
        }

        [SolverStrategy(Strategy.Column2CandidateColumn3BlockedRowSolver)]
        [Fact]
        public void AllColumnSlotsEmptyA()
        {
            var board = "..2.3.1.8.....83...31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..";
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.Solve(1);
            var expectedValue = 1;
            Assert.True(solution.Solved && solution.Value == expectedValue, "Box should be solved.");
        }

        [SolverStrategy(Strategy.Column2BlockedColumn3CandidateRowSolver)]
        [Fact]
        public void AllColumnSlotsEmptyB()
        {
            var board = "..2.3.1.8...1.83..831.2.5...6..51274.1.243.56254.6..311...8.6.5.......13..531.4..";
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.Solve(6);
            var expectedValue = 3;
            Assert.True(solution.Solved && solution.Value == expectedValue, "Box should be solved.");
        }


        [SolverStrategy(Strategy.Column2BlockedColumn3CandidateRowSolver)]
        [Fact]
        public void Column2BlockedColumn3CandidateRowSolver()
        {
            var board = "..2.3.1.8...1.83..831.2.5...6..5127..1.2.3.56254.6..31....8.6.5.......13..531.4..";
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            var solution = solver.Solve(5);
            var expectedValue = 4;
            Assert.True(solution.Solved && solution.Value == expectedValue, "Box should be solved.");
        }

    }
}
