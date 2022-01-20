namespace sudoku_solver;

public interface ICandidateSolver
{
        // Attempts to find candidates that can be removed
        bool TryFindCandidates(Puzzle puzzle, [NotNullWhen(true)] out Candidates? candidates);
}