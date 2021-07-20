namespace sudoku_solver
{
    public record Solution(int Row, int Column, int Value, string Solver, string SolverKind)
    {
        public string GetLocation() => $"r{Row+1}:c{Column+1}";
    };
}
