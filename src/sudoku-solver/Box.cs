using System;
using System.Diagnostics;

namespace sudoku_solver;

public ref struct Box
{
    private Puzzle _puzzle;
    private int _index;
    private int _firstCellIndex;
    private int _firstRowIndex;
    private int[] _horizontalNeighbors;
    private int[] _verticalNeighbors;

    public Box(Puzzle puzzle, int index)
    {
        _puzzle = puzzle;
        _firstCellIndex = Puzzle.GetFirstCellIndexForBox(index);
        _firstRowIndex = Puzzle.GetFirstRowIndexForBox(index);
        _index = index;
        _horizontalNeighbors = Puzzle.GetBoxIndexesForHorizontalNeighbors(index);
        _verticalNeighbors = Puzzle.GetBoxIndexesForVerticalNeighbors(index);
    }

    public Puzzle Puzzle => _puzzle;
    public Line FirstRow => GetRowLine(0);
    public Line InsideRow => GetRowLine(1);
    public Line LastRow => GetRowLine(2);
    public Line FirstColumn => GetColumnLine(0);
    public Line InsideColumn => GetColumnLine(1);
    public Line LastColumn => GetColumnLine(2);
    public Box FirstHorizontalNeighbor => _puzzle.GetBox(_horizontalNeighbors[0]);
    public Box SecondHorizontalNeighbor => _puzzle.GetBox(_horizontalNeighbors[1]);
    public Box FirstVerticalNeighbor => _puzzle.GetBox(_verticalNeighbors[0]);
    public Box SecondVerticalNeighbor => _puzzle.GetBox(_verticalNeighbors[1]);

    public int this[int i] => i switch
    {
        < 3 => FirstRow[i],
        < 6 => InsideRow[i-3],
        < 9 => LastRow[i-6],
        _ => throw new ArgumentException()
    };

    public int GetUnsolvedCount() => 
            FirstRow.GetUnsolvedCount() +
            InsideRow.GetUnsolvedCount() +
            LastRow.GetUnsolvedCount();

    public int CountValidSolved()
    {
        var values = new bool[10];
        if (CountLine(FirstRow, out int firstRowCount) &&
            CountLine(InsideRow, out int insideRowCount) &&
            CountLine(LastRow, out int lastRowCount))
            {
                return firstRowCount + insideRowCount + lastRowCount;
            }

        return -1;

        bool CountLine(Line line, out int count)
        {
            count = 0;
            ReadOnlySpan<int> segment = line.Segment;
            for (int i = 0; i < segment.Length; i++)
            {
                int value = segment[i];
                if (value is >= 1 and <= 9)
                {
                    if (values[value])
                    {
                        return false;
                    }
                    values[value] = true;
                    count++;
                }
                else if (value != Puzzle.UnsolvedMarker)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public bool[] GetValues()
    {
        var values = new bool[10];
        for(int i = 0; i < 9; i++)
        {
            int value = i switch
            {
                < 3 => FirstRow[i],
                < 6 => InsideRow[i-3],
                < 9 => LastRow[i-6],
                _ => throw new ArgumentException()
            };

            if (values[value] && value != 0)
            {
                throw new Exception("Something went wrong");
            }
            else
            {
                values[value] = true;
            }
        }
        return values;
    }

    public Line AsLine() => new Line(
        new int[]
        {
            FirstRow[0],
            FirstRow[1],
            FirstRow[2],
            InsideRow[0],
            InsideRow[1],
            InsideRow[2],
            LastRow[0],
            LastRow[1],
            LastRow[2]});

    public ReadOnlySpan<int> AsValues()
    {
        var values = new int[9];
        int count = 0;
        count = ProcessValues(FirstRow.Segment, count);
        count = ProcessValues(InsideRow.Segment, count);
        count = ProcessValues(LastRow.Segment, count);

        return values[0..count];

        int ProcessValues(ReadOnlySpan<int> values0, int count)
        {
            foreach (int value in values0)
            {
                if (value == 0 || values.AsSpan(0..count).Contains(value))
                {
                    continue;
                }
                values[count] = value;
                count++;
            }
            return count;
        }
    }

    // Rows
    public static int FirstRowIndex(int index) => (index / 3) * 3;

    public Line GetRowLine(int index) => new Line(GetRow(index));
    
    public ReadOnlySpan<int> GetRow(int index) => _puzzle.Board.Slice(_firstCellIndex + (9 * index), 3);

    // Columns
    public Line GetColumnLine(int index)
    {
        var offset = _firstCellIndex + index;

        return new Line
        {
            Segment = new int[] 
            {
                _puzzle[offset],
                _puzzle[offset + 9],
                _puzzle[offset + 18]
            }
        };
    }

    // Cells
    public int GetRowIndexForCell(int index) => _firstRowIndex + (index / 3);

    public int GetColumnIndexForCell(int index) => Puzzle.GetFirstColumnIndexForBox(index) + (index % 3);

    // Other
    public (int row, int column) GetLocation(int cell) => 
        (GetRowIndexForCell(cell), GetColumnIndexForCell(cell));
}
