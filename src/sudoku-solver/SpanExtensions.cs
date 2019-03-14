using System;

public static class ReadOnlySpanExtensions
{
    public static ReadOnlySpan<int> Intersect(this ReadOnlySpan<int> values1, ReadOnlySpan<int> values2)
    {
        var values = new int[9];
        var count = 0;
        foreach (int value1 in values1)
        {
            if (value1 == 0) continue;
            foreach (int value2 in values2)
            {
                if (value1 == value2)
                {
                    if (count > 0 && values.AsSpan(0..count).Contains(value1))
                    {
                        break;
                    }
                    values[count] = value1;
                    count++;
                    break;
                }
            }
        }
        return values[0..count];
    }

    public static ReadOnlySpan<int> Union(this ReadOnlySpan<int> values1, ReadOnlySpan<int> values2)
    {
        var values = new int[9];
        int count = 0;
        count = ProcessValues(values1, count);
        count = ProcessValues(values2, count);

        return values[0..count];

        int ProcessValues(ReadOnlySpan<int> values0, int count)
        {
            foreach(int value in values0)
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

    public static ReadOnlySpan<int> DisjointSet(this ReadOnlySpan<int> values1, ReadOnlySpan<int> values2)
    {
        var values = new int[9];
        var count = 0;
        foreach (int value1 in values1)
        {
            if (value1 == 0) continue;
            bool inSet = false;
            foreach (int value2 in values2)
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

        return values[0..count];
    }

    public static bool Contains(this ReadOnlySpan<int> values, int value)
    {
        foreach(int v in values)
        {
            if (value == v)
            {
                return true;
            }
        }
        return false;
    }
}
