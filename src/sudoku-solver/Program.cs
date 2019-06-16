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
            var puzzle = tests[4];

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

            var solutionsFound = 0;
            foreach (var solution in puzzle.TrySolvers(solvers))
            {
                iterations++;
                if (solution.Solved)
                {
                    solutionsFound++;
                    var solverKind = solution.SolverKind is null ? $"{solution.Solver}" : $"{solution.Solver}:{solution.SolverKind}";
                    WriteLine($"Solved cell: {solution.GetLocation()}; {solution.Value}");
                    WriteLine($"Solved by: {solverKind}");
                }
                else
                {
                    WriteLine("No more solutions found.");
                }
                
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
            WriteLine($"{solutionsFound} solutions found.");
            //WriteLine($"{iterations} solutions attempted.");
        }
    }
}
