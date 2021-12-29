using System.Collections.Generic;

namespace sudoku_solver;

public interface ISolver
{
    bool TrySolve(Puzzle puzzle, out Solution solution);
}
