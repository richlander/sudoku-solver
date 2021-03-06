using System;
using System.Linq;
using sudoku_solver;
using Xunit;

namespace sudoku_tests
{
    public class NakedSinglesTests
    {
        // These tests use a puzzle from: 
        // http://sudopedia.enjoysudoku.com/Valid_Test_Cases.html
        // That page is licensed with the GNU Free Documentation License
        // https://www.gnu.org/copyleft/fdl.html

        // The targeted puzzle should be solvable with a "naked singles solver"
        // Puzzle: 3.542.81.4879.15.6.29.5637485.793.416132.8957.74.6528.2413.9.655.867.192.965124.8

        string _board = "3.542.81.4879.15.6.29.5637485.793.416132.8957.74.6528.2413.9.655.867.192.965124.8";
        string _completedBoard = "365427819487931526129856374852793641613248957974165283241389765538674192796512438";

        [Fact]
        public void FindFirstSolution()
        {
            var puzzle = new Puzzle(_board);
            var solver = new NakedSinglesSolver(puzzle);
            var solution = solver.FindSolution().First();
            Assert.True(solution.Solved, "A solved solution should be returned.");
        }

        [Fact]
        public void SolvePuzzle()
        {
            var puzzle = new Puzzle(_board);
            var solver = new NakedSinglesSolver(puzzle);
            var solved = puzzle.Solve(solver);
            Assert.True(solved && puzzle.ToString() == _completedBoard, "Puzzle should  be solved.");
        }

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
            var board = "3.542.81.4879.15.6.29.56374852793.416132.8957.74.6528.2413.9.655.867.192.965124.8";
            var puzzle = new Puzzle(board);
            var solver = new NakedSinglesSolver(puzzle);
            var solution = solver.SolveRow(3);
            Assert.True(solution.Solved, "Row should  be solved.");
        }

        [Fact]
        public void BoxTest()
        {
            var board = "3.542.81.4879.15.6.29.563748527936416132.8957.74.6528.2413.9.655.867.192.965124.8";
            var puzzle = new Puzzle(board);
            var solver = new NakedSinglesSolver(puzzle);
            var solution = solver.SolveBox(3);
            Assert.True(solution.Solved, "Box should  be solved.");
        }
    }
}
