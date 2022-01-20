namespace sudoku_solver;

public interface ISolver
{
    bool TrySolve(Puzzle puzzle, [NotNullWhen(true)] out Solution? solution);
}
