using System.Collections.Generic;

namespace sudoku_solver
{
    public interface ISolver
    {
        IEnumerable<Solution> FindSolution();
        bool IsEffective();
    }
}
