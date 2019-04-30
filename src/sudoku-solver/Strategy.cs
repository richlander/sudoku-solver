using System;
using System.Reflection;

namespace sudoku_solver
{
    // Strategy is modelled after CompilerFeature
    // https://github.com/dotnet/roslyn/blob/master/src/Test/Utilities/Portable/Traits/CompilerFeature.cs
    public enum Strategy
    {
        RowSolver,
        ColumnSolver,
        RowColumnSolver,
        Row2BlockedRow3CandidateColumnSolver,
        Row2CandidateRow3BlockedColumnSolver,
        Column2CandidateColumn3BlockedRowSolver,
        Column2BlockedColumn3CandidateRowSolver,
        Column2CandidateColumn3Blocked,
        ColumnLastPossibleSlot,
        RowLastPossibleSlot
    }
}