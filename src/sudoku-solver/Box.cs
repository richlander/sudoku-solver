using System;
using System.Diagnostics;

namespace sudoku_solver;

public ref struct Box
{
    private Puzzle _puzzle;
    private int _cellOffset;
    private int _rowOffset;
    private int _index;

    public Box(Puzzle puzzle, int index)
    {
        _puzzle = puzzle;
        _cellOffset = GetCellOffset(index);
        _rowOffset = (index / 3) * 3;
        _index = index;
    }

    public Puzzle Puzzle => _puzzle;
    public Line FirstRow => GetRow(0);
    public Line InsideRow => GetRow(1);
    public Line LastRow => GetRow(2);
    public Line FirstColumn => GetColumn(0);
    public Line InsideColumn => GetColumn(1);
    public Line LastColumn => GetColumn(2);
    public Box FirstHorizontalNeighbor => _puzzle.GetBox((_index + 1) % 3 + _rowOffset);
    public Box SecondHorizontalNeighbor => _puzzle.GetBox((_index + 2) % 3 + _rowOffset);
    public Box FirstVerticalNeighbor => _puzzle.GetBox((_index + 3) % 9);
    public Box SecondVerticalNeighbor => _puzzle.GetBox((_index + 6) % 9);

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

    public Line GetRow(int index) => new Line(_puzzle.Board.Slice(_cellOffset + (9 * index), 3));
    
    public Line GetColumn(int index)
    {
        var offset = _cellOffset + index;

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

    public int GetRowOffset() => (_index / 3) * 3;

    public int GetColumnOffset() => (_index % 3) * 3;

    private static int GetCellOffset(int index)
    {
        var offset = (index / 3) * 27 + (index % 3) * 3;
        if (offset > 80)
        {
            throw new Exception();
        }
        return offset;
    }

    public static int GetFirstCellForBox(int index) => GetCellOffset(index);

    public int GetRowOffsetForCell(int index) => GetRowOffset() + (index / 3);

    public int GetColumnOffsetForCell(int index) => GetColumnOffset() + (index % 3);

    public (int row, int column) GetLocation(int cell) => 
        (GetRowOffsetForCell(cell), GetColumnOffsetForCell(cell));
}
