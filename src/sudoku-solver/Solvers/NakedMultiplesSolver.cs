using System;
using System.Collections.Generic;

namespace sudoku_solver
{
    public class NakedMultiplesSolver : ISolver
    {
        Puzzle _puzzle;

        public NakedMultiplesSolver()
        {
        }

        public bool TrySolve(Puzzle puzzle, out Solution solution)
        {
            _puzzle = puzzle;

            for (int i = 0; i < 9; i++)
            {
                if (TrySolveBox(i, out solution))
                {
                    return true;
                }
            }

            solution = null;
            return false;
        }

        private bool TrySolveBox(int index, out Solution solution)
        {
            solution = null;
            Box box = _puzzle.GetBox(index);
            BoxCandidates boxCandidates = new(box);

            var (found, multiples) = boxCandidates.GetMultiples();

            if (!found)
            {
                return false;
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
                    var (row, column) = box.GetLocation(i);
                    solution = new Solution(
                        row,
                        column,
                        candidates[0],
                        nameof(NakedMultiplesSolver),
                        null
                    );
                    return true;
                }
            }

            return false;
        }
    }
}