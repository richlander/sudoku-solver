using System;
using System.Collections.Generic;

public class Puzzle
{
    private Memory<int> _puzzle;
    public ReadOnlyMemory<int> Values = new int[] {1,2,3,4,5,6,7,8,9};
    public Puzzle(Memory<int> puzzle)
    {
        if (puzzle.Length == 81)
        {
            _puzzle = puzzle;
        }
        else
        {
            throw new Exception();
        }
    }

    public Span<int> GetRow(int index)
    {
        var start = index * 9;

        return _puzzle.Slice(start, 9).Span;
    }

    public int[] GetColumn(int index)
    {
        var column = new int[9];
        var grid = _puzzle.Span;
        for (int i = 0;  i < 9; i++)
        {
            var cIndex = i + (i * 9);
            column[i] = grid[cIndex];
        }
        return column;
    }

    public Box GetBox(int index)
    {
        /*
        1       2       3 
        0-2     3-5     6-8
        9-11    12-14   15-17
        18-20   21-23   24-26

        4       5       6
        27-29   30-32   33-35
        36-38   39-41   42-44
        45-47   48-50   41-53

        7       8       9
        54-56   57-59   60-52
        63-65   66-68   69-71   
        72-74   75-77   78-80

        0*9 + 0*3  -- (offset *9) + (i-1)*3 + (row*3)
        0*9 + 1*3

        3*9 + 0*3
        3*9 + 1*3

        6*9 + 0*3
        6*9 + 1*3
         
        */
        int start = (index / 3) * 27 + (index % 3) * 3;

        var box = new Box
        {
            FirstRow = _puzzle.Slice(start, 3).Span,
            InsideRow = _puzzle.Slice(start + 9, 3).Span,
            LastRow = _puzzle.Slice(start + 18, 3).Span
        };

        return box;
    }

    public void UpdateCell(int box, int cell, int value)
    {
        var element = box*9 + cell;
        var puzzle = _puzzle.Span;
        if (puzzle[element] == 0)
        {
            puzzle[element] = value;
        }
        else
        {
            throw new Exception();
        }
    }
}