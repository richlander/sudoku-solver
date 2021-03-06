using System;
using System.Collections.Generic;

namespace sudoku_solver
{
    public class NakedSinglesSolver : ISolver
    {
        private Puzzle _puzzle;
        private readonly int _effectiveCount = 8;
        public NakedSinglesSolver(Puzzle puzzle)
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

        public Solution Solve(int index)
        {
            if (IsBoxEffective(index)) return SolveBox(index);
            if (IsColumnEffective(index)) return SolveColumn(index);
            if (IsRowEffective(index)) return SolveRow(index);
            return new Solution 
            {
                Solved = false
            };
        }

        public bool IsEffective()
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

        public bool IsBoxEffective(int box) => _puzzle.SolvedForBox[box] == _effectiveCount;
        public bool IsRowEffective(int row) => _puzzle.SolvedForRow[row] == _effectiveCount;
        public bool IsColumnEffective(int column) => _puzzle.SolvedForColumn[column] == _effectiveCount;
        
        public Solution SolveColumn(int index)
        {
            var line = _puzzle.GetColumn(index);
            (bool solved, int cell, int value) = SolveLine(line);
            
            var solution = new Solution
            {
                Solved = solved
            };

            if (solved)
            {
                solution.Column = index;
                solution.Row = cell;
                solution.Value = value;
                solution.Solver = this;
                return solution;
            }

            return solution;
        }

        public Solution SolveRow(int index)
        {
            var line = _puzzle.GetRow(index);
            (bool solved, int cell, int value) = SolveLine(line);

            var solution = new Solution
            {
                Solved = solved
            };

            if (solved)
            {
                solution.Column = cell;
                solution.Row = index;
                solution.Value = value;
                solution.Solver = this;
                return solution;
            }

            return solution;
        }

        private (bool solved, int cell, int value) SolveLine(Line line)
        {
            var values = new bool[10];
            var unsolvedCells = 0;
            var unsolvedCell = 0;
            var unknownValue = (false, 0, 0);

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
                    return unknownValue;
                }
            }

            if (unsolvedCells == 0)
            {
                return unknownValue;
            }

            for (int i = 1; i < 10; i++)
            {
                if (!values[i])
                {
                    return (true, unsolvedCell, i);
                }
            }

            return unknownValue;
        }

        public Solution SolveBox(int index)
        {
            var box = _puzzle.GetBox(index);

            var solution = new Solution
            {
                Solved = false
            };
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
                    return solution;
                }
            }
            
            if (unsolvedCells == 0)
            {
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

            (var row, var column) = Puzzle.GetLocationForBoxCell(index, unsolvedCell);

            solution.Solved = true;
            solution.Row = row;
            solution.Column = column;
            solution.Solver = this;
            return solution;
        }
    }
}
