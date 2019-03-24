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

        public ReadOnlySpan<int> Union(Line line)
        {
            return Line.Union(this.Segment,line.Segment);
        }

        public ReadOnlySpan<int> DisjointSet(Line line)
        {
            return Line.DisjointSet(this.Segment, line.Segment);
        }

        public static ReadOnlySpan<int> Union(ReadOnlySpan<int> values1, ReadOnlySpan<int> values2)
        {
            var values = new int[9];
            var count = 0;
            foreach(int value1 in values1)
            {
                foreach(int value2 in values2)
                {
                    if (value1 == value2)
                    {
                        values[count] = value1;
                        count++;
                        break;
                    }
                }
            }

            ReadOnlySpan<int> union = values;
            return union[0..count];
        }

        public static ReadOnlySpan<int> DisjointSet(ReadOnlySpan<int> values1, ReadOnlySpan<int> values2)
        {
            var values = new int[9];
            var count = 0;
            foreach(int value1 in values1)
            {
                bool inSet = false;
                foreach(int value2 in values2)
                {
                    if (value1 == value2)
                    {
                        inSet = true;
                        break;
                    }
                }
                if (!inSet)
                {
                    values[count] = value1;
                    count++;
                }
            }

            ReadOnlySpan<int> union = values;
            return union[0..count];
        }
    }
}

public static class Extensions
{
    public static ReadOnlySpan<int> AsRich(this ReadOnlySpan<int> foo)
    {
        return foo;
    }
}
