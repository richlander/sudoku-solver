using System;
using System.Diagnostics;

namespace sudoku_solver
{
    public ref struct Box
    {
        private Puzzle _puzzle;
        private ReadOnlySpan<int> _board;
        private int _cellOffset;
        private int _rowOffset;
        private int _index;

        public Box(Puzzle puzzle, int index)
        {
            _puzzle = puzzle;
            _board = puzzle.Board.Span;
            _cellOffset = GetCellOffset(index);
            _rowOffset = (index / 3) * 3;
            _index = index;
        }

        public Line FirstRow => GetRow(0);
        public Line InsideRow => GetRow(1);
        public Line LastRow => GetRow(2);
        public Line FirstColumn => GetColumn(0);
        public Line InsideColumn => GetColumn(1);
        public Line LastColumn => GetColumn(2);
        public Box FirstHorizontalNeighbor => _puzzle.GetBox((_index + 1) % 3 + _rowOffset);
        public Box SecondHorizontalNeighbor => _puzzle.GetBox((_index + 2) % 3 + _rowOffset);
        public Box FirstVerticalNeighbor => _puzzle.GetBox((_index + 3) % 9);
        public Box SecondVerticalNeighbor => _puzzle.GetBox((_index + 6) % 9);


        public int GetUnsolvedCount()
        {
            return 
                FirstRow.GetUnsolvedCount() +
                InsideRow.GetUnsolvedCount() +
                LastRow.GetUnsolvedCount();
        }

        public bool[] GetValues()
        {
            var values = new bool[10];
            for(int i = 0; i < 9; i++)
            {
                int value;
                if (i < 3)
                {
                    value = FirstRow[i];
                }
                else if (i <6)
                {
                    value = InsideRow[i-3];
                }
                else
                {
                    value = LastRow[i-6];
                }

                if (values[value] && value != 0)
                {
                    throw new Exception("Something went wrong");
                }
                else
                {
                    values[value] = true;
                }
            }
            return values;
        }

        public Line AsLine()
        {
            var boxSequence = new int[]
            {
                FirstRow[0],
                FirstRow[1],
                FirstRow[2],
                InsideRow[0],
                InsideRow[1],
                InsideRow[2],
                LastRow[0],
                LastRow[1],
                LastRow[2]
            };
            return new Line(boxSequence);
        }

        public ReadOnlySpan<int> AsValues()
        {
            var values = new int[9];
            int count = 0;
            count = ProcessValues(FirstRow.Segment, count);
            count = ProcessValues(InsideRow.Segment, count);
            count = ProcessValues(LastRow.Segment, count);

            return values[0..count];

            int ProcessValues(ReadOnlySpan<int> values0, int count)
            {
                foreach (int value in values0)
                {
                    if (value == 0 || values.AsSpan(0..count).Contains(value))
                    {
                        continue;
                    }
                    values[count] = value;
                    count++;
                }
                return count;
            }
        }

        public Line GetRow(int index)
        {
            return new Line(_board.Slice(_cellOffset + (9 * index), 3));
        }
        
        public Line GetColumn(int index)
        {
            var offset = _cellOffset + index;

            return new Line
            {
                Segment = new int[] 
                {
                    _board[offset],
                    _board[offset + 9],
                    _board[offset + 18]
                }
            };
        }

        public int GetOffsetForBox()
        {
            return (_index / 3) * 27 + (_index % 3) * 3;
        }

        public int GetRowOffsetForBox()
        {
            return (_index / 3) * 3;
        }

        public int GetColumnOffsetForBox()
        {
            return (_index % 3) * 3;
        }

        private static int GetCellOffset(int index)
        {
            var offset = (index / 3) * 27 + (index % 3) * 3;
            if (offset > 80)
            {
                Debugger.Break();
            }
            return offset;
        }
    }
}