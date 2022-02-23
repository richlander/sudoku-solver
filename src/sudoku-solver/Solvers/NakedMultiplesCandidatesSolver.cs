using sudoku_solver_extensions;

namespace sudoku_solver;

public class NakedMultiplesCandidatesSolver : ICandidateSolver
{
    public bool TryFindCandidates(Puzzle puzzle, [NotNullWhen(true)] out Candidates? nakedMultiplesCandidates)
    {
        bool candidatesFound = false;
        nakedMultiplesCandidates = new();
        int solvedLimit = 8;
        for (int i = 0; i < 9; i++)
        {
            if (puzzle.SolvedForBox[i] < solvedLimit)
            {
                ReadOnlySpan<int> boxPositions = Puzzle.GetPositionsForBox(i);
                candidatesFound |= GetMultiplesForUnit(boxPositions, puzzle, nakedMultiplesCandidates);
            }

            if (puzzle.SolvedForRow[i] < solvedLimit)
            {
                ReadOnlySpan<int> rowPositions = Puzzle.GetPositionsForRow(i);
                candidatesFound |= GetMultiplesForUnit(rowPositions, puzzle, nakedMultiplesCandidates);
            }
            
            if (puzzle.SolvedForColumn[i] < solvedLimit)
            {
                ReadOnlySpan<int> columnPositions = Puzzle.GetPositionsForColumn(i);
                candidatesFound |= GetMultiplesForUnit(columnPositions, puzzle, nakedMultiplesCandidates);
            }
        }

        return candidatesFound;
    }

    private bool GetMultiplesForUnit(ReadOnlySpan<int> positions, Puzzle puzzle, Candidates nakedMultiplesCandidates)
    {
        bool candidatesFound = false;
        int[] positionsToConsider = new int[10];
        Dictionary<int, int[]> matches = new();

        // find potential multiples
        foreach(int position in positions)
        {
            if (puzzle[position] != 0)
            {
                continue;
            }

            var posCandidates = puzzle.Candidates[position];
            int match = 1;
            int matchSum = 0;
            if (posCandidates.Length is 2 or 3)
            {
                int[] matchData = new int[posCandidates.Length + 1];
                for (int i = 0; i < posCandidates.Length; i++)
                {
                    match *= posCandidates[i];
                    matchSum += posCandidates[i];
                    matchData[i + 1] = posCandidates[i];
                }

                match += matchSum;

                if (matches.ContainsKey(match))
                {
                    matches[match][0]++;

                    if (posCandidates.Length == 2)
                    {
                        continue;
                    }
                }
                else
                {
                    matches.Add(match, matchData);
                }
            }

            if (posCandidates.Length > 1)
            {
                positionsToConsider[0]++;
                int count = positionsToConsider[0];
                positionsToConsider[count] = position;
            }
        }

        if (matches.Count == 0 ||
            positionsToConsider[0] == 0)
        {
            return false;
        }
        
        // remove multiple candidates with no multiples
        foreach (int key in matches.Keys)
        {
            if (matches[key][0] == 0)
            {
            }
            else if (matches[key][0] == matches[key].Length - 2)
            {
                continue;
            }
            
            matches.Remove(key);
        }

        // iterate through positions again to find removal candidates

        foreach(int position in positionsToConsider.AsSpan().Slice(1, positionsToConsider[0]))
        {
            ReadOnlySpan<int> posCandidates = puzzle.Candidates[position];

            foreach (ReadOnlySpan<int> match in matches.Values)
            {
                ReadOnlySpan<int> intersection = posCandidates.Intersect(match.Slice(1));
                if (posCandidates.Length == intersection.Length)
                {
                    continue;
                }
                else if (intersection.Length > 0)
                {
                    nakedMultiplesCandidates.UpdateAddCandidates(position, intersection);
                    candidatesFound = true;
                }
            }
        }

        return candidatesFound;
    }
}
