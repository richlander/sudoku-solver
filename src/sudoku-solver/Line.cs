using System;

namespace sudoku_solver
{
    public ref struct Line
    {
        public Span<int> Segment;

        public Line(Span<int> segment)
        {
            Segment = segment;
        }

        public int this[int i] => Segment[i];

        public int GetUnsolvedCount()
        {
            int count = 0;

            count += Count(Segment);
            return count;

            int Count(Span<int> row)
            {
                var sum = 0;
                for (int i = 0; i < row.Length; i++)
                {
                    if (row[i] != 0)
                    {
                        sum++;
                    }
                }
                return sum;
            }
        }

        public bool IsJustOneElementUnsolved()
        {
            bool justOne = false;
            for (int i = 0; i < Segment.Length; i++)
            {
                if (Segment[i] == Puzzle.UnsolvedMarker)
                {
                    if (justOne)
                    {
                        return false;
                    }
                    else
                    {
                        justOne = true;
                    }
                }
            }
            return justOne;
        }

        public bool ContainsValue(int value)
        {
            for (int i = 0; i < Segment.Length; i++)
            {
                if (Segment[i] == value)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
