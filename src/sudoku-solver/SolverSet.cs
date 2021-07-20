using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace sudoku_solver
{
    public class SolverSet : ISolver
    {
        IList<ISolver> _solvers;

        public SolverSet(IList<ISolver> solvers)
        {
            _solvers = solvers;
        }

        public int SolutionCount {get; private set;}

        // Approach chosen is to collect the first solution from the solvers
        // then reset to first (assumed to be cheapest/simplest) solver.
        public bool TrySolve(out Solution solution)
        {
            foreach(ISolver solver in _solvers)
            {
                if (solver.TrySolve(out solution))
                {
                    SolutionCount++;
                    return true;
                }
            }
            solution = null;
            return false;
        }

        public void Solve()
        {
            while(TrySolve(out Solution solution))
            {
            }
        }
    }
}
