using System;
using System.Diagnostics;

namespace sudoku_solver
{
    public ref struct Box
    {
        private Puzzle _puzzle;
        private ReadOnlySpan<int> _segment;
        private int _offset;
        private int _index;

        public Box(Puzzle puzzle, int index)
        {
            _puzzle = puzzle;
            _segment = puzzle.Board.Span;
            _offset = GetOffset(index);
            _index = index;
        }

        public Line FirstRow => GetRow(0);
        public Line InsideRow => GetRow(1);
        public Line LastRow => GetRow(2);
        public Line FirstColumn => GetColumn(0);
        public Line InsideColumn => GetColumn(1);
        public Line LastColumn => GetColumn(2);

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

        private static int GetOffset(int index)
        {
            var offset = (index / 3) * 27 + (index % 3) * 3;
            if (offset > 80)
            {
                Debugger.Break();
            }
            return offset;
        }

        public Line GetRow(int index)
        {
            return new Line(_segment.Slice(_offset + (9 * index), 3));
        }
        
        public Line GetColumn(int index)
        {
            var offset = _offset + index;

            return new Line
            {
                Segment = new int[] 
                {
                    _segment[offset],
                    _segment[offset + 9],
                    _segment[offset + 18]
                }
            };
        }
    }
}