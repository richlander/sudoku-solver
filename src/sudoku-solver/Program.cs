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
            var puzzle = tests[3];

            var solvers = new ISolver[]
            {
                new NakedSinglesSolver(puzzle),
                //new NakedSingles2Solver(puzzle),
                new HiddenSinglesSolver(puzzle)
            };
         
            var iterations = 0;
            var solved = puzzle.IsSolved();

            if (solved)
            {
                Puzzle.DrawPuzzle(puzzle, new Solution(){Solved = false});
                WriteLine("Puzzle is solved!");
                return;
            }
            foreach ((var solution, var attempts) in puzzle.TrySolvers(solvers))
            {
                iterations++;
                if (solution.Solved)
                {
                    var solverKind = solution.SolverKind is null ? $"{solution.Solver}" : $"{solution.Solver}:{solution.SolverKind}";
                    WriteLine($"Solved cell: {solution.GetLocation()}; {solution.Value}");
                    WriteLine($"Solved by: {solverKind}");
                }
                else if (iterations == 0)
                {
                    WriteLine("No solutions found.");
                }
                else
                {
                    WriteLine("No more solutions found.");
                }
                
                WriteLine($"{attempts} solutions attempted.");
                Puzzle.DrawPuzzle(puzzle, solution);
                WriteLine();
                solved = puzzle.IsSolved();
                if (solution.Solved && solved)
                {
                    WriteLine("Puzzle is solved!");
                    break;
                }
                if (!puzzle.Validate().Valid)
                {
                    WriteLine("Something is busted!");
                    break;
                }
            }

            if (!solved)
            {
                WriteLine($"Solved cells: {puzzle.Solved}; Remaining: {81 - puzzle.Solved}");
                WriteLine(puzzle);
            }
            WriteLine($"{iterations} iterations of solutions used.");
        }
    }
}
