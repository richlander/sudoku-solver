using System;
using System.Collections.Generic;
using System.Text;

namespace sudoku_solver
{
    public class Puzzle
    {
        private Memory<int> _board;
        public static readonly int UnsolvedMarker = 0;
        public const int TotalsCells = 81;
        public int InitialSolved {get; private set;}
        public int Solved {get; private set;}
        public int[] SolvedForBox {get; private set;}
        public int[] SolvedForRow {get; private set;}
        public int[] SolvedForColumn {get; private set;}

        public Puzzle(string board) : this(ReadPuzzleAsString(board))
        {
        }

        public Puzzle(Memory<int> board)
        {
            if (board.Length != TotalsCells)
            {
                throw new Exception("Puzzle incorrect length");
            }

            _board = board;
            if (!IsValid())
            {
                throw new Exception("Puzzle is not valid.");
            }

            UpdateCounts();
        }

        public ReadOnlyMemory<int> Board => _board;

        public IEnumerable<ISolver> Solvers {get; set;}

        public void AddSolver(ISolver solver)
        {
            Solvers = new List<ISolver>()
            {
                solver
            };
        }

        // Approach chosen is to collect the first solution from the solvers
        // then reset to first (assumed to be cheapest/simplest) solver.
        public bool TrySolve(out Solution solution)
        {
            foreach(ISolver solver in Solvers)
            {
                if (solver.TrySolve(this, out solution))
                {
                    return true;
                }
            }
     
            solution = null;
            return false;
        }

        public bool Solve()
        {
            if (TrySolve(out Solution solution))
            {
                Update(solution);
                return true;
            }

            return false;
        }

        public bool SolvePuzzle()
        {
            while(Solve())
            {
            }

            return IsSolved;
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

        public Box GetBox(int index) => new Box(this, index);

        public Line GetBoxAsLine(int index)
        {
            var box = GetBox(index);
            return new Line(new int[]
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
            });
        }

        public static int GetBoxIndex(int row, int column) => (row / 3) * 3 + (column % 3);

        public bool IsSolved => Solved == TotalsCells;

        public bool IsValid()
        {
            for (int i = 0; i < 9; i++)
            {
                if (GetBox(i).CountValidSolved() == -1 ||
                    GetRow(i).CountValidSolved() == -1 ||
                    GetColumn(i).CountValidSolved() == -1)
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdateCounts()
        {
            Solved = 0;
            SolvedForBox = new int[9];
            SolvedForRow = new int[9];
            SolvedForColumn = new int[9];

            for (int i = 0; i < 9; i++)
            {
                // Process box i
                SolvedForBox[i] = GetBox(i).CountValidSolved();
                Solved += SolvedForBox[i];
                // Process row i
                SolvedForRow[i] = GetRow(i).CountValidSolved();
                // Process column i
                SolvedForColumn[i] = GetColumn(i).CountValidSolved();
            }

            InitialSolved = Solved;
        }

        public bool Update(Solution solution)
        {
            int row = solution.Row;
            int column = solution.Column;
            int value = solution.Value;
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
            int box = GetBoxIndex(row,column);
            SolvedForBox[box]++;

            return GetBox(box).CountValidSolved() != -1 &&
                GetColumn(column).CountValidSolved() != -1 &&
                GetRow(row).CountValidSolved() != -1;
        }

        public static int GetOffsetForBox(int index) => (index / 3) * 27 + (index % 3) * 3;

        public override string ToString()
        {
            var buffer = new StringBuilder();
            foreach(int num in _board.Span)
            {
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

        private static int GetOffsetForRow(int index) => index * 9;

        private static int[] ReadPuzzleAsString(string board)
        {
            if (board.Length != 81)
            {
                throw new ArgumentException("Board must be 81 characters long.",nameof(board));
            }

            int[] puzzle = new int[81];
            for(int i = 0; i < 81; i++)
            {
                puzzle[i] = board[i] == '.' ? 0 : (int)board[i] - (int)'0';
            }
            return puzzle;
        }
    }
}
