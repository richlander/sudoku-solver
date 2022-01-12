namespace sudoku_solver;

public partial class Puzzle
{
    // Box positions
    public static int GetBoxIndexForCell(int index) => (index / 27) * 3 + (index / 3) % 3;

    public static int GetFirstCellIndexForBox(int index) => (index / 3) * 27 + (index % 3) * 3;

    public static int GetBoxIndex(int row, int column) => (row / 3) * 3 + (column % 3);

   public static (int row, int column) GetLocationForBoxCell(int box, int index)
    {
        var boxStart = GetFirstCellIndexForBox(box);
        var boxCell = boxStart + index / 3 * 9 + index % 3;

        var row = boxCell / 9;
        var column = boxCell % 9;
        return (row,column);
    }

    public static int[] GetPositionsForBox(int index)
    {
        int[] positions = new int[9];
        int startingRowCell = GetFirstCellIndexForBox(index);
        int cellIndex = 0;
        for (int i = 0; i < 3; i++)
        {
            int startingCell = startingRowCell + (9 * i);
            positions[cellIndex++] = startingCell;
            positions[cellIndex++] = startingCell + 1;
            positions[cellIndex++] = startingCell + 2;
        }

        return positions; 
    }

    public static int GetFirstRowIndexForBox(int index) => (index / 3) * 3;

    public static int GetFirstColumnIndexForBox(int index) => (index % 3) * 3;

    public static int[] GetBoxIndexesForHorizontalNeighbors(int index)
    {
        int firstRowIndex = GetFirstRowIndexForBox(index);
        int[] neighbors = new int[2];

        for (int i = 1; i < 3; i++)
        {
            neighbors[i - 1] = (index + i) % 3 + firstRowIndex;
        }
        return neighbors;
    }

    public static int[] GetBoxIndexesForVerticalNeighbors(int index)
    {
        int[] neighbors = new int[2];

        for (int i = 1; i < 3; i++)
        {
            neighbors[i - 1] = (index + i * 3) % 9;
        }
        return neighbors;
    }

    // Row positions
    public static int GetRowIndexForCell(int index) => index / 9;
    public static int GetFirstCellIndexForRow(int index) => index * 9;

    public static int[] GetPositionsForRow(int index)
    {
        int startingRowCell = GetFirstCellIndexForRow(index);
        return Enumerable.Range(startingRowCell, 9).ToArray();
    }

    // Column positions
    public static int GetColumnIndexForCell(int index) => (index % 9);
    
    public static int GetFirstCellIndexForColumn(int index) => (index / 3) + (index % 3);

    public static int[] GetPositionsForColumn(int index)
    {
        int[] positions = new int[9];
        positions[0] = GetFirstCellIndexForColumn(index);
        for (int i = 1; i < 9; i++)
        {
            positions[i] = positions[i-1] + 9;
        }
        return positions;
    }

}