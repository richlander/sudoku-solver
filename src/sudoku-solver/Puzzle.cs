using System;
using System.Collections.Generic;

public class Puzzle
{
    private Memory<int> _puzzle;
    public ReadOnlyMemory<int> Values = new int[] {1,2,3,4,5,6,7,8,9};
    public int UnsolvedMarker = 0;
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

    public bool IsFull()
    {
        int count = 0;
        var puzzle = _puzzle.Span;
        for (int i =0; i < puzzle.Length; i++)
        {
            if (puzzle[i] == UnsolvedMarker)
            {
                count++;
            }
        }
        return count == puzzle.Length;
    }

    public Line GetRow(int index)
    {
        var start = index * 9;

        return new Line
        {
            Segment = _puzzle.Slice(start, 9).Span 
        };
    }

    public Line GetColumn(int index)
    {
        var column = new int[9];
        var grid = _puzzle.Span;
        for (int i = 0;  i < 9; i++)
        {
            var cIndex = i + (i * 9);
            column[i] = grid[cIndex];
        }
        
        return new Line
        {
            Segment = column
        };
    }

    public Box GetBox(int index)
    {
        int start = (index / 3) * 27 + (index % 3) * 3;

        var box = new Box
        {
            FirstRow = _puzzle.Slice(start, 3).Span,
            InsideRow = _puzzle.Slice(start + 9, 3).Span,
            LastRow = _puzzle.Slice(start + 18, 3).Span
        };

        return box;
    }

    public void Update(Solution solution)
    {
        var element = solution.Box*9 + cell;
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