using System;
using System.Reflection;

namespace sudoku_solver
{
    public enum Strategy
    {
        RowSolver,
        ColumnSolver,
        RowColumnSolver,
        Row2BlockedRow3CandidateColumnSolver,
        Row2CandidateRow3BlockedColumnSolver,
        Column2CandidateColumn3BlockedRowSolver,
        Column2BlockedColumn3CandidateRowSolver,
        Column2CandidateColumn3Blocked
    }
}