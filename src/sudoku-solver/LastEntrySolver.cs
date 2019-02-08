using System;
using System.Collections.Generic;

public class LastEntrySolver
{
    private Puzzle _puzzle;
    public LastEntrySolver(Puzzle puzzle)
    {
        _puzzle = puzzle;
    }

    public IEnumerable<Solution> FindBoxSolutions()
    {
        for (int i = 0; i < 9; i++)
        {
            yield return SolveBox(i);
        }
    }

    public IEnumerable<Solution> FindColumnSolutions()
    {
        for (int i = 0; i < 9; i++)
        {
            yield return SolveColumn(i);
        }
    }

    public IEnumerable<Solution> FindRowSolutions()
    {
        for (int i = 0; i < 9; i++)
        {
            yield return SolveRow(i);
        }
    }


    private Solution SolveColumn(int index)
    {

        var solution = new Solution();
        solution.Box = index;

        var column = _puzzle.GetColumn(index);

        var values = new bool[10];
        var unsolvedCells = 0;
        var unsolvedCell = 99;

        for (int i = 0; i < 9; i++)
        {
            int value = column[i];
            if (value > 0)
            {
                values[value] = true;
            }
            else
            {
                unsolvedCells++;
                unsolvedCell = i;
            }
            if (unsolvedCells > 1)
            {
                solution.Solved = false;
                return solution;
            }
        }

        if (unsolvedCells == 0)
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
        solution.Cell = unsolvedCell;
        return solution;
    }

    private Solution SolveRow(int index)
    {
        var solution = new Solution();
        solution.Box = index;

        var row = _puzzle.GetRow(index);

        var values = new bool[10];
        var unsolvedCells = 0;
        var unsolvedCell = 99;

        for (int i = 0; i <9;i++)
        {
            int value = row[i];
            if (value > 0)
            {
                values[value] = true;
            }
            else
            {
                unsolvedCells++;
                unsolvedCell = i;
            }
            if (unsolvedCells > 1)
            {
                solution.Solved = false;
                return solution;
            }
        }

        if (unsolvedCells == 0)
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
        solution.Cell = unsolvedCell;
        return solution;
    }

    private Solution SolveBox(int index)
    {
        var solution = new Solution();
        solution.Box= index;

        var box = _puzzle.GetBox(index);


        var values = new bool[10];
        var unsolvedCells = 0;
        var unsolvedCell = 99;
        for (int i = 0; i < 9; i++)
        {
            int value;
            if (i < 3)
            {
                value = box.FirstRow[i];
            }
            else if (i <6)
            {
                value = box.InsideRow[i-3];
            }
            else
            {
                value = box.LastRow[i-6];
            }

            if (value > 0)
            {
                values[value] = true;
            }
            else
            {
                unsolvedCells++;
                unsolvedCell = i;
            }
            if (unsolvedCells > 1)
            {
                solution.Solved = false;
                return solution;
            }
        }
        if (unsolvedCells == 0)
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
        solution.Cell = unsolvedCell;
        return solution;
    }
 }