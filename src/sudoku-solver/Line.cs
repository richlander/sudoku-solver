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

        public int Length => Segment.Length;

        public int GetUnsolvedCount()
        {
            int count = 0;

            for (int i = 0; i < Segment.Length; i++)
            {
                if (Segment[i] == 0)
                {
                    count++;
                }
            }
            return count;
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

        public ReadOnlySpan<int>.Enumerator GetEnumerator()
        {
            return Segment.GetEnumerator();
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
            var values = new bool[10];
            for (int i = 0; i < Segment.Length; i++)
            {
                values[Segment[i]] = true;
            }
            return values;
        }

        public ReadOnlySpan<int> GetMissingValues()
        {
            var missingValues = new int[9];
            var values = GetValues();
            var index = 0;
            for (int i = 0; i < 10; i++)
            {
                if (!values[i])
                {
                    missingValues[index] = i;
                    index++;
                }
            }
            return missingValues.AsSpan().Slice(0,index);
        }

        public ReadOnlySpan<int> GetMissingIndices()
        {
            var indices = new int[9];
            var index = 0;
            for (int i = 0; i < Segment.Length; i++)
            {
                if (Segment[i] == 0)
                {
                    index++;
                    indices[index] = i;
                }
            }
            return indices.AsSpan().Slice(0,Math.Max(0, index));
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
