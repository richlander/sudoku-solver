using sudoku_solver_extensions;
namespace sudoku_solver;

public class PointingMultiplesCandidateSolver : ICandidateSolver
{
    public bool TryFindCandidates(Puzzle puzzle, [NotNullWhen(true)] out Candidates? pointingCandidates)
    {
        bool found = false;
        pointingCandidates = new();

        for (int i = 0; i < 9; i++)
        {
            var result = TrySolveBox(i, puzzle, pointingCandidates);
            found = found | result;
        }
        
        return found;
    }

    public bool TrySolveBox(int index, Puzzle puzzle, Candidates pointingCandidates)
    {
        Candidates candidates = puzzle.Candidates;
        bool candidatesFound = false;

        // Get baseline data about box
        Box box = puzzle.GetBox(index);
        ReadOnlySpan<int> boxPositions = Puzzle.GetPositionsForBox(index);
        int[] horizontalNeighbors = Puzzle.GetBoxIndexesForHorizontalNeighbors(index);
        int[] verticalNeighbors = Puzzle.GetBoxIndexesForVerticalNeighbors(index);

        // Used to pivot on which rows should be checked, later
        ReadOnlySpan<int> allRows = new int[] {0, 1, 2};

        // iterate over all three rows in the box, looking for candidates
        for (int i = 0; i < 3; i++)
        {
            // Need to first validate that the box row has unsolved cells
            if (box.IsRowSolved(i))
                {
                    continue;
                }

            // HashSet is used for the same pattern as LINQ Except
            // https://github.com/dotnet/runtime/blob/03d92024f9924d8b05a96e785da154f1d89d858f/src/libraries/System.Linq/src/System/Linq/Except.cs#L94
            // 54 = 2 rows * 3 cells * 9 possible candidates
            HashSet<int> otherRowSet = new(54);
            // for each row, iterate over the other two rows
            // they establish the baseline data with their candidates
            // goal: find candidates in this row that are not in the other two rows
            foreach(int row in allRows.Except(i))
            {
                // there are (virtually) three rows of positions; pick one per `row`
                foreach(int position in boxPositions.Slice(row * 3, 3))
                {
                    // position will only have candidates if that position is unsolved
                    if (puzzle[position] > 0)
                    {
                        continue;
                    }

                    // Add all candidates for that position to the HashSet
                    otherRowSet.AddRange(candidates[position]);
                }
            }

            // determine if there are values in the given row that are not in the other two rows
            // each of the three cells is considered separately
            foreach(int position in boxPositions.Slice(i * 3, 3))
            {
                // Only looking at unsolved positions
                if (puzzle[position] > 0)
                {
                    continue;
                }

                // values unique within row
                ReadOnlySpan<int> rowCandidates = candidates[position].Except(otherRowSet);
                if (rowCandidates.Length == 0)
                {
                    continue;
                }

                // values match with in neighboring rows
                foreach(int neighbor in horizontalNeighbors)
                {
                    ReadOnlySpan<int> neighborBoxPositions = Puzzle.GetPositionsForBox(neighbor);

                    // get cells in neighbor box for same row
                    foreach(int np in neighborBoxPositions.Slice(i * 3, 3))
                    {
                        // Only looking at unsolved cells
                        if (puzzle[np] > 0)
                        {
                            continue;
                        }

                        ReadOnlySpan<int> neighborCandidates = candidates[np];
                        ReadOnlySpan<int> matches = rowCandidates.Intersect(neighborCandidates);
                        if (matches.Length > 0)
                        {
                            pointingCandidates.AddCandidates(np, matches);
                            candidatesFound = true;
                        }
                    }
                }
            }
        }

        return candidatesFound;
    }
}