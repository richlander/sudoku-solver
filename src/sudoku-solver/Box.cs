using System;

namespace sudoku_solver
{
    public ref struct Box
    {

        public Line FirstRow;
        public Line InsideRow;
        public Line LastRow;
        public Line FirstColumn;
        public Line InsideColumn;
        public Line LastColumn;

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

        public Line AsLine()
        {
            var boxSequence = new int[]
            {
                FirstRow[0],
                FirstRow[1],
                FirstRow[2],
                InsideRow[0],
                InsideRow[1],
                InsideRow[2],
                LastRow[0],
                LastRow[1],
                LastRow[2]
            };
            return new Line(boxSequence);
        }
    }
}