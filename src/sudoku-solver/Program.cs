using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;

namespace sudoku_solver
{
    class Program
    {
        static void Main(string[] args)
        {
            //var file = new FileInfo("puzzle.json");
            //var puzzle = PuzzleReader.ReadPuzzle(file);

            var tests = PuzzleTest.Get().ToArray();
            var puzzle = tests[1];

            var solvers = new List<ISolver>
            {
                new LastEntrySolver(puzzle)
            };
         
            foreach ((var solution, var attempts) in puzzle.TrySolvers(solvers))
            {
                if (solution.Solved)
                {
                    WriteLine($"Solved cell: {}");
                    Console.WriteLine($"Solved box {solution.Index + 1}; cell {solution.Cell + 1} -> {solution.Value}");

                }
                else
                {
                    Console.WriteLine("No more solutions found");
                    Console.WriteLine($"Tried {attempts} attempts to solve puzzle.");
                }
            }
        }

    }
}
