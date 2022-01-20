using sudoku_solver_extensions;

namespace sudoku_solver;

public ref struct Line
{
    public ReadOnlySpan<int> Segment;

    public Line(ReadOnlySpan<int> segment) => Segment = segment;

    public int this[int i] => Segment[i];

    public int Length => Segment.Length;

    public bool HasUnsolved => ContainsValue(0);

    public int GetUnsolvedCount()
    {
        int count = 0;
        for (int i = 0; i < Segment.Length; i++)
        {
            if (Segment[i] == Puzzle.UnsolvedMarker)
            {
                count++;
            }
        }
        return count;
    }

    public int CountValidSolved()
    {
        int count = 0;
        var values = new bool[10];
        for (int i = 0; i < Segment.Length; i++)
        {
            int value = Segment[i];
            if (value is >= 1 and <= 9)
            {
                if (values[value])
                {
                    return -1;
                }
                values[value] = true;
                count++;
            }
            else if (value != Puzzle.UnsolvedMarker)
            {
                throw new Exception($"Puzzle is invalid. Contains illegal charater: {value}.");
            }
        }
        return count;
    }

    public bool IsJustOneElementUnsolved(out int index)
    {
        bool justOne = false;
        index = 0;
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
                    index = i;
                    justOne = true;
                }
            }
        }
        return true;
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

    public ReadOnlySpan<int> Except(Line line) => Segment.Except(line.Segment);

    public ReadOnlySpan<int> Intersect(Line line) => Segment.Intersect(line.Segment);

    public ReadOnlySpan<int> DisjointSet(Line line) => Segment.DisjointSet(line.Segment);

    public ReadOnlySpan<int> Union(Line line) => Segment.Union(line.Segment);

    // assumes lines are of the same length
    public static ReadOnlySpan<int> FindMissingValues(Line line1, Line line2)
    {
        bool[] values = new bool[10];
        int[] missingValues = new int[9];
        int length = -1;

        for (int i = 0; i < line1.Length; i++)
        {
            Update(line1[i]);
            Update(line2[i]);
        }

        for (int i = 1; i < 10; i++)
        {
            if (!values[i])
            {
                length++;
                missingValues[length] = i;
            }
        }

        if (length == -1)
        {
            length = 0;
        }
        else if (length == 0)
        {
            length = 1;
        }

        return missingValues.AsSpan().Slice(0,length);

        void Update(int value)
        {
            if (value != 0 && !values[value])
            {
                values[value] = true;
            }
        }
    }

    public ReadOnlySpan<int>.Enumerator GetEnumerator() => Segment.GetEnumerator();

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
