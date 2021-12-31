namespace sudoku_solver;

public partial class Puzzle
{
    public Line GetRow(int row)
    {
        var start = GetFirstCellIndexForRow(row);
        return new Line(_board.AsSpan().Slice(start, 9));
    }

    public Line GetColumn(int column)
    {
        var col = new int[9];
        for (int i = 0;  i < 9; i++)
        {
            var cell = column + (i * 9);
            col[i] = _board[cell];
        }
        
        return new Line(col);
    }

    public Box GetBox(int index) => new Box(this, index);

    public Line GetBoxAsLine(int index)
    {
        var box = GetBox(index);
        return new Line(new int[]
        {
            box.FirstRow[0],
            box.FirstRow[1],
            box.FirstRow[2],
            box.InsideRow[0],
            box.InsideRow[1],
            box.InsideRow[2],
            box.LastRow[0],
            box.LastRow[1],
            box.LastRow[2]
        });
    }
}