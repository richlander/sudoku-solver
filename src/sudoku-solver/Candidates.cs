namespace sudoku_solver;

// Exposes candidates for each cell.
// Solvers update candidates per their algorithms.
// Primary data structure is 
public class Candidates
{
    private Puzzle _puzzle;
    private int[][] _candidates = new int[81][];

    public Candidates(Puzzle puzzle)
    {
        _puzzle = puzzle;
    }

    public ReadOnlySpan<int> this[int i] => _candidates[i].AsSpan(1,_candidates[i][0]);

    public void RemoveCandidates(int index, int value)
    {
        int[] candidates = _candidates[index];
        bool found = false;

        if (candidates[0] == 0)
        {
            return;
        }

        for (int i = 1; i <= candidates[0]; i++)
        {
            if (found)
            {
                candidates[i-1] = candidates[i];
            }
            else if (candidates[i] == value)
            {
                found = true;
            }
        }

        if (found)
        {
            candidates[0] = candidates[0] - 1;
        }
    }

    public void Update()
    {
        for (int i = 0; i < 81; i++)
        {
            if (_puzzle[i] != 0)
            {
                continue;
            }

            _candidates[i] = GetCandidates(i);
        }
    }

    private int[] GetCandidates(int index)
    {
        // get row that includes cell
        var rowIndex = Puzzle.GetRowIndexForCell(index);
        var row = _puzzle.GetRow(rowIndex);

        // get column that includes cell
        var columnIndex = Puzzle.GetColumnIndexForCell(index);
        var column = _puzzle.GetColumn(columnIndex);

        // get box that includes cell
        var boxIndex = Puzzle.GetBoxIndexForCell(index);
        var box = _puzzle.GetBox(boxIndex).AsLine();

        bool[] values = new bool[10];
        int[] missingValues = new int[10];

        if (index == 18)
        {

        }

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

        if (count == 0)
        {  
            throw new Exception();
        }

        missingValues[0] = count;
        return missingValues;
    }
}
