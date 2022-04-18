namespace sudoku_solver;

public partial class Puzzle
{
    [MemberNotNull(nameof(_candidates))]
    // Update counts that are maintained for puzzle solving
    private void UpdateCounts()
    {
        // Solved counts
        Solved = 0;
        SolvedForBox = new int[9];
        SolvedForRow = new int[9];
        SolvedForColumn = new int[9];

        // Candidates data
        Dictionary<int, int[]> candy = new();

        // Iterate for 9 rows
        for (int i = 0; i < 9; i++)
        {
            // Process row i
            Line row = GetRow(i);
            SolvedForRow[i] = row.CountValidSolved();
            // Process column i
            Line column = GetColumn(i);
            SolvedForColumn[i] = column.CountValidSolved();
            // Process box i
            Box box = GetBox(i);
            SolvedForBox[i] = box.CountValidSolved();
            Solved += SolvedForBox[i];

            // Iterate for each cell in row
            for (int j = 0; j < 9; j++)
            {
                int cellIndex = i * 9 + j;

                if (this[cellIndex] == 0)
                {
                    continue;
                }

                // Optimization to avoid asking for box when not needed
                if (j % 3 == 0)
                {
                    int boxIndex = GetBoxIndexForCell(cellIndex);
                    box = GetBox(boxIndex);
                }

                column = GetColumn(j);

                bool[] values = new bool[10];
                int[] missingValues = new int[10];

                for (int k = 0; k < 9; k++)
                {
                    values[row[k]] = true;
                    values[column[k]] = true;
                    values[box[k]] = true;
                }

                int count = 0;
                for (int k = 1; k < 10; k++)
                {
                    if (!values[k])
                    {
                        count++;
                        missingValues[count] = i;
                    }
                }

                if (count == 0)
                {
                    continue;
                }

                missingValues[0] = count;
                candy.Add(cellIndex, missingValues);
            }
        }

        _candidates = new(candy);
        InitialSolved = Solved;
    }
}