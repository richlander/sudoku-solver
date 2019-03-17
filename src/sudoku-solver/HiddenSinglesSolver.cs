using System;
using System.Collections.Generic;
using System.Linq;

namespace sudoku_solver
{
    public class HiddenSinglesSolver : ISolver
    {
        private Puzzle _puzzle;

        public HiddenSinglesSolver(Puzzle puzzle)
        {
            _puzzle = puzzle;
        }

        public IEnumerable<Solution> FindSolution()
        {
            for (int i = 0; i < 9; i++)
            {
                if (IsBoxEffective(i).effective) yield return SolveBox(i);
            }        
        }

        public bool IsEffective()
        {
            for (int i = 0; i < 9; i++)
            {
                (var effective, var rows) = IsBoxEffective(i);
                if (effective)
                {
                    return true;
                }
            } 
            return false;
        }

        public (bool effective, bool[] effectiveRows) IsBoxEffective(int index)
        {
            var box = _puzzle.GetBox(index);
            var effective = new bool[]
            {
                box.FirstRow.IsJustOneElementUnsolved().justOne,
                box.InsideRow.IsJustOneElementUnsolved().justOne,
                box.LastRow.IsJustOneElementUnsolved().justOne
            };

            return ((effective[0] || effective[1] || effective[2]), effective);
        }

        // look for a row with only one element unsolved
        // find the set of missing numbers in the box
        // Determine if the other two rows both have those missing numbers
        // If so, there should be just one candidate
        // Repeat for columns
        private Solution SolveBox(int index)
        {
            (var effective, var effectiveRows) = IsBoxEffective(index);

            if (!effective)
            {
                return new Solution
                {
                    Solved = false
                };
            }

            var box = _puzzle.GetBox(index);
            
            // calculate other two boxes to check
            var box2Index = 0;
            var box3Index = 0;
            var offset = index % 3;
            if (offset == 0)
            {
                box2Index = index +1;
                box3Index = index +2;
            }
            else if (offset == 1)
            {
                box2Index = index - 1;
                box3Index = index + 1;
            }
            else if (offset == 2)
            {
                box2Index = index - 1;
                box3Index = index - 2;
            }

            var box2 = _puzzle.GetBox(box2Index);
            var box3 = _puzzle.GetBox(box3Index);
            var values = box.GetValues();

            for (int i = 0; i < 3;i++)
            {
                if (!effectiveRows[i])
                {
                    continue;
                }

                var row = new Line();
                IList<int> row1 = null;
                IList<int> row2 = null;
                if (i == 0)
                {
                    row = box.FirstRow;
                    row1 = FindDisjointValues(values, box2.InsideRow, box3.InsideRow);
                    row2 = FindDisjointValues(values, box2.LastRow, box3.LastRow);
                }
                else if (i == 1)
                {
                    row = box.InsideRow;
                    row1 = FindDisjointValues(values, box2.FirstRow, box3.FirstRow);
                    row2 = FindDisjointValues(values, box2.LastRow, box3.LastRow);
                }
                else if (i == 2)
                {
                    row = box.LastRow;
                    row1 = FindDisjointValues(values, box2.FirstRow, box3.FirstRow);
                    row2 = FindDisjointValues(values, box2.InsideRow, box3.InsideRow);
                }

                var commonValues = row1.Intersect(row2);

                if (commonValues.Count() == 1)
                {
                    var value = commonValues.First();
                    var position = FindFirstMissingValue(row);
                    (var r, var c) = Puzzle.GetLocationForBoxCell(index,i *3 + position);
                    
                    return new Solution
                    {
                        Solved = true,
                        Value = value,
                        Row = r,
                        Column = c
                    };
                }
            }
            return new Solution
            {
                Solved = false
            };
        }

        private int FindFirstMissingValue(Line line)
        {
            for (int i = 0; i < line.Segment.Length;i++)
            {
                if (line[i] == Puzzle.UnsolvedMarker)
                {
                    return i;
                }
            }
            return 99;
        }

        private IList<int> FindDisjointValues(bool[] values, Line line1, Line line2)
        {
            var disjointValues = new List<int>();

            FindValues(line1);
            FindValues(line2);

            return disjointValues;
            
            void FindValues(Line line)
            {
                for (int i = 0; i < line.Segment.Length;i++)
                {
                    var value = line[i];
                    if (!values[value])
                    {
                        disjointValues.Add(value);
                    }
                }
            }
        }

        private int GetFirstRowForBox(int index)
        {
            return (index / 3) * 3;
        }

        private int GetFirstColumnForBox(int index)
        {
            return (index % 3) *3;
        }
    }
}