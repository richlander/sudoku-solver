using System.Collections.Generic;

namespace sudoku_solver
{
    public class BruteForceSolver : ISolver
    {

        Puzzle _puzzle;

        public BruteForceSolver(Puzzle puzzle)
        {
            _puzzle = puzzle;
        }
        public IEnumerable<Solution> FindSolution()
        {
            throw new System.NotImplementedException();
        }

        public bool IsEffective()
        {
            return true;
        }

        public void Solve()
        {
            
        }
    }
}