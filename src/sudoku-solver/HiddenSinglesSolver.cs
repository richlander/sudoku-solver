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

            // get adjacent neighboring boxes
            var ahnb1 = box.FirstHorizontalNeighbor;
            var ahnb2 = box.SecondHorizontalNeighbor;
            var avnb1 = box.FirstVerticalNeighbor;
            var avnb2 = box.SecondVerticalNeighbor;

            // iterate over the three rows in the box
            // goal is to solve a cell in this row (not other rows)
            // initial logic (body of first for loop) can solve any row cell using
            // horizontal adjacent rows as input
            // later logic (body of embedded for loop) can solve a given cell, one at a time
            // using rows and/or columns as input
            for (int i = 0; i < 3; i++)
            {

                // target box
                var boxRow1 = box.GetRow(i);

                // check if row contains a zero
                if (!boxRow1.ContainsValue(0))
                {
                    continue;
                }

                // calculate rows for i
                var row1Index = i;
                var row2Index = (i + 1) % 3;
                var row3Index = (i + 2) % 3;

                var boxRow2 = box.GetRow(row2Index);
                var boxRow3 = box.GetRow(row3Index);

                // get all values in box
                var boxLine = box.AsLine();

                // neighboring boxes
                // horizontal adjacent box 1 -- rows
                var ahnb1Row1 = ahnb1.GetRow(row1Index);
                var ahnb1Row2 = ahnb1.GetRow(row2Index);
                var ahnb1Row3 = ahnb1.GetRow(row3Index);

                // horizontal adjacent box 2 -- rows
                var ahnb2Row1 = ahnb1.GetRow(row1Index);
                var ahnb2Row2 = ahnb2.GetRow(row2Index);
                var ahnb2Row3 = ahnb2.GetRow(row3Index);

                // determine union of values of rows
                var adjRow2Union = ahnb1Row2.Union(ahnb2Row2);
                var adjRow3Union = ahnb1Row3.Union(ahnb2Row3);

                // get complete row that includes current row of box
                var currentRowIndex = box.GetRowOffsetForBox() + i;
                var currentRow = _puzzle.GetRow(currentRowIndex);

                // calculate full set of illegal values for row 1
                var boxRow1IllegalValues = boxLine.Union(currentRow);

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
                        var solverKind = nameof(Strategy.RowSolver);
                        return GetSolution(index, i, column, boxRow1Candidates[0],solverKind);
                    }
                }

                // iterate over columns for row
                // mostly targets row[column] cell
                for (int y = 0; y < 3; y++)
                {
                    // check if current value is 0
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
                    var boxCol1IllegalValues = boxLine.Union(currentCol);

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
                            var solverKind = nameof(Strategy.ColumnSolver);
                            return GetSolution(index, row, y, boxCol1Candidates[0],solverKind);
                        }
                    }

                    // determine if rows and columns together present a solution
                    // test: SolutionAllSlotsEmpty
                    var rowAndColumnCandidates = boxRow1Candidates.Intersect(boxCol1Candidates);
                    if (rowAndColumnCandidates.Length == 1)
                    {
                        var solverKind = nameof(Strategy.RowColumnSolver);
                        if (boxCol1[i] == 0)
                        {
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
                        var solverKind = nameof(Strategy.ColumnCandidateColumnBlockedRowSolver);

                        // row1[col3] has a value, so col3 don't need to participate in providing a candidate
                        if (col2Candidates.Length > 0 &&
                            boxRow1[col2Index] == 0 && boxRow1[col3Index] != 0 &&
                            ((solved, solution) = CheckForIntersectionCandidates(boxRow1Candidates, col2Candidates, solverKind)).solved)
                        {
                            return solution;
                        }

                        // row1[col2] has a value, so col2 don't need to participate in providing a candidate
                        if (col3Candidates.Length > 0 &&
                            boxRow1[col2Index] != 0 && boxRow1[col3Index] == 0 &&
                            ((solved, solution) = CheckForIntersectionCandidates(boxRow1Candidates, col3Candidates, solverKind)).solved)
                        {
                            return solution;
                        }
                    }

                    // check columns
                    if (boxCol1Candidates.Length > 0)                    
                    {
                        bool solved;
                        Solution solution;
                        var solverKind = nameof(Strategy.RowCandidateRowBlockedColumnSolver);

                        // col1[row3] has a value, so row3 don't need to participate in providing a candidate
                        if (row2Candidates.Length > 0 &&
                            boxCol1[row2Index] == 0 && boxCol1[row3Index] != 0 &&
                            ((solved, solution) = CheckForIntersectionCandidates(boxCol1Candidates, row2Candidates, solverKind)).solved)
                        {
                            return solution;
                        }

                        // col1[row2] has a value, so row2 don't need to participate in providing a candidate
                        // test: SolutionTwoSlotsEmptyInColumn
                        if (row3Candidates.Length > 0 &&
                            boxCol1[row2Index] != 0 && boxCol1[row3Index] == 0 &&
                            ((solved, solution) = CheckForIntersectionCandidates(boxCol1Candidates, row3Candidates, solverKind)).solved)
                        {
                            return solution;
                        }
                    }

                    // the following set of cases involve a row or column with:
                    // all cells filled, such that a row or column doesn't need to participate in providing a candidate

                    // col3 is blocked, so col3 doesn't need to participate in providing a candidate
                    var col2Blocked = boxCol2.GetUnsolvedCount() == 0;
                    var col3Blocked = boxCol3.GetUnsolvedCount() == 0;
                    var row2Blocked = boxRow2.GetUnsolvedCount() == 0;
                    var row3Blocked = boxRow3.GetUnsolvedCount() == 0;

                    if (boxCol1.GetUnsolvedCount() == 1)
                    {
                        var solverKind = nameof(Strategy.ColumnCandidateColumnBlocked);

                        // Column 1 one slot open, column 2 blocked, column 3 offers a candidate
                        if (col2Blocked && 
                            col3Candidates.Length == 1)
                        {
                            return GetSolution(index, i, y, col3Candidates[0],solverKind);
                        }
                        
                        // Column 1 one slot open, column 2 offers a candidate, column 3 blocked
                        if (col3Blocked && 
                            col2Candidates.Length == 1)
                        {
                            return GetSolution(index, i, y, col2Candidates[0],solverKind);
                        }
                    }

                    if (boxRow1.GetUnsolvedCount() == 1)
                    {
                        var solverKind = nameof(Strategy.RowCandidateRowBlocked);
                        // Row 1 one slot open, row 2 blocked, row 3 offers a candidate
                        if (row2Blocked && 
                            row3Candidates.Length == 1)
                        {
                            return GetSolution(index, i, y, row3Candidates[0],solverKind);
                        }
                        
                        // Column 1 one slot open, column 2 offers a candidate, column 3 blocked
                        if (row3Blocked && 
                            row2Candidates.Length == 1)
                        {
                            return GetSolution(index, i, y, row2Candidates[0],solverKind);
                        }
                    }

                    // following set of cases test hidden singles in columns or rows

                    // pattern
                    // box 1 col1 is full
                    // box 2 has one value not in col1
                    // only one slot available in col1 in current box
                    // means that current cell must have that value
 
                    {
                        var solverKind = nameof(Strategy.ColumnLastPossibleSlot);

                        var candidates1 = avnb1.AsLine().DisjointSet(currentCol);

                        foreach (var candidate in candidates1)
                        {
                            if (!boxLine.ContainsValue(candidate) &&
                            !currentRow.ContainsValue(candidate) &&
                            CheckForValueInCellOrRow(candidate,box,col1Index,row2Index,row3Index) &&
                            CheckForValueInCellOrRow(candidate,avnb2,col1Index,0,1,2)
                            )
                            {
                                return GetSolution(index,i,y,candidate,solverKind);
                            }

                        }
                        
                        var candidates2 = avnb2.AsLine().DisjointSet(currentCol);

                        foreach (var candidate in candidates2)
                        {
                            if (!boxLine.ContainsValue(candidate) &&
                            !currentRow.ContainsValue(candidate) &&
                            CheckForValueInCellOrRow(candidate,box,col1Index,row2Index,row3Index) &&
                            CheckForValueInCellOrRow(candidate,avnb1,col1Index,0,1,2)
                            )
                            {
                                return GetSolution(index,i,y,candidate,solverKind);
                            }

                        }
                    }

                    {
                        var solverKind = nameof(Strategy.RowLastPossibleSlot);

                        var candidates1 = ahnb1.AsLine().DisjointSet(currentRow);

                        foreach(var candidate in candidates1)
                        {
                            if (!boxLine.ContainsValue(candidate) &&
                                !currentCol.Segment.Contains(candidate) &&
                                CheckForValueInCellOrColumn(candidate,box,row1Index,col2Index,col3Index) &&
                                CheckForValueInCellOrColumn(candidate,ahnb2,row1Index,0,1,2)
                            )
                            {
                                return GetSolution(index,i,y,candidate,solverKind);

                            }
                        }

                        var candidates2 = ahnb2.AsLine().DisjointSet(currentRow);

                        foreach(var candidate in candidates2)
                        {
                            if (!boxLine.ContainsValue(candidate) &&
                                !currentCol.Segment.Contains(candidate) &&
                                CheckForValueInCellOrColumn(candidate,box,row1Index,col2Index,col3Index) &&
                                CheckForValueInCellOrColumn(candidate,ahnb1,row1Index,0,1,2)
                            )
                            {
                                return GetSolution(index,i,y,candidate,solverKind);

                            }
                        }
                    }

                    // Strategy where a column or row has two empty cells and one is constrained
                    {
                        var solverKind = nameof(Strategy.ColumnLastTwoPossibleSlots);

                        if (currentCol.GetUnsolvedCount() == 2)
                        {
                            var missingValues = currentCol.GetMissingValues();
                            for(int k = 0; k < 2; k++)
                            {
                                var value = missingValues[k];
                                var otherValue = missingValues[(k+1) % 2];
                                if (currentRow.ContainsValue(value) &&
                                    !boxLine.ContainsValue(otherValue))
                                    {
                                        return GetSolution(index, i, y, otherValue, solverKind);
                                    }
                            }
                        }
                    }

                    {
                        var solverKind = nameof(Strategy.RowLastTwoPossibleSlots);

                        if (currentRow.GetUnsolvedCount() == 2)
                        {
                            var missingValues = currentRow.GetMissingValues();
                            for(int k = 0; k < 2; k++)
                            {
                                var value = missingValues[k];
                                var otherValue = missingValues[(k+1) % 2];
                                if (currentCol.ContainsValue(value) &&
                                    !boxLine.ContainsValue(otherValue))
                                    {
                                        return GetSolution(index, i, y, otherValue, solverKind);
                                    }
                            }
                        }
                    }

                    {
                        var solverKind = nameof(Strategy.LastInRowOrColumn);
                        (var valuesFound, var values) = FindMissingValues(currentCol,currentRow);

                        for (var j = 1; j < 10;j++)
                        {
                            if (values[j])
                            {
                                continue;
                            }

                            var columnIndex = box.GetColumnOffsetForBox() + y;
                            var rowIndex = box.GetRowOffsetForBox() + i;
                            var foundSolution = 0;

                            // try with columns for the row
                            for (int l = 0; l < 9;l++)
                            {
                                var cell = currentRow[l];
                                if (cell != 0 || l == columnIndex)
                                {
                                    continue;
                                }

                                if (_puzzle.GetColumn(l).ContainsValue(j))
                                {
                                    foundSolution = j;
                                }
                                else
                                {
                                    foundSolution = 0;
                                    break;
                                }
                            }

                            if (foundSolution !=0)
                            {
                                return GetSolution(index,i,y,foundSolution, solverKind);
                            }

                            // try with rows for the column
                            for (int l = 0; l < 9;l++)
                            {
                                var cell = currentCol[l];
                                if (cell != 0 || l == columnIndex)
                                {
                                    continue;
                                }

                                if (_puzzle.GetRow(l).ContainsValue(j))
                                {
                                    foundSolution = j;
                                }
                                else
                                {
                                    foundSolution = 0;
                                    break;
                                }
                            }

                            if (foundSolution !=0)
                            {
                                return GetSolution(index,i,y,foundSolution, solverKind);
                            }
                        }


                    }

                    bool CheckRowForValue(int searchValue, Box box, int row)
                    {
                        // get complete row that includes current row of box
                        var rowIndex = box.GetRowOffsetForBox() + row;
                        //TODO: consider an overload that only returns non-zero values
                        var targetRow = _puzzle.GetRow(rowIndex);

                        return targetRow.Segment.Contains(searchValue);
                    }

                    bool CheckForValueInCellOrColumn(int value, Box box, int row, params int[] cols)
                    {
                        foreach(int col in cols)
                        {
                            var targetColumn = box.GetColumn(col);
                            if (targetColumn[row] != 0 ||
                                CheckColumnForValue(value, box, col))
                            {
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return true;
                    }

                    bool CheckForValueInCellOrRow(int value, Box box, int col, params int[] rows)
                    {
                        foreach(int row in rows)
                        {
                            var targetRow = box.GetRow(row);
                            if (targetRow[col] != 0 || 
                               CheckRowForValue(value, box, row))
                            {
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return true;
                    }

                    // assumes lines are of the same length
                    (int valuesFound, bool[] values) FindMissingValues(Line line1, Line line2)
                    {
                        int valuesFound = 0;
                        bool[] values = new bool[10];

                        for (int i = 0; i < line1.Length; i++)
                        {
                            Update(line1[i]);
                            Update(line2[i]);
                        }

                        return (valuesFound, values);

                        void Update(int value)
                        {
                            if (value != 0 && !values[value])
                            {
                                values[value] = true;
                                valuesFound++;
                            }
                        }
                    }

                    bool CheckColumnForValue(int searchValue, Box box, int col)
                    {
                        // get complete column that includes current column of box
                        var colIndex = box.GetColumnOffsetForBox() + col;
                        var targetCol = _puzzle.GetColumn(colIndex);

                        return targetCol.Segment.Contains(searchValue);
                    }

                    (bool solved, Solution solution) CheckForIntersectionCandidates(ReadOnlySpan<int> candidate1, ReadOnlySpan<int> candidate2, string solverKind)
                    {
                        var candidates = candidate1.Intersect(candidate2);
                        if (candidates.Length == 1)
                        {
                            return (true, GetSolution(index, i, y, candidates[0],solverKind));
                        }
                        return (false, Solution.False);
                    }
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
           
            return Solution.False;
        }
    }
}
