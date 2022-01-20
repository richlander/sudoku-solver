using sudoku_solver_extensions;
namespace sudoku_solver;

// Exposes candidates for each cell.
// Solvers update candidates per their algorithms.
// Primary data structure is 
public class Candidates
{
    private Dictionary<int, int[]> _candidates = new Dictionary<int, int[]>();

    public Candidates(Dictionary<int, int[]> candidates)
    {
        _candidates = candidates;
    }

    public Candidates()
    {
        _candidates = new Dictionary<int, int[]>();
    }

    public ReadOnlySpan<int> this[int i] => _candidates[i].AsSpan(1,_candidates[i][0]);

    public bool Contains(int index) => _candidates.ContainsKey(index);

    public void AddCandidates(int index, ReadOnlySpan<int> values)
    {
        if (_candidates.ContainsKey(index))
        {
            throw new Exception("Something wrong happened.");
        }

        int[] candidates = new int[values.Length + 1];
        _candidates[index] = candidates;
        candidates[0] = values.Length;
        for(int i = 0; i < values.Length; i++)
        {
            candidates[i + 1] = values[i];
        }
    }

    public bool RemoveCandidates(int index)
    {
        return _candidates.Remove(index);
    }

    public void RemoveCandidates(int index, ReadOnlySpan<int> values)
    {
        int[] candidates = _candidates[index];

        // 0 index is a fake `Length` value
        if (candidates[0] == 0)
        {
            return;
        }

        HashSet<int> valuesSet = new(9);
        valuesSet.AddRange(values);
        int nextWrite = 1;

        for (int i = 1; i <= candidates[0]; i++)
        {
            if (valuesSet.Contains(candidates[i]))
            {
                candidates[0]--;
                continue;
            }

            if (i != nextWrite)
            {
                candidates[nextWrite] = candidates[i];
            }
        }

        if (candidates[0] < 0)
        {
            throw new Exception("Something wrong happened.");
        }
    }
}
