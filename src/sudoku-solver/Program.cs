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
            var puzzle = tests[2];

            var solvers = new ISolver[]
            {
                new SingleEntrySolver(puzzle),
                new HiddenSinglesSolver(puzzle),
                new HighestOccuringAdjacentSolver(puzzle)
            };
         
            var iterations = 0;
            var solved = false;
            foreach ((var solution, var attempts) in puzzle.TrySolvers(solvers))
            {
                iterations++;
                if (solution.Solved)
                {
                    WriteLine($"Solved cell: {solution.GetLocation()}; {solution.Value}");
                    WriteLine($"Solved by: {solution.Solver}");
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
                DrawPuzzle(puzzle, solution);
                WriteLine();
                solved = puzzle.IsSolved();
                if (solution.Solved && solved)
                {
                    WriteLine("Puzzle is solved!");
                    break;
                }
            }

            WriteLine($"{iterations} iterations of solutions used.");
        }

        public static void DrawPuzzle(Puzzle puzzle, Solution solution)
        {
            PrintColumnSolution(solution);

            for (int i = 0; i < 9; i++)
            {
                var row = puzzle.GetRow(i);

                if (i == 3 || i == 6)
                {
                    WriteLine("------+-------+------");
                }
                
                for (int j = 0; j < 9; j++)
                {
                    if (j == 3 || j == 6)
                    {
                        Write($"| {row.Segment[j]} ");
                    }
                    else if (j == 8)
                    {
                        Write($"{row.Segment[j]}");
                    }
                    else
                    {
                        Write($"{row.Segment[j]} ");
                    }
                }

                if (solution.Solved && i == solution.Row)
                {
                    Write("*");
                }

                WriteLine();
            }

            void PrintColumnSolution(Solution solution)
            {
                if (!solution.Solved)
                {
                    return;
                }

                for(int i = 0; i < solution.Column; i++)
                {
                    Write("  ");
                    if (i ==3 || i == 6)
                    {
                        Write("  ");
                    }
                }
                Write("*");
                WriteLine();
            }
        }

    }
}
