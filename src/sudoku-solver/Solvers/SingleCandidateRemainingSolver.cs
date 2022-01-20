using sudoku_solver;

public class SingleCandidateRemainingSolver : ISolver
{
    public bool TrySolve(Puzzle puzzle, [NotNullWhen(true)] out Solution? solution)
    {
        Candidates candidates = puzzle.Candidates;

        for (int i = 0; i < 81; i++)
        {

            if (puzzle[i] > 0)
            {
                continue;
            }

            if (candidates[i].Length != 1)
            {
                continue;
            }

            int row = Puzzle.GetRowIndexForCell(i);
            int column = Puzzle.GetColumnIndexForCell(i);
            int value = candidates[i][0];
            solution = new(
                row,
                column,
                value,
                nameof(SingleCandidateRemainingSolver),
                string.Empty
            );
            return true;
        }

        solution = null;
        return false;
    }
}
