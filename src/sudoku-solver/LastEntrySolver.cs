using System;
using System.Collections.Generic;

public class LastEntrySolver
{
    private Puzzle _puzzle;
    public LastEntrySolver(Puzzle puzzle)
    {
        _puzzle = puzzle;
    }

    public IEnumerable<Solution> GetSolutions()
    {
        for (int i = 0; i < 9; i++)
        {
            yield return SolveBox(i);
        }
    }

    private Solution SolveBox(int index)
    {
        var solution = new Solution();
        solution.Box= index;

        var box = _puzzle.GetBox(index).Span;
        var values = new bool[10];
        var emptyCells = 0;
        var emptyCell = 0;
        for (int i = 0; i < 9; i++)
        {
            var value = box[i];
            if (value > 0)
            {
                values[value] = true;
            }
            else
            {
                emptyCell = i;
                emptyCells++;
            }
            if (emptyCells > 1)
            {
                solution.Solved = false;
                return solution;
            }
        }
        if (emptyCells == 0)
        {
            solution.Solved = false;
            return solution;
        }
        for (int i = 1; i < 10; i++)
        {
            if (!values[i])
            {
                solution.Value = i;
                break;
            }
        }
        solution.Solved = true;
        solution.Cell = emptyCells;
        return solution;
    }
 }