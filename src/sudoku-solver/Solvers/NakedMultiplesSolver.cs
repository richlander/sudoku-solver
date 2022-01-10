using sudoku_solver_extensions;
namespace sudoku_solver;

// Naked multiples: cells with matching candidates in a unit (like two cells in a box with only 5 and 9 and candidates)
// remove those candidates for all other cells in the unit. 

// Example:
// Solved cell: r2:c7; 7
// Solved by: NakedMultiplesSolver
//                 *
// 7 9 4 | 5 8 2 | 1 3 6
// 2 6 0 | 0 3 1 | 7 0 0*
// 0 0 0 | 0 7 6 | 0 0 2
// ------+-------+------
// 6 8 0 | 7 1 0 | 3 2 4
// 4 3 2 | 8 6 0 | 0 0 0
// 0 0 7 | 2 4 3 | 8 6 0
// ------+-------+------
// 0 2 0 | 6 5 7 | 4 0 3
// 0 4 0 | 1 2 8 | 6 0 7
// 0 7 6 | 3 9 4 | 2 0 8
public class NakedMultiplesSolver : ISolver
{
    Puzzle _puzzle;
    Candidates _candidates;

    public bool TrySolve(Puzzle puzzle, out Solution? solution)
    {
        _puzzle = puzzle;
        _candidates = puzzle.Candidates;
        int[] positions;
        int[]? multiples;

        if (_candidates is null)
        {
            solution = null;
            return false;
        }

        for (int i = 0; i < 9; i++)
        {
            if (TrySolveBox(i, out multiples, out positions) ||
                TrySolveRow(i, out multiples, out positions) ||
                TrySolveColumn(i, out multiples, out positions))
            {
                foreach (int position in positions)
                {
                    if (_puzzle[position] != 0)
                    {
                        continue;
                    }

                    ReadOnlySpan<int> candidates = _candidates[position];
                    var remainder = candidates.DisjointSet(multiples);
                    if (remainder.Length == 1)
                    {
                        
                        solution = new Solution(
                            Puzzle.GetRowIndexForCell(position),
                            Puzzle.GetColumnIndexForCell(position),
                            remainder[0],
                            nameof(NakedMultiplesSolver),
                            string.Empty
                        );
                        return true;
                    }
                }
            }
        }

        solution = null;
        return false;
    }

    private bool TrySolveBox(int index, out int[]? multiples, out int[] positions)
    {
        positions = Puzzle.GetPositionsForBox(index);
        return GetMultiplesForUnit(positions, out multiples);
    }

    private bool TrySolveRow(int index, out int[]? multiples, out int[] positions)
    {
        positions = Puzzle.GetPositionsForRow(index);
        return GetMultiplesForUnit(positions, out multiples);
    }

    private bool TrySolveColumn(int index, out int[]? multiples, out int[] positions)
    {
        positions = Puzzle.GetPositionsForColumn(index);
        return GetMultiplesForUnit(positions, out multiples);
    }

    private bool GetMultiplesForUnit(int[] positions, out int[]? multiples)
    {
        var pairs = new int[18];
        var triples = new int[27];
        var pairIndex = 0;
        var triplesIndex = 0;
        multiples = null;

        foreach(int position in positions)
        {
            if (_puzzle[position] != 0)
            {
                continue;
            }

            var candidates = _candidates[position];
            // TODO: Better approach for comparing multiples
            if (candidates.Length == 2)
            {
                    pairs[pairIndex++] = candidates[0];
                    pairs[pairIndex++] = candidates[1];
            }
            else if (candidates.Length == 3)
            {
                    triples[triplesIndex++] = candidates[0];
                    triples[triplesIndex++] = candidates[1];
                    triples[triplesIndex++] = candidates[2];
            }
        }

        var index = 2;
        while (index <= pairIndex)
        {
            var innerIndex = index;
            while (innerIndex <= pairIndex)
            {
                if (pairs[index] == pairs[index-2] && 
                    pairs[index+1] == pairs[index-1])   
                {

                    multiples = new int[]{pairs[index], pairs[index+1]};
                    return true;
                }
                innerIndex +=2;
            }
            index+=2;
        }

        return false;
    }
}
