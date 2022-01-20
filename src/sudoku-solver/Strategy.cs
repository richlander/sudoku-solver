namespace sudoku_solver;

// Strategy is modelled after CompilerFeature
// https://github.com/dotnet/roslyn/blob/master/src/Test/Utilities/Portable/Traits/CompilerFeature.cs
public enum Strategy
{
    RowSolver,
    ColumnSolver,
    RowColumnSolver,
    RowCandidateRowBlockedColumnSolver,
    ColumnCandidateColumnBlockedRowSolver,
    ColumnCandidateColumnBlocked,
    RowCandidateRowBlocked,
    ColumnLastPossibleSlot,
    RowLastPossibleSlot,
    RowLastTwoPossibleSlots,
    ColumnLastTwoPossibleSlots,
    LastInRowOrColumn
}
