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
                //TODO: consider an overload that only returns non-zero values
                var currentRow = _puzzle.GetRow(currentRowIndex);

                // get all values in box
                var boxLine = box.AsLine();
                //var boxValues = box.AsValues();

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
                        if (boxCol1[i] == 0)
                        {
                            var solverKind = nameof(Strategy.RowColumnSolver);
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
                        var solverKind1 = nameof(Strategy.Column2CandidateColumn3BlockedRowSolver);

                        // row1[col3] has a value, so col3 don't need to participate in providing a candidate
                        if (col2Candidates.Length > 0 &&
                            boxRow1[col2Index] == 0 && boxRow1[col3Index] != 0 &&
                            ((solved, solution) = CheckForIntersectionCandidates(boxRow1Candidates, col2Candidates, solverKind1)).solved)
                        {
                            return solution;
                        }

                        var solverKind2 = nameof(Strategy.Column2BlockedColumn3CandidateRowSolver);
                        // row1[col2] has a value, so col2 don't need to participate in providing a candidate
                        if (col3Candidates.Length > 0 &&
                            boxRow1[col2Index] != 0 && boxRow1[col3Index] == 0 &&
                            ((solved, solution) = CheckForIntersectionCandidates(boxRow1Candidates, col3Candidates, solverKind2)).solved)
                        {
                            return solution;
                        }
                    }

                    // check columns
                    if (boxCol1Candidates.Length > 0)                    
                    {
                        bool solved;
                        Solution solution;
                        var solver1 = nameof(Strategy.Row2CandidateRow3BlockedColumnSolver);
                        // col1[row3] has a value, so row3 don't need to participate in providing a candidate
                        if (row2Candidates.Length > 0 &&
                            boxCol1[row2Index] == 0 && boxCol1[row3Index] != 0 &&
                            ((solved, solution) = CheckForIntersectionCandidates(boxCol1Candidates, row2Candidates, solver1)).solved)
                        {
                            return solution;
                        }

                        var solver2 = nameof(Strategy.Row2BlockedRow3CandidateColumnSolver);
                        // col1[row2] has a value, so row2 don't need to participate in providing a candidate
                        // test: SolutionTwoSlotsEmptyInColumn
                        if (row3Candidates.Length > 0 &&
                            boxCol1[row2Index] != 0 && boxCol1[row3Index] == 0 &&
                            ((solved, solution) = CheckForIntersectionCandidates(boxCol1Candidates, row3Candidates, solver2)).solved)
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
                        if (col3Blocked && 
                            col2Candidates.Length == 1)
                        {
                            var solverKind = nameof(Strategy.Column2CandidateColumn3Blocked);
                            return GetSolution(index, i, y, col2Candidates[0],solverKind);
                        }
                    }

                    // following set of cases test hidden singles in columns or rows

                    // pattern
                    // box 1 col1 is full
                    // box 2 has one value not in col1
                    // only one slot available in col1 in current box
                    // means that current cell must have that value

                    {
                         
                        var solverCol = nameof(Strategy.ColumnLastPossibleSlot);
                        bool solved;
                        Solution solution;

                        // start with columns
                        // test each cell in avnb1.col1
                        // avnb2 presents candidate
                        if (//avnb1Col1.GetUnsolvedCount() == 0 &&
                            ((solved, solution) = CheckForDisjointCandidates(avnb2.AsLine().Segment, currentCol.Segment, solverCol)).solved &&
                            //!currentCol.ContainsValue(solution.Value) &&
                            !boxLine.ContainsValue(solution.Value) &&
                            CheckForNonZeroValueOrRowHasValue(solution.Value, boxCol1[row2Index], box, row2Index) &&
                            CheckForNonZeroValueOrRowHasValue(solution.Value, boxCol1[row3Index], box, row3Index) &&
                            CheckBoxForRowValues(solution.Value,avnb1,col1Index,0,1,2)
                        )
                        {
                            return solution;
                        }

                        // test avnb2.col1 solved
                        // avnb1 presents candidate
                        if (//avnb2Col1.GetUnsolvedCount() == 0 &&
                            ((solved, solution) = CheckForDisjointCandidates(avnb1.AsLine().Segment, currentCol.Segment, solverCol)).solved &&
                            //!currentCol.ContainsValue(solution.Value) &&
                            !boxLine.ContainsValue(solution.Value) &&
                            CheckForNonZeroValueOrRowHasValue(solution.Value, boxCol1[row2Index], box, row2Index) &&
                            CheckForNonZeroValueOrRowHasValue(solution.Value, boxCol1[row3Index], box, row3Index)&&
                            CheckBoxForRowValues(solution.Value,avnb2,col1Index,0,1,2)
                            )
                        {
                            return solution;
                        }

                        /* 

                        // try rows

                        // test ahnb1.col1 solved
                        // ahnb2 presents candidate
                        var solverRow = nameof(Strategy.RowLastPossibleSlot);
                        if (ahnb1Row1.GetUnsolvedCount() == 0 &&
                            ((solved, solution) = CheckForDisjointCandidates(ahnb2.AsLine().Segment, currentRow.Segment, solverRow)).solved &&
                            !currentRow.ContainsValue(solution.Value) &&
                            !boxLine.ContainsValue(solution.Value) &&
                            CheckForNonZeroValueOrColHasValue(boxRow1[col2Index], solution.Value, box, col2Index) &&
                            CheckForNonZeroValueOrColHasValue(boxRow1[col3Index], solution.Value, box, col3Index))

                        {
                            return solution;
                        }

                        // test ahnb2.col1 solved
                        // ahnb1 presents candidate
                        if (ahnb2Row1.GetUnsolvedCount() == 0 &&
                            boxRow1.IsJustOneElementUnsolved().justOne &&
                            ((solved, solution) = CheckForDisjointCandidates(ahnb1.AsLine().Segment, currentRow.Segment, solverRow)).solved &&
                            !currentRow.ContainsValue(solution.Value) &&
                            !boxLine.ContainsValue(solution.Value))
                        {
                            return solution;
                        }
                        */
                    }

                    // pattern
                    // box 1 col1 is full
                    // box 2 has one value not in col1
                    // multiple slots available in col1 in current box
                    // other slots have the same value filled from a row or column
                    // means that current cell must have that value

