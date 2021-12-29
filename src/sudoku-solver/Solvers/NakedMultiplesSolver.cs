using System;
using System.Collections.Generic;

namespace sudoku_solver
{
    // Naked multiples: cells with matching candidates in a unit (like two cells in a box with only 5 and 9 and candidates)
    // remove those candidates for all other cells in the unit. 

    // Example:
    // Solved cell: r2:c7; 7
    // Solved by: NakedMultiplesSolver
    //                 *
    // 7 9 4 | 5 8 2 | 1 3 6
    // 2 6 0 | 0 3 1 | 7 0 0*
    // 0 0 0 | 0 7 6 | 0 0 2
    // ------+-------+------
    // 6 8 0 | 7 1 0 | 3 2 4
    // 4 3 2 | 8 6 0 | 0 0 0
    // 0 0 7 | 2 4 3 | 8 6 0
    // ------+-------+------
    // 0 2 0 | 6 5 7 | 4 0 3
    // 0 4 0 | 1 2 8 | 6 0 7
    // 0 7 6 | 3 9 4 | 2 0 8
    public class NakedMultiplesSolver : ISolver
    {
        Puzzle _puzzle;

        public NakedMultiplesSolver()
        {
        }

        public NakedMultiplesSolver(Puzzle puzzle)
        {
            _puzzle = puzzle;
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