using System;

public ref struct Line
{
    public Span<int> Segment;

    public int GetUnsolved()
    {
        int count = 0;

        count += Count(Segment);
        return count;

        int Count(Span<int> row)
        {
            var count = 0;
            for (int i = 0; i < row.Length; i++)
            {
                if (row[i] != 0)
                {
                    count++;
                }
            }
            return count;
        }
    }
}