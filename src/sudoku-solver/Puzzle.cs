using System;
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

    public ReadOnlyMemory<int> GetBox(int i)
    {
        int start = i*9;
        return _puzzle.Slice(i*9,9);
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