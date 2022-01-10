namespace sudoku_solver_extensions;

public static class HashSetExtensions
{
    public static void AddRange(this HashSet<int> set, ReadOnlySpan<int> values)
    {
        foreach(int value in values)
        {
            set.Add(value);
        }
    }
}