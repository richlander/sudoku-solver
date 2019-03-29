using System;
using System.Collections.Generic;
using System.Linq;

namespace sudoku_solver
{
    public class HiddenSinglesSolver : ISolver
    {
        private Puzzle _puzzle;

        public HiddenSinglesSolver(Puzzle puzzle)
        {
            _puzzle = puzzle;
        }

        public bool IsEffective()
        {
            // too hard to tell w/o doing all the work of the solution
            return true;
        }

        public IEnumerable<Solution> FindSolution()
        {
            for (int i = 0; i < 9; i++)
            {
                var solution = Solve(i);
                if (solution.Solved)
                {
                    yield return solution;
                }
            }
        }

        public Solution Solve(int index)
        {
            var box = _puzzle.GetBox(index);

            // adjacent neighboring boxes
            // first two values in array are horizontal neighbors
            // second two values in array are vertical neighbors

            var offset = (index / 3) * 3;
            int[] avn = new int[]
            {
                (index + 1) % 3 + offset,
                (index + 2) % 3 + offset,
                (index + 3) % 9,
                (index + 6) % 9
            };

            // get adjacent neighboring boxes
            var ahnb1 = _puzzle.GetBox(avn[0]);
            var ahnb2 = _puzzle.GetBox(avn[1]);
            var avnb1 = _puzzle.GetBox(avn[2]);
            var avnb2 = _puzzle.GetBox(avn[3]);

            // iterate over the three rows in the box
            for (int i = 0; i < 3; i++)
            {
                // for each row, the neighboring rows are the same
                // the columns differ per cell
                var row1Index = i;
                var row2Index = (i + 1) % 3;
                var row3Index = (i + 2) % 3;

                // there are nine row to consider (three boxes)

                // baseline box
                var row1 = box.GetRow(row1Index);

                if (!row1.ContainsValue(0))
                {
                    continue;
                }

                var row2 = box.GetRow(row2Index);
                var row3 = box.GetRow(row3Index);

                // horizontal adjacent box 1 -- rows
                var ahnb1Row2 = ahnb1.GetRow(row2Index);
                var ahnb1Row3 = ahnb1.GetRow(row3Index);

                // horizontal adjacent box 2 -- rows
                var ahnb2Row2 = ahnb2.GetRow(row2Index);
                var ahnb2Row3 = ahnb2.GetRow(row3Index);

                // determine union of values of rows
                var row2Union = ahnb1Row2.Union(ahnb2Row2);
                var row3Union = ahnb1Row3.Union(ahnb2Row3);

                // get complete row that includes the the first row of box
                var firstRowIndex = box.GetRowOffsetForBox() + i;
                var firstRow = _puzzle.GetRow(firstRowIndex);

                // the boundaries of the adjacent boxes don't matter
                // the appropriate rows have been merged (Union) at this point

                // determine union of values of row 1 -- these values are all off-limits
                // get complete row that includes the the first row of box
                var boxValues = box.AsValues();

                // determine disjoint set with baseline rows -- box row values are off-limits

                // determine disjoint set with baseline rows -- box row values are off-limits
                var row2Values = row2Union.DisjointSet(boxValues);
                var row3Values = row3Union.DisjointSet(boxValues);

                // determine disjoint set with baseline row -- looking for values now in that row
                var row2Candidates = row2Values.DisjointSet(firstRow.Segment);
                var row3Candidates = row3Values.DisjointSet(firstRow.Segment);
                var rowCandidates = row2Candidates.Intersect(row3Candidates);

                // determine if rows on their own present a solution
                if (rowCandidates.Length == 1)
                {
                    (var justOne, var column) = row1.IsJustOneElementUnsolved();
                    if (justOne)
                    {
                        return GetSolution(index, i, column, rowCandidates[0]);
                    }
                }

                // interate over columns for row
                for (int y = 0; y < 3; y++)
                {
                    if (row1[y] != 0)
                    {
                        continue;
                    }

                    var col1Index = y;
                    var col2Index = (y + 1) % 3;
                    var col3Index = (y + 2) % 3;

                    // baseline box
                    var col1 = box.GetColumn(col1Index);
                    var col2 = box.GetColumn(col2Index);
                    var col3 = box.GetColumn(col3Index);

                    // vertical adjacent box 1 -- cols
                    var avnb1Col1 = avnb1.GetColumn(col1Index);
                    var avnb1Col2 = avnb1.GetColumn(col2Index);
                    var avnb1Col3 = avnb1.GetColumn(col3Index);

                    // vertical adjacent box 2 -- cols
                    var avnb2Col1 = avnb2.GetColumn(col1Index);
                    var avnb2Col2 = avnb2.GetColumn(col2Index);
                    var avnb2Col3 = avnb2.GetColumn(col3Index);

                    // get complete column that includes the the first column of box
                    var firstColIndex = box.GetColumnOffsetForBox() + y;
                    var firstCol = _puzzle.GetColumn(firstColIndex);

                    // determine union of values of cols
                    var col2Union = avnb1Col2.Union(avnb2Col2);
                    var col3Union = avnb1Col3.Union(avnb2Col3);

                    // determine disjoint set with baseline cols -- box col values are off-limits
                    var col2Values = col2Union.DisjointSet(boxValues);
                    var col3Values = col3Union.DisjointSet(boxValues);

                    // determine disjoint set with baseline col -- looking for values now in that col
                    var col2Candidates = col2Values.DisjointSet(firstCol.Segment);
                    var col3Candidates = col3Values.DisjointSet(firstCol.Segment);
                    var colCandidates = col2Candidates.Intersect(col3Candidates);

                    // determine if columns on their own present a solution
                    if (colCandidates.Length == 1)
                    {
                        (var justOne, var row) = col1.IsJustOneElementUnsolved();
                        if (justOne)
                        {
                            return GetSolution(index, row, y, colCandidates[0]);
                        }
                    }

                    // determine if rows and columns together present a solution
                    var rowAndColumnCandidates = rowCandidates.Intersect(colCandidates);
                    if (rowAndColumnCandidates.Length == 1)
                    {
                        return GetSolution(index, i, y, rowAndColumnCandidates[0]);
                    }

                    // determine if various combinations of rows and columns present a solution

                    // row1 is candidate for all following cases

                    // following set of cases involve a row or a column with:
                    // a candidate, one cell == 0 and the other non zero
                    // the cases are looking for a set of rows and columns
                    // that project the same value except for the row or column with 
                    // the non-zero value (which doesn't require a projected value)

                    // row 2 == 0; row 3 != 0
                    // row 2 needs to match columns; row 3 values need to be considered
                    var row2AndColumnCandidates = row2Candidates.Intersect(colCandidates);
                    var row2AndColumnCandidatesNarrowed = row2AndColumnCandidates.DisjointSet(row3Candidates);
                    if (row2AndColumnCandidatesNarrowed.Length == 1 && row2[0] == 0 && row3[0] != 0)
                    {
                        return GetSolution(index, i, y, row2AndColumnCandidatesNarrowed[0]);
                    }

                    if (row2AndColumnCandidatesNarrowed.Length == 1 && row2[0] != 0 && row3[0] == 0)
                    {
                        return GetSolution(index, i, y, row2AndColumnCandidatesNarrowed[0]);
                    }

                    // row 2 != 0; row 3 == 0
                    // row 3 needs to match columns; row 2 values need to be considered
                    var row3AndColumnCandidates = row3Candidates.Intersect(colCandidates);
                    var row3AndColumnCandidatesNarrowed = row3AndColumnCandidates.DisjointSet(row2Candidates);
                    if (row3AndColumnCandidatesNarrowed.Length == 1 && row2[0] != 0 && row3[0] == 0)
                    {
                        return GetSolution(index, i, y, row3AndColumnCandidatesNarrowed[0]);
                    }

                    if (row3AndColumnCandidatesNarrowed.Length == 1 && row2[0] == 0 && row3[0] != 0)
                    {
                        return GetSolution(index, i, y, row3AndColumnCandidatesNarrowed[0]);
                    }

                    // row 3 == 0; row 2 == 0
                    // col 2 == 0; col 3 != 0
                    var col2AndRowCandidates = col2Candidates.Intersect(rowCandidates);
                    var col2AndRowCandidatesNarrowed = col2AndRowCandidates.DisjointSet(col3Candidates);
                    if (col2AndRowCandidatesNarrowed.Length == 1 && col2[i] == 0 && col3[i] != 0)
                    {
                        return GetSolution(index, i, y, col2AndRowCandidatesNarrowed[0]);
                    }

                    // row 3 == 0; row 2 == 0
                    // col 2 != 0; col 3 == 0
                    var col3AndRowCandidates = col3Candidates.Intersect(rowCandidates);
                    var col3AndRowCandidatesNarrowed = col3AndRowCandidates.DisjointSet(col2Candidates);
                    if (col3AndRowCandidatesNarrowed.Length == 1 && col2[i] != 0 && col3[i] == 0)
                    {
                        return GetSolution(index, i, y, col3AndRowCandidatesNarrowed[0]);
                    }

                    // following set of cases are more complicated
                    // they enable more incomplete rows or columns

                    // column 2 and 3 and row2 project value
                    // row 3, column 3 has a value
                    // As a result, row 2, column 3 should be projected value

                    if (row2AndColumnCandidates.Length == 1 && row3[y] != 0)
                    {
                        return GetSolution(index, i, y, row2AndColumnCandidates[0]);
                    }

                    // Full column forces projection of value into another column
     
                }

                Solution GetSolution(int box, int row, int column, int value)
                {
                    (var r, var c) = Puzzle.GetLocationForBoxCell(index, (row * 3) + column);
                    return new Solution
                    {
                        Solved = true,
                        Value = value,
                        Row = r,
                        Column = c,
                        Solver = this
                    };
                }
            }
           
            return new Solution
            {
                Solved = false
            };
        }
    }
}
