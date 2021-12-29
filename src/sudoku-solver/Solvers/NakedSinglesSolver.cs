using System;
using System.Collections.Generic;

namespace sudoku_solver
{
    // Naked singles: One absent value in a unit (row, column, box) of 9
    // Solves a row like this: 023456789
    // Expected solution is: 1

    // Example:
    // Solved cell: r4:c3; 2
    // Solved by: NakedSinglesSolver
    //     *
    // 3 0 5 | 4 2 0 | 8 1 0
    // 4 8 7 | 9 0 1 | 5 0 6
    // 0 2 9 | 0 5 6 | 3 7 4
    // ------+-------+------
    // 8 5 2 | 7 9 3 | 0 4 1*
    // 6 1 3 | 2 0 8 | 9 5 7
    // 0 7 4 | 0 6 5 | 2 8 0
    // ------+-------+------
    // 2 4 1 | 3 0 9 | 0 6 5
    // 5 0 8 | 6 7 0 | 1 9 2
    // 0 9 6 | 5 1 2 | 4 0 8

    public class NakedSinglesSolver : ISolver
    {
        private Puzzle _puzzle;
        private const int _effectiveCount = 8;
        public NakedSinglesSolver()
        {
        }

        public NakedSinglesSolver(Puzzle puzzle)
        {
            _puzzle = puzzle;
        }

        public bool TrySolve(Puzzle puzzle, out Solution solution)
        {
            _puzzle = puzzle;

            for (int i = 0; i < 9; i++)
            {
                if (TrySolveBox(i, out solution) ||
                    TrySolveColumn(i, out solution) ||
                    TrySolveRow(i, out solution))
                {
                    return true;
                }
            }

            solution = null;
            return false;
        }

        public bool TrySolveColumn(int index, out Solution solution)
        {
            if (_puzzle.SolvedForColumn[index] == _effectiveCount &&
                TrySolveLine(_puzzle.GetColumn(index), out int row, out int value))
            {
                solution = new Solution(
                    row,
                    index,
                    value,
                    nameof(NakedSinglesSolver),
                    null);
                return true;
            }

            solution = null;
            return false;
        }

        public bool TrySolveRow(int index, out Solution solution)
        {
            if (_puzzle.SolvedForRow[index] == _effectiveCount &&
                TrySolveLine(_puzzle.GetRow(index), out int column, out int value))
            {
                solution = new Solution(
                    index,
                    column,
                    value,
                    nameof(NakedSinglesSolver),
                    null
                );
                return true;
            }

            solution = null;
            return false;
        }

        public bool TrySolveBox(int index, out Solution solution)
        {
            if (_puzzle.SolvedForBox[index] == _effectiveCount &&
                TrySolveLine(_puzzle.GetBoxAsLine(index), out int lineIndex, out int value))
            {
                var (row, column) = Puzzle.GetLocationForBoxCell(index, lineIndex);
                solution = new Solution(
                    row,
                    column,
                    value,
                    nameof(NakedSinglesSolver),
                    null
                );
                return true;
            }

            solution = null;
            return false;
        }

        private bool TrySolveLine(Line line, out int index, out int value)
        {
            bool[] values = new bool[10];
            int unsolvedCells = 0;
            index = 0;
            value = 0;

            for (int i = 0; i <9;i++)
            {
                int val = line.Segment[i];
                if (val > 0)
                {
                    values[val] = true;
                }
                else
                {
                    index = i;
                    unsolvedCells++;
                }

                if (unsolvedCells > 1)
                {
                    return false;
                }
            }

            if (unsolvedCells == 0)
            {
                return false;
            }

            for (int i = 1; i < 10; i++)
            {
                if (!values[i])
                {
                    value = i;
                    return true;
                }
            }

            return false;
        }
    }
}
