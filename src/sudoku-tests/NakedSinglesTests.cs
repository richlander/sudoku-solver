using System;
using sudoku_solver;
using Xunit;

namespace sudoku_tests
{
    public class NakedSinglesTests
    {
        // this puzzle should be solvable with a "naked singles solver".
        // From: http://sudopedia.enjoysudoku.com/Valid_Test_Cases.html
        private string _board = "3.542.81.4879.15.6.29.5637485.793.416132.8957.74.6528.2413.9.655.867.192.965124.8";
        
        [Fact]
        public void ColumnTest()
        {
            var puzzle = new Puzzle(_board);
            var solver = new NakedSinglesSolver(puzzle);
            var solution = solver.SolveColumn(2);
            Assert.True(solution.Solved, "Column should  be solved.");
        }

        [Fact]
        public void RowTest()
        {
            var puzzle = new Puzzle(_board);
            var solver = new NakedSinglesSolver(puzzle);
            Solution solution;
            solution = solver.SolveColumn(2);
            puzzle.Update(solution);
            solution = solver.SolveRow(3);
            Assert.True(solution.Solved, "Row should  be solved.");
        }

        [Fact]
        public void BoxTest()
        {
            var puzzle = new Puzzle(_board);
            var solver = new NakedSinglesSolver(puzzle);
            Solution solution;
            solution = solver.SolveColumn(2);
            puzzle.Update(solution);
            solution = solver.SolveRow(3);
            puzzle.Update(solution);
            solution = solver.SolveBox(3);
            Assert.True(solution.Solved, "Box should  be solved.");
        }

        [Fact]
        public void PuzzleTest()
        {
            var puzzle = new Puzzle(_board);
            var solver = new NakedSinglesSolver(puzzle);
            var solved = puzzle.Solve(new ISolver[]{solver});
            Assert.True(solved, "Puzzle should  be solved.");
        }
    }
}
