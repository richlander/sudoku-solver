using sudoku_solver_extensions;
namespace sudoku_solver;

public class PointingMultiplesCandidateSolver : ICandidateSolver
{
    public bool TryRemoveCandidates(Puzzle puzzle, out Candidates? removalCandidates)
    {
        throw new NotImplementedException();
    }

    public bool TrySolveBox(int index, Puzzle puzzle, out Candidates? removalCandidates)
    {
        Candidates candidates = puzzle.Candidates;
        removalCandidates = new();
        bool results = false;

        ReadOnlySpan<int> boxPositions = Puzzle.GetPositionsForBox(index);

        Box box = puzzle.GetBox(index);
        ReadOnlySpan<int> pointingCandidates;
        int[] neighbors = box.GetNeighbors().ToArray();
        ReadOnlySpan<int> allRows = new int[] {0, 1, 2};
        // iterate over all three rows in the box
        for (int i = 0; i < 3; i++)
        {
            HashSet<int> otherRowSet = new(6 * 9);
            // for each row, iterate over the other two rows
            // they establish the baseline data with their candidates
            // we want to find candidates that are not in the other two rows
            foreach(int row in allRows.Except(i))
            {
                foreach(int position in boxPositions.Slice(row * 3, 3))
                {
                    if (!candidates.Contains(position))
                    {
                        continue;
                    }

                    otherRowSet.AddRange(candidates[position]);
                }
            }

            // determine if there are values in the given row that are not in the other two rows
            // each of the three cells is considered separately
            foreach(int position in boxPositions.Slice(i * 3, 3))
            {
                if (!candidates.Contains(position))
                {
                    continue;
                }

                // values unique within row
                pointingCandidates = candidates[position].Except(otherRowSet);
                if (pointingCandidates.Length == 0)
                {
                    continue;
                }

                // values match with in neighboring rows
                foreach(int neighbor in neighbors)
                {
                    ReadOnlySpan<int> neighborBoxPositions = Puzzle.GetPositionsForBox(neighbor);

                    // get cells for same row
                    foreach(int np in neighborBoxPositions.Slice(i * 3, 3))
                    {
                        if (!candidates.Contains(np))
                        {
                            continue;
                        }

                        ReadOnlySpan<int> neighborCandidates = candidates[np];
                        ReadOnlySpan<int> matches = pointingCandidates.Intersect(neighborCandidates);
                        if (matches.Length > 0)
                        {
                            removalCandidates.AddCandidates(np, matches);
                            results = true;
                        }
                    }
                }
            }
        }

        return results;
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