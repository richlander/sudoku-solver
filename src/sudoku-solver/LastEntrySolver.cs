using System;
using System.Collections.Generic;

namespace sudoku_solver
{
    public class LastEntrySolver : ISolver
    {
        private Puzzle _puzzle;
        private readonly int _effectiveCount = 8;
        public LastEntrySolver(Puzzle puzzle)
        {
            _puzzle = puzzle;
        }

        public IEnumerable<Solution> FindSolution()
        {
            for (int i = 0; i < 9; i++)
            {
                if (IsBoxEffective(i)) yield return SolveBox(i);
                if (IsColumnEffective(i)) yield return SolveColumn(i);
                if (IsRowEffective(i)) yield return SolveRow(i);
            }
        }

        public bool CheckEffective()
        {
            for (int i = 0; i < 9; i++)
            {
                if (IsBoxEffective(i)    ||
                    IsColumnEffective(i) ||
                    IsRowEffective(i))
                    {
                        return true;
                    }
            }
            return false;
        }

        private bool IsBoxEffective(int box) => _puzzle.SolvedForBox[box] == _effectiveCount;
        private bool IsRowEffective(int row) => _puzzle.SolvedForRow[row] == _effectiveCount;
        private bool IsColumnEffective(int column) => _puzzle.SolvedForColumn[column] == _effectiveCount;
        private Solution SolveColumn(int column)
        {

            var solution = new Solution();
            solution.Column = column;
            solution.Type = SequenceType.Column;

            var column = _puzzle.GetColumn(column);
            return SolveLine(column, solution);
        }

        private Solution SolveRow(int row)
        {
            var solution = new Solution();
            solution.Column = row;
            solution.Type = SequenceType.Row;

            var row = _puzzle.GetRow(row);
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

        private Solution SolveBox(int box)
        {
            var solution = new Solution();
            solution.Type = SequenceType.Box;
            solution.Column= box;

            var box = _puzzle.GetBox(box);


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
}
