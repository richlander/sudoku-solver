using System;
using System.Collections.Generic;
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
            var puzzle = tests[1];

            var solvers = new List<ISolver>
            {
                new LastEntrySolver(puzzle)
            };
         
            var attempts = 1;
            while (TrySolvers(puzzle, solvers) > 0)
            {
                attempts++;
            }

            Console.WriteLine("No more solutions found.");
            Console.WriteLine($"Tried {attempts} attempts to solve puzzle with {solvers.Count} solvers.");
        }

        static int TrySolvers(Puzzle puzzle, IReadOnlyCollection<ISolver> solvers)
        {
            var success = 0;
            foreach (var solver in solvers)
            {
                if (!solver.CheckEffective())
                {
                    continue;
                }
                foreach (var solution in solver.FindSolutions())
                {
                    if (solution.Solved)
                    {
                        Console.WriteLine($"Solved box {solution.Index + 1}; cell {solution.Cell + 1} -> {solution.Value}");
                        puzzle.Update(solution);
                        success++;                        

                        if (puzzle.IsSolved())
                        {
                            Console.WriteLine("Puzzle is solved");
                        }
                    }
                }
            }
            return success;
        }
    }
}
