using System;

namespace sudoku_solver
{
    public ref struct Line
    {
        public Span<int> Segment;

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
    }
}
