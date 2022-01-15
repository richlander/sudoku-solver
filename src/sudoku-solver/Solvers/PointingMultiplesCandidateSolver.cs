using sudoku_solver_extensions;
namespace sudoku_solver;

public class PointingMultiplesCandidateSolver : ICandidateSolver
{
    public bool TryRemoveCandidates(Puzzle puzzle, out Candidates? removalCandidates)
    {
        throw new NotImplementedException();
    }

    public bool TrySolveBox(int index, Puzzle puzzle, out Candidates? pointingCandidates)
    {
        Candidates candidates = puzzle.Candidates;
        pointingCandidates = new();
        bool candidatesFound = false;

        // Get baseline data about box
        ReadOnlySpan<int> boxPositions = Puzzle.GetPositionsForBox(index);
        int[] horizontalNeighbors = Puzzle.GetBoxIndexesForHorizontalNeighbors(index);
        int[] verticalNeighbors = Puzzle.GetBoxIndexesForVerticalNeighbors(index);

        // Used to pivot on which rows should be checked, later
        ReadOnlySpan<int> allRows = new int[] {0, 1, 2};

        // iterate over all three rows in the box, looking for candidates
        for (int i = 0; i < 3; i++)
        {
            if (candidatesFound)
            {
                break;
            }

            // Need to first validate that the box row has unsolved cells
            int rowStartingCell = i * 3;
            if (puzzle[rowStartingCell] > 0 &&
                puzzle[rowStartingCell + 1] > 0 &&
                puzzle[rowStartingCell + 2] > 0)
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

    public bool TrySolveRows(int index, Puzzle puzzle, Box box, ReadOnlySpan<int> neighbors)
    {
        HashSet<int> set = new(6);
        ReadOnlySpan<int> allRows = new int[] {1, 2, 3};
        foreach(int i in allRows.Except(index))
        {
            set.AddRange(box.GetRow(i));
        }

        ReadOnlySpan<int> indexRow = box.GetRow(index);
        ReadOnlySpan<int> distinctInBox = indexRow.Except(set);

        if (distinctInBox.Length == 0)
        {
            return false;
        }

        foreach(int neighbor in neighbors)
        {
            Box nb = new(puzzle, neighbor);
            ReadOnlySpan<int> neighborRow = nb.Get
            ReadOnlySpan<int> matchedValues = indexRow.Intersect(neighborRow);
            if (matchedValues.Length > 0)
            {
                foreach(int value in matchedValues)
                {
                    Line l = new Line(neighborRow);
                    l.GetMissingIndices()
                }
            }
        }

        Box n1 = puzzle.GetBox(neighbors[0]);
        Box n2 = puzzle.GetBox(neighbors[1]);

        n1.GetRow
        
        set.Clear();
        set.AddRange(n1.GetRow(index));
        set.AddRange(n2.GetRow(index));

        ReadOnlySpan<int> distinctInRow = indexRow.Except(set);

        if (distinctInRow.Length == 0)
        {
            return false;
        }


        candidates = distinctInRow;
        return true;
    }

}