using System;
public ref struct Box
{
    public Span<int> FirstRow;
    public Span<int> InsideRow;
    public Span<int> LastRow;

    public int GetUnsolvedCount()
    {
        int count = 0;

        count += Count(FirstRow);
        count += Count(InsideRow);
        count += Count(LastRow);
        return count;

        int Count(Span<int> row)
        {
            var sum = 0;
            for (int i = 0;i <row.Length; i++)
            {
                if (row[i] != 0)
                {
                    sum++;
                }
            }
            return sum;
        }
    }
}