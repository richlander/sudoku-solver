using System;

namespace sudoku_solver
{
    public ref struct Box
    {
        private Puzzle _puzzle;
        private Span<int> _segment;
        private int _offset;
        private int _index;

        public Box(Puzzle puzzle, int index)
        {
            _puzzle = puzzle;
            _segment = puzzle.s;
            _offset = GetOffset(index);
            _index = index;
        }

        public Line FirstRow => new Line(_segment.Slice(_offset, 3));
        public Line InsideRow => new Line(_segment.Slice(_offset + 9, 3));
        public Line LastRow => new Line(_segment.Slice(_offset + 18, 3));
        public Line FirstColumn => GetColumnSegment(0);
        public Line InsideColumn => GetColumnSegment(1);
        public Line LastColumn => GetColumnSegment(2);

        public int GetUnsolvedCount()
        {
            int count = 0;

            count += Count(FirstRow);
            count += Count(InsideRow);
            count += Count(LastRow);
            return count;

            int Count(Line line)
            {
                var sum = 0;
                for (int i = 0;i <line.Segment.Length; i++)
                {
                    if (line[i] != 0)
                    {
                        sum++;
                    }
                }
                return sum;
            }
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
            return (index / 3) * 27 + (index % 3) * 3;
        }

        private Line GetColumnSegment(int index)
        {
            var offset = GetOffset(_index) + index;

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