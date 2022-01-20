namespace sudoku_solver;

public class BasicCandidatesSolver : ICandidateSolver
{
    public bool TryFindCandidates(Puzzle puzzle, [NotNullWhen(true)] out Candidates? candidates)
    {
        Dictionary<int, int[]> candy = new();
        for (int i = 0; i < 81; i++)
        {
            if (puzzle[i] != 0)
            {
                continue;
            }

            candy.Add(i, GetCandidates(i, puzzle));
        }

        if (candy.Count > 0)
        {
            candidates = new Candidates(candy);
            return true;
        }

        candidates = null;
        return false;
    }

    private int[] GetCandidates(int index, Puzzle puzzle)
    {
        // get row that includes cell
        var rowIndex = Puzzle.GetRowIndexForCell(index);
        var row = puzzle.GetRow(rowIndex);

        // get column that includes cell
        var columnIndex = Puzzle.GetColumnIndexForCell(index);
        var column = puzzle.GetColumn(columnIndex);

        // get box that includes cell
        var boxIndex = Puzzle.GetBoxIndexForCell(index);
        var box = puzzle.GetBox(boxIndex);

        bool[] values = new bool[10];
        int[] missingValues = new int[10];

        for (int i = 0; i < 9; i++)
        {
            values[row[i]] = true;
            values[column[i]] = true;
            values[box[i]] = true;
        }

        int count = 0;
        for (int i = 1; i < 10; i++)
        {
            if (!values[i])
            {
                count++;
                missingValues[count] = i;
            }
        }

        missingValues[0] = count;
        return missingValues;
    }
}