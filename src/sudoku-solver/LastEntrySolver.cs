using System;
using System.Collections.Generic;

public class LastEntrySolver : ISolver
{
    private Puzzle _puzzle;
    public LastEntrySolver(Puzzle puzzle)
    {
        _puzzle = puzzle;
    }

    public IEnumerable<Solution> FindSolutions()
    {
        for (int i = 0; i < 9; i++)
        {
            if (_puzzle.SolvedForBox[i] == 8) yield return SolveBox(i);
            if (_puzzle.SolvedForRow[i] == 8) yield return SolveRow(i);
            if (_puzzle.SolvedForColumn[i] == 8) yield return SolveColumn(i);
        }
    }

    public bool CheckEffective()
    {
        for (int i = 0; i < 9; i++)
        {
            if (_puzzle.SolvedForBox[i] == 8) return true;
            if (_puzzle.SolvedForRow[i] == 8) return true;
            if (_puzzle.SolvedForColumn[i] == 8) return true;
        }
        return false;
    }

    private Solution SolveColumn(int index)
    {

        var solution = new Solution();
        solution.Index = index;
        solution.Type = SequenceType.Column;

        var column = _puzzle.GetColumn(index);
        return SolveLine(column, solution);
    }

    private Solution SolveRow(int index)
    {
        var solution = new Solution();
        solution.Index = index;
        solution.Type = SequenceType.Row;

        var row = _puzzle.GetRow(index);
        return SolveLine(row, solution);
    }

    private Solution SolveLine(Line line, Solution solution)
    {
        var values = new bool[10];
        var unsolvedCells = 0;
        var unsolvedCell = 99;

        for (int i = 0; i <9;i++)
        {
            int value = line.Segment[i];
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
        solution.Type = SequenceType.Box;
        solution.Index= index;

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