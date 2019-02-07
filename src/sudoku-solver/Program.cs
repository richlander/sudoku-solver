using System;
using System.IO;
using System.Linq;

namespace sudoku_solver
{
    class Program
    {
        static void Main(string[] args)
        {
            //var file = new FileInfo("puzzle.json");
            //var puzzle = PuzzleReader.ReadPuzzle(file);

            var tests = PuzzleTest.Get().ToArray();

            var lastSolver = new LastEntrySolver(tests[0]);

            foreach (var solution in lastSolver.GetSolutions())
            {
                if (solution.Solved)
                {
                    Console.WriteLine($"Solved {solution.Value} @ box {solution.Box +1}; cell {solution.Cell + 1}");
                }
            }
        }
    }
}
