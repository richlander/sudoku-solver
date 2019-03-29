using System;
using sudoku_solver;
using Xunit;

namespace sudoku_tests
{
    public class CompletedTest
    {
        [Fact]
        public void Test1()
        {
            var board = "974236158638591742125487936316754289742918563589362417867125394253649871491873625";

            var puzzle = new Puzzle(board);
            var solved = puzzle.IsSolved();
            Assert.True(solved,"Puzzle should be solved.");
        }
    }
}
