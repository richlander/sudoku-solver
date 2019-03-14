namespace sudoku_solver
{
    public struct Solution
    {
        public int Column;
        public int Row;
        public int Value;
        public bool Solved;
        public ISolver Solver;

        public string SolverKind;

        public static Solution False => new Solution { Solved = false};

        public string GetLocation()
        {
            if (Solved)
            {
                return $"r{Row+1}:c{Column+1}";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
