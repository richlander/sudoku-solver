using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static System.Console;

namespace sudoku_solver
{
    public class Puzzle
    {
        private Memory<int> _board;
        public static readonly int UnsolvedMarker = 0;
        public int TotalsCells = 81;

        public int Solved {get; private set;}
        private int UnSolvedcells => TotalsCells - Solved;
        public int[] SolvedForBox {get; private set;}
        public int[] SolvedForRow { get; private set; }
        public int[] SolvedForColumn { get; private set; }

        public Puzzle(string board) : this(ReadPuzzleAsString(board))
        {
        }

        public Puzzle(Memory<int> board)
        {
            _board = board;
            Validate();
        }

        public ReadOnlyMemory<int> Board => _board;

        public bool IsSolved()
        {
            var solved = false;
            if (Solved == TotalsCells && Validate().Solved)
            {
                solved = true;
            }
            else if (Solved == TotalsCells)
            {
                throw new Exception("Something went wrong");
            }
            return solved;
        }

        public bool Solve(ISolver solver)
        {
            var solvers = new List<ISolver>()
            {
                solver
            };

            foreach (var solution in TrySolvers(solvers))
            {
            }
            return IsSolved();
        }

        public bool Solve(IReadOnlyCollection<ISolver> solvers)
        {
            foreach(var solution in TrySolvers(solvers))
            {
            }
            return IsSolved();
        }

        public IEnumerable<Solution> TrySolvers(IReadOnlyCollection<ISolver> solvers)
        {
            var isEffective = true;
            var solverCollection = solvers.ToArray();
            var max = solverCollection.Length -1;
            var index = 0;

            // continue until
            // IsEffective == false for all solvers
            // for solvers that reports IsEffective == true but return no success
            while (isEffective)
            {
                isEffective = false;
                var solver = solverCollection[index];
                if (solver.IsEffective())
                {
                    foreach (var solution in solver.FindSolution())
                    {
                        if (solution.Solved)
                        {
                            Update(solution);
                            isEffective = true;   
                        }
                        yield return solution;
                    }
                }

                if (isEffective && index != 0)
                {
                    index = 0;
                }
                else if (!isEffective && index < max)
                {
                    isEffective = true;
                    index++;
                }
            }
        }
        public PuzzleState Validate()
        {
            if (_board.Length != TotalsCells)
            {
                throw new Exception("Puzzle incorrect length");
            }

            Solved = 0;
            SolvedForBox = new int[9];
            SolvedForRow = new int[9];
            SolvedForColumn = new int[9];

            for (int i = 0; i < 9; i++)
            {
                // Process box i
                var boxSequence = GetBoxAsLine(i);
                var (boxCount, boxState) = ProcessSequence(boxSequence.Segment);
                if (!boxState.Valid) return boxState;
                SolvedForBox[i] = boxCount;

                // Update total count
                // Only count one of box, row or column
                Solved += boxCount;

                // Process row i
                var row = GetRow(i);
                var (rowCount, rowState) = ProcessSequence(row.Segment);
                if (!rowState.Valid) return rowState;
                SolvedForRow[i] = rowCount;

                // Process column i
                var column = GetColumn(i);
                var (columnCount, columnState) = ProcessSequence(column.Segment);
                if (!columnState.Valid) return columnState;
                SolvedForColumn[i] = columnCount;
            }

            return new PuzzleState
            {
                Solved = Solved == TotalsCells,
                Valid = true
            };

            (int solvedCount, PuzzleState puzzleState) ProcessSequence(ReadOnlySpan<int> sequence)
            {
                int count = 0;
                var values = new bool[10];
                for (int i = 0; i < sequence.Length; i++)
                {
                    var value = sequence[i];
                    if (value >= 1 && value <= 9)
                    {
                        if (values[value])
                        {
                            return (0, new PuzzleState
                            {
                                Valid = false,
                                Description = "Character already seen"
                            });
                        }
                        values[value] = true;
                        count++;
                    }
                    else if (value != UnsolvedMarker)
                    {
                        throw new Exception($"Unknown character: {value}");
                    }
                }
                return (count, new PuzzleState
                        {
                            Valid = true
                        });
            }
        }

        public Line GetRow(int row)
        {
            var start = GetOffsetForRow(row);

            return new Line(_board.Slice(start, 9).Span);
        }

        public Line GetColumn(int column)
        {
            var col = new int[9];
            for (int i = 0;  i < 9; i++)
            {
                var cell = column + (i * 9);
                col[i] = _board.Span[cell];
            }
            
            return new Line(col);
        }

        public Box GetBox(int index)
        {
            return new Box(this, index);
        }

        public Line GetBoxAsLine(int index)
        {
            var box = GetBox(index);
            var boxSequence = new int[]
            {
                box.FirstRow[0],
                box.FirstRow[1],
                box.FirstRow[2],
                box.InsideRow[0],
                box.InsideRow[1],
                box.InsideRow[2],
                box.LastRow[0],
                box.LastRow[1],
                box.LastRow[2]
            };
            return new Line(boxSequence);
        }

        public static int GetBoxIndex(int row, int column)
        {
            return (row / 3) * 3 + (column % 3);
        }

        public void Update(Solution solution)
        {
            if (!solution.Solved)
            {
                return;
            }

            UpdateCell(solution.Row, solution.Column, solution.Value);
        }

        private void UpdateCell(int row, int column, int value)
        {
            var puzzle = _board.Span;
            var index = row * 9 + column;
            if (puzzle[index] != 0)
            {
                throw new Exception("Something went wrong! Oops.");    
            }

            puzzle[index] = value;
            Solved++;
            SolvedForRow[row]++;
            SolvedForColumn[column]++;
            var box = GetBoxIndex(row,column);
            SolvedForBox[box]++;
        }

        public static int GetOffsetForBox(int index)
        {
            return (index / 3) * 27 + (index % 3) * 3;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            var board = _board.Span;
            for (int i = 0; i < board.Length; i++)
            {
                var num = board[i];
                char cell = num == 0 ? '.' : (char)('0' + num);
                buffer.Append(cell);
            }
            return buffer.ToString();
        }

        public static (int row, int column) GetLocationForBoxCell(int box, int index)
        {
            var boxStart = Puzzle.GetOffsetForBox(box);
            var boxCell = boxStart + index / 3 * 9 + index % 3;

            var row = boxCell / 9;
            var column = boxCell % 9;
            return (row,column);
        }

        private static int[] GetOffsetForColumn(int index)
        {
            var column = new int[9];
            for (int i = 0; i < 9; i++)
            {
                var cIndex = index + (i * 9);
                column[i] = cIndex;
            }
            return column;
        }

        private static int GetOffsetForRow(int index)
        {
            return index * 9;
        }

        private static int[] ReadPuzzleAsString(string board)
        {
            int[] puzzle = new int[81];
            int index = 0;
            foreach (var cell in board)
            {
                var value = 0;
                if (cell != '.')
                {
                    value = (int)cell - (int)'0';
                }
                puzzle[index] = value;
                index++;
            }
            if (index != 81)
            {
                throw new Exception();
            }
            return puzzle;
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
            WriteLine(puzzle);


            void PrintColumnSolution(Solution solution)
            {
                if (!solution.Solved)
                {
                    return;
                }

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
