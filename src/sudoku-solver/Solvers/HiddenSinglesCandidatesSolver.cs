using sudoku_solver_extensions;

namespace sudoku_solver;

// Hidden singles: One logically absent value in a row or column, based on 
// values being present in an adjacent row or column.
// Example:
// Solved cell: r3:c1; 8
// Solved by: HiddenSinglesSolver:RowSolver
//  *
//  0 0 2 | 0 3 0 | 0 0 8
//  0 0 0 | 0 0 8 | 0 0 0
//  8 3 1 | 0 2 0 | 0 0 0*
// ..2.3...8.....8....31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..

public class HiddenSinglesCandidatesSolver : ICandidateSolver
{
    public bool TryFindCandidates(Puzzle puzzle, [NotNullWhen(true)] out Candidates? candidates)
    {
        bool candidatesFound = false;
        candidates = new();
        for(int i = 0; i < 9; i++)
        {
            candidatesFound |= TryFindBox(i, puzzle, candidates);
        }

        return candidatesFound;
    }

    private bool TryFindBox(int index, Puzzle puzzle, Candidates hiddenSinglesCandidates)
    {
        Candidates candidates = puzzle.Candidates;
        bool candidatesFound = false;

        // Get baseline data about box
        Box box = puzzle.GetBox(index);
        ReadOnlySpan<int> boxPositions = Puzzle.GetPositionsForBox(index);
        int[] horizontalNeighbors = Puzzle.GetBoxIndexesForHorizontalNeighbors(index);
        int[] verticalNeighbors = Puzzle.GetBoxIndexesForVerticalNeighbors(index);

        // Used to pivot on which rows should be checked, later
        ReadOnlySpan<int> allRows = new int[] {0, 1, 2};

        // look for a candidate being unique in one cell in a box row
        // iterate over 3 rows
        for (int i = 0; i < 3; i++)
        {
            // Need to first validate that the box row has unsolved cells
            if (box.IsRowSolved(i))
                {
                    continue;
                }
            
            int[] rowIndices = box.GetRowIndices(i);
            int rowStartIndex = rowIndices[0];
            // Determine candidates for neighbors box rows (for same i)
            // Collect candidates from neighbors (2 rows * 3 cells * 9 possible candidates)
            HashSet<int> neighborBoxCandidates = new(54);
            foreach(int neighbor in horizontalNeighbors)
            {
                ReadOnlySpan<int> neighborBoxPositions = Puzzle.GetPositionsForBox(neighbor);

                // get cells in neighbor box for same row
                foreach(int np in neighborBoxPositions.Slice(i * 3, 3))
                {
                    // Only looking at unsolved cells
                    if (puzzle[np] > 0)
                    {
                        continue;
                    }

                    neighborBoxCandidates.AddRange(candidates[np]);
                }
            }

            // Now we need to determine if there are unique candidates per row cell
            for (int j = 0; j < 3; j++)
            {
                int cellPosition = rowStartIndex + j;
                // validate that call is unsolved
                if (puzzle[cellPosition] > 0)
                {
                    continue;
                }

                // Collect candidates from neighbors (2 cells * 9 possible candidates)
                HashSet<int> neighborCellCandidates = new(18);

                // iterate over other rows
                for (int k = 1; k < 3; k++)
                {
                    int neighborCellIndex = (j + k) % 3;
                    int neighborCellPosition = rowIndices[neighborCellIndex];

                    if (puzzle[neighborCellPosition] > 0)
                    {
                        continue;
                    }

                    neighborCellCandidates.AddRange(candidates[neighborCellPosition]);
                }

                ReadOnlySpan<int> cellCandidates = candidates[cellPosition];
                ReadOnlySpan<int> uniqueCandidatesForCellPerNeighborBoxes = cellCandidates.Except(neighborBoxCandidates);

                if (uniqueCandidatesForCellPerNeighborBoxes.Length == 0)
                {
                    continue;
                }

                ReadOnlySpan<int> uniqueCandidatesForCell = uniqueCandidatesForCellPerNeighborBoxes.Except(neighborCellCandidates);

                if (uniqueCandidatesForCell.Length == 0)
                {
                    continue;
                }

                candidatesFound = true;
                hiddenSinglesCandidates.AddCandidates(cellPosition, uniqueCandidatesForCell);
            }
        }

        return candidatesFound;
    }
}