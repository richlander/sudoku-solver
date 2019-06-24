using System;
using System.Collections.Generic;

namespace sudoku_solver
{
    public class NakedMultiplesSolver : ISolver
    {
        Puzzle _puzzle;

        public NakedMultiplesSolver(Puzzle puzzle)
        {
            _puzzle = puzzle;
        }

        public bool IsEffective()
        {
            return true;
        }

        public IEnumerable<Solution> FindSolution()
        {
            for (int i = 0; i < 9; i++)
            {
                var solution = SolveBox(i);
                if (solution.Solved)
                {
                    yield return solution;
                }
            }
        }

        private Solution SolveBox(int index)
        {
            var box = _puzzle.GetBox(index);
            var boxCandidates = new BoxCandidates(box);

            (var found, var multiples) = boxCandidates.GetMultiples();

            if (!found)
            {
                return Solution.False;
            }
            
            for (int i = 0; i <9; i++)
            {
                var cellCandidates = boxCandidates[i];

                if (cellCandidates.Length == 0)
                {
                    continue;
                }

                var candidates = cellCandidates.DisjointSet(multiples);

                if (candidates.Length == 1)
                {
                    return box.GetSolution(i,candidates[0],nameof(NakedMultiplesSolver));
                }
            }

            return Solution.False;
        }
    }
}