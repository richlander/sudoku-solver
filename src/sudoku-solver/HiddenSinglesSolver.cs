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
            // goal is to solve a cell in this row (not other rows)
            // ititial logic (body of first for loop) can solve any row cell using
            // horizontal adjacent rows as input
            // later logic (body of embedded for loop) can solve a given cell, one at a time
            // using rows and/or columns as input
            for (int i = 0; i < 3; i++)
            {
                // calculate rows for i
                var row1Index = i;
                var row2Index = (i + 1) % 3;
                var row3Index = (i + 2) % 3;

                // target box
                var boxRow1 = box.GetRow(row1Index);

                if (!boxRow1.ContainsValue(0))
                {
                    continue;
                }

                var boxRow2 = box.GetRow(row2Index);
                var boxRow3 = box.GetRow(row3Index);


                // neighboring boxess
                // horizontal adjacent box 1 -- rows
                var ahnb1Row2 = ahnb1.GetRow(row2Index);
                var ahnb1Row3 = ahnb1.GetRow(row3Index);

                // horizontal adjacent box 2 -- rows
                var ahnb2Row2 = ahnb2.GetRow(row2Index);
                var ahnb2Row3 = ahnb2.GetRow(row3Index);

                // determine union of values of rows
                var adjRow2Union = ahnb1Row2.Union(ahnb2Row2);
                var adjRow3Union = ahnb1Row3.Union(ahnb2Row3);

                // get complete row that includes current row of box
                var currentRowIndex = box.GetRowOffsetForBox() + i;
                //TODO: consider an overload that only returns non-zero values
                var currentRow = _puzzle.GetRow(currentRowIndex);

                // get all values in box
                var boxValues = box.AsValues();

                // calculate full set of illegal values for row 1
                var boxRow1IllegalValues = boxValues.Union(currentRow.Segment);

                // determine disjoint set with baseline row -- looking for values in that row
                var row2Candidates = adjRow2Union.DisjointSet(boxRow1IllegalValues);
                var row3Candidates = adjRow3Union.DisjointSet(boxRow1IllegalValues);
                var boxRow1Candidates = row2Candidates.Intersect(row3Candidates);

                // determine if rows on their own present a solution
                if (boxRow1Candidates.Length == 1)
                {
                    (var justOne, var column) = boxRow1.IsJustOneElementUnsolved();
                    if (justOne)
                    {
                        var solverKind = "RowSolver";
                        return GetSolution(index, i, column, boxRow1Candidates[0],solverKind);
                    }
                }

                // interate over columns for row
                // mostly targets row[column] cell
                for (int y = 0; y < 3; y++)
                {
                    if (boxRow1[y] != 0)
                    {
                        continue;
                    }

                    var col1Index = y;
                    var col2Index = (y + 1) % 3;
                    var col3Index = (y + 2) % 3;

                    // baseline box
                    var boxCol1 = box.GetColumn(col1Index);
                    var boxCol2 = box.GetColumn(col2Index);
                    var boxCol3 = box.GetColumn(col3Index);

                    // vertical adjacent box 1 -- cols
                    var avnb1Col1 = avnb1.GetColumn(col1Index);
                    var avnb1Col2 = avnb1.GetColumn(col2Index);
                    var avnb1Col3 = avnb1.GetColumn(col3Index);

                    // vertical adjacent box 2 -- cols
                    var avnb2Col1 = avnb2.GetColumn(col1Index);
                    var avnb2Col2 = avnb2.GetColumn(col2Index);
                    var avnb2Col3 = avnb2.GetColumn(col3Index);

                    // get complete column that includes the the first column of box
                    var currentColIndex = box.GetColumnOffsetForBox() + y;
                    var currentCol = _puzzle.GetColumn(currentColIndex);

                    // calculate full set of illegal values for col 1
                    var boxCol1IllegalValues = boxValues.Union(currentCol.Segment);

                    // determine union of values of cols
                    var col2Union = avnb1Col2.Union(avnb2Col2);
                    var col3Union = avnb1Col3.Union(avnb2Col3);

                    // determine disjoint set with baseline col -- looking for values now in that col
                    var col2Candidates = col2Union.DisjointSet(boxCol1IllegalValues);
                    var col3Candidates = col3Union.DisjointSet(boxCol1IllegalValues);
                    var boxCol1Candidates = col2Candidates.Intersect(col3Candidates);

                    // determine if columns on their own present a solution
                    // can solve any cell in given box column
                    if (boxCol1Candidates.Length == 1)
                    {
                        (var justOne, var row) = boxCol1.IsJustOneElementUnsolved();
                        if (justOne)
                        {
                            var solverKind = "ColumnSolver";
                            return GetSolution(index, row, y, boxCol1Candidates[0],solverKind);
                        }
                    }

                    // determine if rows and columns together present a solution
                    var rowAndColumnCandidates = boxRow1Candidates.Intersect(boxCol1Candidates);
                    if (rowAndColumnCandidates.Length == 1)
                    {
                        if (boxCol1[i] == 0)
                        {
                            var solverKind = "RowColumnSolver";
                            return GetSolution(index, i, y, rowAndColumnCandidates[0],solverKind);
                        }
                    }

                    // determine if various combinations of rows and columns present a solution

                    // col1[row] is candidate for all following cases

                    // the following set of cases involve a row or a column with:
                    // a candidate, one cell == 0 and the other non zero
                    // the cases are looking for a set of rows and columns
                    // that project the same value except for the row or column with 
                    // the non-zero value (which doesn't require a projected value)

                    // check rows
                    if (boxRow1Candidates.Length > 0)                    
                    {
                        bool solved;
                        Solution solution;

                        // row1[col3] has a value, so col3 don't need to participate in providing a candidate
                        if (col2Candidates.Length > 0 &&
                            boxRow1[col2Index] == 0 && boxRow1[col3Index] != 0 &&
                            ((solved, solution) = CheckForBlockedCellCandidates(boxRow1Candidates, col2Candidates, "Column2CandidateBlockedColumn3+RowsSolver")).solved)
                        {
                            return solution;
                        }

                        // row1[col2] has a value, so col2 don't need to participate in providing a candidate
                        if (col3Candidates.Length > 0 &&
                            boxRow1[col2Index] != 0 && boxRow1[col3Index] == 0 &&
                            ((solved, solution) = CheckForBlockedCellCandidates(boxRow1Candidates, col3Candidates, "Column3CandidateBlockedColumn2+RowsSolver")).solved)
                        {
                            return solution;
                        }
                    }


                    // check columns
                    if (boxCol1Candidates.Length > 0)                    
                    {
                        bool solved;
                        Solution solution;

                        // col1[row3] has a value, so row3 don't need to participate in providing a candidate
                        if (row2Candidates.Length > 0 &&
                            boxCol1[row2Index] == 0 && boxCol1[row3Index] != 0 &&
                            ((solved, solution) = CheckForBlockedCellCandidates(boxCol1Candidates, row2Candidates, "Row2CandidateBlockedRow3+RowsSolver")).solved)
                        {
                            return solution;
                        }

                        // col1[row2] has a value, so row2 don't need to participate in providing a candidate
                        if (row3Candidates.Length > 0 &&
                            boxCol1[row2Index] != 0 && boxCol1[row3Index] == 0 &&
                            ((solved, solution) = CheckForBlockedCellCandidates(boxCol1Candidates, row3Candidates, "Row3CandidateBlockedRow2+RowsSolver")).solved)
                        {
                            return solution;
                        }
                    }
                    /* 

                    // row1[col2] has a value, so col2 don't need to participate in providing a candidate
                    if (boxRow1Candidates.Length > 0 && col3Candidates.Length > 0 &&
                        boxRow1[col2Index] != 0 && boxRow1[col3Index] == 0)
                    {
                        var col2AndRowCandidates = boxRow1Candidates.Intersect(col3Candidates);
                        if (col2AndRowCandidates.Length == 1)
                        {
                            var solverKind = "BlockedColumn2+RowsSolver";
                            return GetSolution(index, i, y, col2AndRowCandidates[0],solverKind);
                        }
                    }

                    // row1[col3] has a value, so col2 don't need to participate in providing a candidate
                    if (boxRow1Candidates.Length > 0 && col2Candidates.Length > 0 &&
                        boxRow1[col2Index] == 0 && boxRow1[col3Index] != 0)
                    {
                        var col2AndRowCandidates = boxRow1Candidates.Intersect(col2Candidates);
                        if (col2AndRowCandidates.Length == 1)
                        {
                            var solverKind = "BlockedColumn3+RowsSolver";
                            return GetSolution(index, i, y, col2AndRowCandidates[0],solverKind);
                        }
                    }
                    

                    // col1[row2] has a value, so row2 don't need to participate in providing a candidate
                    if (boxCol1Candidates.Length > 0 && row3Candidates.Length > 0 &&
                        boxCol1[row2Index] != 0 && boxCol1[row3Index] == 0)
                    {
                        var row2AndColCandidates = boxCol1Candidates.Intersect(row3Candidates);
                        if (row2AndColCandidates.Length == 1)
                        {
                            var solverKind = "BlockedRow2+ColsSolver";
                            return GetSolution(index, i, y, row2AndColCandidates[0],solverKind);
                        }
                    }

                    // row1[col3] has a value, so col3 don't need to participate in providing a candidate
                    if (boxCol1Candidates.Length > 0 && row2Candidates.Length > 0 &&
                        boxCol1[row2Index] == 0 && boxCol1[row3Index] != 0)
                    {
                        var row2AndColCandidates = boxCol1Candidates.Intersect(row2Candidates);
                        if (row2AndColCandidates.Length == 1)
                        {
                            var solverKind = "BlockedRow3+ColsSolver";
                            return GetSolution(index, i, y, row2AndColCandidates[0],solverKind);
                        }
                    }
*/

                    // the following set of cases involve a row or column with:
                    // all cells filled, such that a row or column doesn't need to participate in providing a candidate

                    // col3 is blocked, so col3 doesn't need to participate in providing a candidate
                    var col2Blocked = boxCol2.GetUnsolvedCount() == 0;
                    var col3Blocked = boxCol3.GetUnsolvedCount() == 0;
                    var row2Blocked = boxRow2.GetUnsolvedCount() == 0;
                    var row3Blocked = boxRow3.GetUnsolvedCount() == 0;

                    if (boxCol1.GetUnsolvedCount() == 1)
                    {
                        if (col3Blocked && 
                            col2Candidates.Length == 1)
                        {
                            var solverKind = "Col3BlockedCol2Candidate";
                            return GetSolution(index, i, y, col2Candidates[0],solverKind);
                        }
                    }


                    (bool solved, Solution solution) CheckForBlockedCellCandidates(ReadOnlySpan<int> candidate1, ReadOnlySpan<int> candidate2, string solverKind)
                    {
                        var candidates = candidate1.Intersect(candidate2);
                        if (candidates.Length == 1)
                        {
                            return (true, GetSolution(index, i, y, candidates[0],solverKind));
                        }
                        return (false, Solution.False);
                    }


                    /* 

                    continue;

                    // row 2 == 0; row 3 != 0
                    // row 2 needs to match columns; row 3 values need to be considered
                    var row2AndColumnCandidates = adjRow2Candidates.Intersect(boxCol1Candidates);
                    var row2AndColumnCandidatesNarrowed = row2AndColumnCandidates.DisjointSet(adjRow3Candidates);
                    if (row2AndColumnCandidatesNarrowed.Length == 1 && boxRow2[0] == 0 && boxRow3[0] != 0)
                    {
                        var solverKind = "unknown";
                        return GetSolution(index, i, y, row2AndColumnCandidatesNarrowed[0],solverKind);
                    }

                    if (row2AndColumnCandidatesNarrowed.Length == 1 && boxRow2[0] != 0 && boxRow3[0] == 0)
                    {
                        var solverKind = "unknown";
                        return GetSolution(index, i, y, row2AndColumnCandidatesNarrowed[0],solverKind);
                    }

                    // row 2 != 0; row 3 == 0
                    // row 3 needs to match columns; row 2 values need to be considered
                    var row3AndColumnCandidates = adjRow3Candidates.Intersect(boxCol1Candidates);
                    var row3AndColumnCandidatesNarrowed = row3AndColumnCandidates.DisjointSet(adjRow2Candidates);
                    if (row3AndColumnCandidatesNarrowed.Length == 1 && boxRow2[0] != 0 && boxRow3[0] == 0)
                    {
                        var solverKind = "unknown";
                        return GetSolution(index, i, y, row3AndColumnCandidatesNarrowed[0],solverKind);
                    }

                    if (row3AndColumnCandidatesNarrowed.Length == 1 && boxRow2[0] == 0 && boxRow3[0] != 0)
                    {
                        var solverKind = "unknown";
                        return GetSolution(index, i, y, row3AndColumnCandidatesNarrowed[0],solverKind);
                    }

                    // row 3 == 0; row 2 == 0
                    // col 2 == 0; col 3 != 0
                    var col2AndRowCandidates = col2Candidates.Intersect(boxRow1Candidates);
                    var col2AndRowCandidatesNarrowed = col2AndRowCandidates.DisjointSet(col3Candidates);
                    if (col2AndRowCandidatesNarrowed.Length == 1 && boxCol2[i] == 0 && boxCol3[i] != 0)
                    {
                        var solverKind = "unknown";
                        return GetSolution(index, i, y, col2AndRowCandidatesNarrowed[0],solverKind);
                    }

                    // row 3 == 0; row 2 == 0
                    // col 2 != 0; col 3 == 0
                    var col3AndRowCandidates = col3Candidates.Intersect(boxRow1Candidates);
                    var col3AndRowCandidatesNarrowed = col3AndRowCandidates.DisjointSet(col2Candidates);
                    if (col3AndRowCandidatesNarrowed.Length == 1 && boxCol2[i] != 0 && boxCol3[i] == 0)
                    {
                        var solverKind = "unknown";
                        return GetSolution(index, i, y, col3AndRowCandidatesNarrowed[0],solverKind);
                    }

                    // following set of cases are more complicated
                    // they enable more incomplete rows or columns

                    // column 2 and 3 and row2 project value
                    // row 3, column 3 has a value
                    // As a result, row 2, column 3 should be projected value

                    if (row2AndColumnCandidates.Length == 1 && boxRow3[y] != 0)
                    {
                        var solverKind = "unknown";
                        return GetSolution(index, i, y, row2AndColumnCandidates[0],solverKind);
                    }

                    // Full column forces projection of value into another column

                    */  
                }

                Solution GetSolution(int box, int row, int column, int value, string solverKind)
                {
                    (var r, var c) = Puzzle.GetLocationForBoxCell(index, (row * 3) + column);
                    return new Solution
                    {
                        Solved = true,
                        Value = value,
                        Row = r,
                        Column = c,
                        Solver = this,
                        SolverKind = solverKind
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
