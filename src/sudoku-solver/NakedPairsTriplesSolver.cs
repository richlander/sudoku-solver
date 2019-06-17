using System;
using System.Collections.Generic;

namespace sudoku_solver
{
    public class NakedPairsTripleSolver : ISolver
    {
        Puzzle _puzzle;

        public NakedPairsTripleSolver(Puzzle puzzle)
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
                yield return SolveBox(i);
            }
        }

        private Solution SolveBox(int index)
        {
            var box = _puzzle.GetBox(index);
            var boxAsLine = box.AsLine();
            var rowOffset = box.GetRowOffset();
            var columnOffset = box.GetColumnOffset();

            var markedCells = new Dictionary<int, int[]>();

            // visit all cells in box
            for (int i =0; i <9; i++)
            {
                if (boxAsLine[i] != 0)
                {
                    continue;
                }

                // get complete row that includes current row of box
                var currentRowIndex = rowOffset + (i % 3);
                var currentRow = _puzzle.GetRow(currentRowIndex);

                // get complete column that includes the the first column of box
                var currentColIndex = columnOffset + (i / 3) - (i% 3);
                var currentCol = _puzzle.GetColumn(currentColIndex);

                var missingValues = FindMissingValues(currentRow,currentCol);
                markedCells.Add(i,missingValues.ToArray());
            }

                // assumes lines are of the same length
            Span<int> FindMissingValues(Line line1, Line line2)
            {
                bool[] values = new bool[10];
                int[] missingValues = new int[9];
                int length = -1;

                for (int i = 0; i < line1.Length; i++)
                {
                    Update(line1[i]);
                    Update(line2[i]);
                }

                for (int i = 1; i < 10; i++)
                {
                    if (!values[i])
                    {
                        length++;
                        missingValues[length] = i;
                    }
                }

                if (length == -1)
                {
                    length = 0;
                }

                return missingValues.AsSpan().Slice(0,length);

                void Update(int value)
                {
                    if (value != 0 && !values[value])
                    {
                        values[value] = true;
                    }
                }
            }

            return Solution.False;
        }
    }
}