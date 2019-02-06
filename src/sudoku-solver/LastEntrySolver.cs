using System;
using System.Collections.Generic;

public class LastEntrySolver
{
    private Puzzle _puzzle;
    private int[] _boxCounts = new int[9];
    private int Box = 0;
    public LastEntrySolver(Puzzle puzzle)
    {
        _puzzle = puzzle;
    }

    public IEnumerable<Solution> GetSolutions()
    {
        UpdateSolutionCounts();
        for (int i = 0; i < 9; i++)
        {
            yield return SolveBox(i);
        }
    }

    private Solution SolveBox(int index)
    {
        var solution = new Solution();
        solution.Box= index;
        if (_boxCounts[index] < 8)
        {
            solution.Solved = false;
            return solution;
        }
        else if (_boxCounts[index] == 9)
        {
            solution.Solved = false;
            return solution;
        }

        var box = _puzzle.GetBox(index).Span;
        var valuesFound = new bool[10];
        var emptyCell = 0;
        for (int i = 0; i < 9; i++)
        {
            var value = box[i];
            if (value > 0)
            {
                valuesFound[value] = true;
            }
            else
            {
                emptyCell = i;
            }
        }
        for (int i = 1; i < 10; i++)
        {
            if (!valuesFound[i])
            {
                solution.Value = i;
                break;
            }
        }
        solution.Solved = true;
        solution.Cell = emptyCell;
        return solution;
    }

    private void UpdateSolutionCounts()
    {
        for (int i = 0; i < 9; i++)
        {
            var box = _puzzle.GetBox(i).Span;

            var solved = 0;
            for (int j = 0; j < 9; j++)
            {
                var value = box[j];
                if (value > 0)
                {
                    solved++;
                }
            }
            _boxCounts[i] = solved;
        }
    }
 }