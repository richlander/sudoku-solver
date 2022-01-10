using System.Diagnostics.CodeAnalysis;

namespace sudoku_solver;

public interface ICandidateSolver
{
        // Attempts to find candidates that can be removed
        bool TryRemoveCandidates(Puzzle puzzle, [NotNullWhen(true)] out Candidates? removalCandidates);
}