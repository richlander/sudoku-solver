using System;

namespace sudoku_solver
{
    public ref struct Box
    {
        public Line FirstRow;
        public Line InsideRow;
        public Line LastRow;

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
    }
}