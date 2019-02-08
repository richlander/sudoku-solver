using System;
using System.Collections.Generic;

namespace sudoku_solver
{
    public class Puzzle
    {
        private Memory<int> _puzzle;
        public static readonly int UnsolvedMarker = 0;
        public int TotalsCells = 81;
        private Puzzle(Memory<int> puzzle)
        {
            _puzzle = puzzle;
        }

        private int Solved {get; set;}
        private int UnSolvedcells => TotalsCells - Solved;
        public int[] SolvedForBox {get; private set;}
        public int[] SolvedForRow { get; private set; }
        public int[] SolvedForColumn { get; private set; }

        public static Puzzle ReadPuzzle(Memory<int> puzzle)
        {
            var p = new Puzzle(puzzle);
            p.Validate();
            return p;
        }

        public bool IsSolved()
        {
            if (Solved == TotalsCells && Validate().Solved)
            {
                return true;
            }
            else if (Solved == TotalsCells)
            {
                throw new Exception("Something went wrong");
            }
            return false;
        }

        public IEnumerable<(Solution Solution, int attempts)> TrySolvers(IReadOnlyCollection<ISolver> solvers)
        {
            var solved = true;
            var attempts = 0;
            while (solved)
            {
                (var solution, var attempts_) = Solve(solvers);
                solved = solution.Solved;
                attempts += attempts_;
                if (solved)
                {
                    Update(solution);
                    yield return (solution, attempts);
                    attempts = 0;
                }
            }

            yield return (new Solution{Solved=false}, attempts);

            (Solution solution, int attempts) Solve(IReadOnlyCollection<ISolver> solverCollection)
            {
                var attempts = 0;
                foreach (var solver in solverCollection)
                {
                    if (!solver.IsEffective())
                    {
                        continue;
                    }
                    foreach (var solution in solver.FindSolution())
                    {
                        attempts++;
                        if (solution.Solved)
                        {
                            return (solution, attempts);
                        }
                    }
                }
                return (new Solution{Solved=false}, attempts);
            }
        }
        private PuzzleState Validate()
        {
            if (_puzzle.Length != TotalsCells)
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

            (int solvedCount, PuzzleState puzzleState) ProcessSequence(Span<int> sequence)
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

            return new Line(_puzzle.Slice(start, 9).Span);
        }

        public Line GetColumn(int column)
        {
            var col = new int[9];
            for (int i = 0;  i < 9; i++)
            {
                var cell = column + (i * 9);
                col[i] = _puzzle.Span[cell];
            }
            
            return new Line(col);
        }

        public Box GetBox(int index)
        {
            int start = GetOffsetForBox(index);

            var box = new Box
            {
                FirstRow = new Line(_puzzle.Slice(start, 3).Span),
                InsideRow = new Line(_puzzle.Slice(start + 9, 3).Span),
                LastRow = new Line(_puzzle.Slice(start + 18, 3).Span)
            };

            return box;
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
            var puzzle = _puzzle.Span;
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
    }
}
