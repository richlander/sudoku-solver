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

        // This puzzle is composed of "hidden singles".
        // ..2.3...8.....8....31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..
        // It requires identifying solutions by virtue of a neighboring unit (row, column, box)
        // holding a value, resulting in a single cell where that value can reside.
        // It differs from naked singles since the remaining solution

        // Example:
        // Solved cell: r3:c1; 8
        // Solved by: HiddenSinglesSolver:RowSolver
        // *
        // 0 0 2 | 0 3 0 | 0 0 8
        // 0 0 0 | 0 0 8 | 0 0 0
        // 8 3 1 | 0 2 0 | 0 0 0*
        // ------+-------+------
        // 0 6 0 | 0 5 0 | 2 7 0
        // 0 1 0 | 0 0 0 | 0 5 0
        // 2 0 4 | 0 6 0 | 0 3 1
        // ------+-------+------
        // 0 0 0 | 0 8 0 | 6 0 5
        // 0 0 0 | 0 0 0 | 0 1 3
        // 0 0 5 | 3 1 0 | 4 0 0

        string _board = "..2.3...8.....8....31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..";
        string _completedBoard = "672435198549178362831629547368951274917243856254867931193784625486592713725316489";

        private Puzzle GetBaseCase()
        {
            Puzzle puzzle = new(_board);
            puzzle.AddSolver(new HiddenSinglesSolver());
            return puzzle;
        }

        [Fact]
        public void FindFirstSolution()
        {
            Puzzle puzzle = GetBaseCase();
            Assert.True(puzzle.TrySolve(out Solution solution), "A solved solution should be returned.");
        }

        [Fact]
        public void SolvePuzzle()
        {
            Puzzle puzzle = GetBaseCase();
            Assert.False(puzzle.TrySolve(out Solution solution) && puzzle.ToString() == _completedBoard, "Puzzle should  be solved.");
        }

        // [SolverStrategy(Strategy.RowSolver)]
        [Fact]
        public void OneEmptySlotInRowHorizontalOnly()
        {
            var expectedValue = 8;
            Puzzle puzzle = GetBaseCase();
            Assert.True(puzzle.TrySolve(out Solution solution) && solution.Value == expectedValue, "Box should be solved.");
        }

        // [SolverStrategy(Strategy.RowColumnSolver)]
        [Fact]
        public void AllSlotsEmpty()
        {
            var puzzle = new Puzzle(_board);
            var expectedValue = 3;
            var solver = new HiddenSinglesSolver(puzzle);
            Assert.True(solver.TrySolveBox(2, out Solution solution) && solution.Value == expectedValue, "Box should be solved.");
        }

        // [SolverStrategy(Strategy.ColumnSolver)]
        [Fact]
        public void LastColumnSlotEmpty()
        {
            var board = "..2.3.1.8...1.83..831.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..";
            var expectedValue = 5;
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            Assert.True(solver.TrySolveBox(2, out Solution solution) && solution.Value == expectedValue, "Box should be solved.");
        }

        // [SolverStrategy(Strategy.Row2BlockedRow3CandidateColumnSolver)]
        [Fact]
        public void TwoColumnSlotsEmpty1()
        {
            var board = "..2.3...8.....83...31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..";
            var expectedValue = 1;
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            Assert.True(solver.TrySolveBox(2, out Solution solution) && solution.Value == expectedValue, "Box should be solved.");
        }

        // [SolverStrategy(Strategy.Row2CandidateRow3BlockedColumnSolver)]
        [Fact]
        public void TwoColumnSlotsEmpty2()
        {
            var board = "..2.3.1.8...1.83..831.2.5...6..5127..1.....5.254.6..31....8.6.5.......13..531.4..";
            var expectedValue = 3;
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            Assert.True(solver.TrySolveBox(4, out Solution solution) && solution.Value == expectedValue, "Box should be solved.");
        }

        // [SolverStrategy(Strategy.Column2CandidateColumn3BlockedRowSolver)]
        [Fact]
        public void AllColumnSlotsEmptyA()
        {
            var board = "..2.3.1.8.....83...31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..";
            var expectedValue = 1;
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            Assert.True(solver.TrySolveBox(1, out Solution solution) && solution.Value == expectedValue, "Box should be solved.");
        }

        // [SolverStrategy(Strategy.Column2BlockedColumn3CandidateRowSolver)]
        [Fact]
        public void AllColumnSlotsEmptyB()
        {
            var board = "..2.3.1.8...1.83..831.2.5...6..51274.1.243.56254.6..311...8.6.5.......13..531.4..";
            var expectedValue = 3;
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            Assert.True(solver.TrySolveBox(6, out Solution solution) && solution.Value == expectedValue, "Box should be solved.");
        }


        // [SolverStrategy(Strategy.Column2BlockedColumn3CandidateRowSolver)]
        [Fact]
        public void Column2BlockedColumn3CandidateRowSolver()
        {
            var board = "..2.3.1.8...1.83..831.2.5...6..5127..1.2.3.56254.6..31....8.6.5.......13..531.4..";
            var expectedValue = 4;
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            Assert.True(solver.TrySolveBox(5, out Solution solution) && solution.Value == expectedValue, "Box should be solved.");
        }


        // [SolverStrategy(Strategy.ColumnLastPossibleSlot)]
        [Fact]
        public void ColumnLastPossibleSlotBox1Blocked()
        {
            var board = "..2.3.1.8...1.83..831.2.5..36..51274.1.243.56254.6..311.3.8.6.5.......13..531.4..";
            var expectedValue = 7;
            var puzzle = new Puzzle(board);
            var solver = new HiddenSinglesSolver(puzzle);
            Assert.True(solver.TrySolveBox(8, out Solution solution) && solution.Value == expectedValue, "Box should be solved.");
        }


        // [SolverStrategy(Strategy.ColumnLastPossibleSlot)]
        [Fact]
        public void ColumnLastPossibleSlotBox2Blocked()
        {
            var board = "..2.3.1.8...1.83..831.2.5..36..51274.1.243.56254.6..311.3.8.6.5......713..531.4..";
            var expectedValue = 8;
            var puzzle = new Puzzle(board);
            Console.WriteLine(puzzle);
            var solver = new HiddenSinglesSolver(puzzle);
            Assert.True(solver.TrySolveBox(8, out Solution solution) && solution.Value == expectedValue, "Box should be solved.");
        }
    }
}
