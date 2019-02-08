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
         
            TrySolvers(puzzle,solvers);
        }

        static void TrySolvers(Puzzle puzzle, IEnumerable<ISolver> solvers)
        {
            foreach (var solver in solvers)
            {
                foreach (var solution in solver.FindSolutions())
                {
                    if (solution.Solved)
                    {
                        Console.WriteLine($"Solved {solution.Value} @ box {solution.Box + 1}; cell {solution.Cell + 1}");
                        puzzle.UpdateCell(solution);                        

                        if (puzzle.IsFull())
                        {
                            Console.WriteLine("Puzzle is solved");
                        }
                    }
                }
            }

        }
    }
}
