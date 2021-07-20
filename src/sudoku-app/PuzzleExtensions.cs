using static System.Console;

namespace sudoku_solver
{
    public static class PuzzleExtensions
    {
        public static void DrawPuzzle(this Puzzle puzzle, Solution solution)
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

                if (i == solution.Row)
                {
                    Write("*");
                }
                WriteLine();
            }
            WriteLine(puzzle);

            void PrintColumnSolution(Solution solution)
            {
                for(int i = 0; i < solution.Column; i++)
                {
                    Write("  ");
                    if (i == 2 || i == 5)
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