/*                
                    {
                        var solverCol = nameof(Strategy.ColumnLastPossibleSlot);
                        bool solved;
                        Solution solution;

                        // test avnb1.col1 solved
                        // avnb2 presents candidate
                        if (((solved, solution) = CheckForDisjointCandidates(avnb2.AsLine().Segment, currentCol.Segment, solverCol)).solved &&
                            !currentCol.ContainsValue(solution.Value) &&
                            !boxLine.ContainsValue(solution.Value) &&
                            CheckForNonZeroValueOrRowHasValue(solution.Value, boxCol1[row2Index], box, row2Index) &&
                            CheckForNonZeroValueOrRowHasValue(solution.Value, boxCol1[row3Index], box, row3Index) &&
                            CheckBoxForRowValues(solution.Value,ahnb1,i,0,1,2))
                        {
                            return solution;
                        }

                        // test avnb2.col1 solved
                        // avnb1 presents candidate
                        if (avnb2Col1.GetUnsolvedCount() == 0 &&
                            ((solved, solution) = CheckForDisjointCandidates(avnb1.AsLine().Segment, currentCol.Segment, solverCol)).solved &&
                            !currentCol.ContainsValue(solution.Value) &&
                            !boxLine.ContainsValue(solution.Value) &&
                            CheckForNonZeroValueOrRowHasValue(solution.Value, boxCol1[row2Index], box, row2Index) &&
                            CheckForNonZeroValueOrRowHasValue(solution.Value, boxCol1[row3Index], box, row3Index))
 
                        {
                            return solution;
                        }
                    }
                    */

                    bool CheckBoxForRowValues(int searchValue, Box box, int row, params int[] indices)
                    {
                        var targetRow = box.GetRow(row);
                        foreach(int index in indices)
                        {
                            if (!CheckForNonZeroValueOrColHasValue(searchValue, targetRow[index], box, i))
                            {
                                return false;
                            }
                        }
                        return true;
                    }

                    bool CheckForNonZeroValueOrRowHasValue(int searchValue, int cellValue, Box box, int row)
                    {
                        // get complete row that includes current row of box
                        var rowIndex = box.GetRowOffsetForBox() + row;
                        //TODO: consider an overload that only returns non-zero values
                        var targetRow = _puzzle.GetRow(rowIndex);

                        if (cellValue != 0)
                        {
                            return true;
                        }

                        if (targetRow.Segment.Contains(searchValue))
                        {
                            return true;
                        }

                        return false;
                    }

                    bool CheckForNonZeroValueOrColHasValue(int searchValue, int cellValue, Box box, int col)
                    {
                        // get complete row that includes current row of box
                        var colIndex = box.GetColumnOffsetForBox() + col;
                        //TODO: consider an overload that only returns non-zero values
                        var targetCol = _puzzle.GetColumn(colIndex);


                        if (cellValue != 0)
                        {
                            return true;
                        }

                        if (targetCol.Segment.Contains(searchValue))
                        {
                            return true;
                        }

                        return false;
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

                    (bool solved, Solution solution) CheckForDisjointCandidates(ReadOnlySpan<int> line, ReadOnlySpan<int> candidate2, string solverKind)
                    {
                        var candidates = line.DisjointSet(candidate2);
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
           
            return new Solution
            {
                Solved = false
            };
        }
    }
}
