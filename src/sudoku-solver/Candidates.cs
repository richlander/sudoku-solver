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

    public IEnumerable<int> Positions => _candidates.Keys;

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

    // Update candidates list -- additive
    public void UpdateAddCandidates(int index, ReadOnlySpan<int> values)
    {
        if (!_candidates.ContainsKey(index))
        {
            AddCandidates(index, values);
            return;
        }

        int[] candidates = _candidates[index];

        HashSet<int> candidatesSet = new(9);
        candidatesSet.AddRange(candidates);

        for (int i = 0; i < values.Length; i++)
        {
            if (!candidatesSet.Contains(values[i]))
            {
                candidates[candidates[0]] = values[i];
                candidates[0]++;
            }
            else
            {
                throw new Exception("Something went wrong here.");
            }
        }
    }

    // Update candidates list -- subtractive
    public void UpdateRemoveCandidates(int index, ReadOnlySpan<int> values)
    {
        if (!_candidates.ContainsKey(index))
        {
            throw new Exception("Something went wrong here.");
        }

        int[] candidates = _candidates[index];

        HashSet<int> valuesSet = new(9);
        valuesSet.AddRange(values);
        int nextWrite = 1;
        int length = candidates[0];

        for (int i = 1; i <= length; i++)
        {
            if (valuesSet.Contains(candidates[i]))
            {
                candidates[0]--;
                continue;
            }

            candidates[nextWrite] = candidates[i];
            nextWrite++;
        }
    }

    public void UpdateRemoveCandidates(Candidates candidates)
    {
        foreach(int position in candidates.Positions)
        {
            UpdateRemoveCandidates(position, candidates[position]);
        }
    }
}
