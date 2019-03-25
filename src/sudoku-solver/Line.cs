using System;

namespace sudoku_solver
{
    public ref struct Line
    {
        public ReadOnlySpan<int> Segment;

        public Line(ReadOnlySpan<int> segment)
        {
            Segment = segment;
        }

        public int this[int i] => Segment[i];

        public int GetUnsolvedCount()
        {
            int count = 0;

            count += Count(Segment);
            return count;

            int Count(ReadOnlySpan<int> row)
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

        public (bool justOne, int index) IsJustOneElementUnsolved()
        {
            bool justOne = false;
            int index = 0;
            for (int i = 0; i < Segment.Length; i++)
            {
                if (Segment[i] == Puzzle.UnsolvedMarker)
                {
                    if (justOne)
                    {
                        return (false, 0);
                    }
                    else
                    {
                        index = i;
                        justOne = true;
                    }
                }
            }
            return (justOne, index);
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

        public bool[] GetValues()
        {
            var values = new bool[9];
            for (int i = 0; i < Segment.Length; i++)
            {
                values[Segment[i]] = true;
            }
            return values;
        }

        public ReadOnlySpan<int> Intersect(Line line)
        {
            return Segment.Intersect(line.Segment);
        }

        public ReadOnlySpan<int> DisjointSet(Line line)
        {
            return Segment.DisjointSet(line.Segment);
        }

        public ReadOnlySpan<int> Union(Line line)
        {
            return Segment.Union(line.Segment);
        }
    }
}
