using System;
using System.Collections.Generic;
using System.Linq;

namespace sudoku_solver
{
    public class HighestOccuringSolver : ISolver
    {
        private Puzzle _puzzle;

        public HighestOccuringSolver(Puzzle puzzle)
        {
            _puzzle = puzzle;
        }

        public IEnumerable<Solution> FindSolution()
        {
            for (int i = 0; i < 9; i++)
            {
                var solution = Solve(i);
                if (solution.Solved)
                {
                    yield return solution;
                }
            }
            yield return new Solution
            {
                Solved = false
            };
        }

        public bool IsEffective()
        {
            return true;
        }

        public Solution Solve(int index)
        {
            return SolveBox(index);
        }

        // find a box row/column with just one missing value
        // find a value that is present in the other two rows/columns but not this box
        // the value can be placed in the open spot if everything lines up
        private Solution SolveBox(int index)
        {
            var box = _puzzle.GetBox(index);

            // adjacent neightboring boxes
            // first two values in array are horizontal neighbors
            // second two values in array are vertical neighbors

            var offset = (index / 3) * 3;
            int[] avn = new int[]
            {
                (index + 1) % 3 + offset,
                (index + 2) % 3 + offset,
                (index + 3) % 9,
                (index + 6) % 9
            };

            var ahnb1 = _puzzle.GetBox(avn[0]);
            var ahnb2 = _puzzle.GetBox(avn[1]);
            var avnb1 = _puzzle.GetBox(avn[2]);
            var avnb2 = _puzzle.GetBox(avn[3]);

            // find intersection of values for adjacent rows
            for (int i = 0; i < 3; i++)
            {
                (var justOne, var candidateIndex) = box.GetRow(i).IsJustOneElementUnsolved();
                if (!justOne)
                {
                    continue;
                }
                
                // calculate indexes of adjacent rows
                var row1 = (i + 1) % 3;
                var row2 = (i + 2) % 3;

                //var firstRow = ahnb1.InsideRow.Segment.ToArray().Intersect(ahnb2.InsideRow.Segment.ToArray());
                var row1Values = ahnb1.GetRow(row1).Segment.ToArray().Union(ahnb2.GetRow(row1).Segment.ToArray());
                var row2Values = ahnb1.GetRow(row2).Segment.ToArray().Union(ahnb2.GetRow(row2).Segment.ToArray());
                var rowsValues = row1Values.Intersect(row2Values);
                var row0Values = ahnb1.GetRow(i).Segment.ToArray().Union(ahnb2.GetRow(i).Segment.ToArray());

                var rowCandidateValues = rowsValues.Where(x => !row0Values.Contains(x)).ToArray();

                if (rowCandidateValues.Length == 1)
                {
                    (var r, var c) = Puzzle.GetLocationForBoxCell(index, i * 3 + candidateIndex);
                    return new Solution
                    {
                        Solved = true,
                        Row = r,
                        Column = c,
                        Value = rowCandidateValues[0]
                    };
                }
            }

            // find intersection of values for adjacent columns
            for (int i = 0; i < 3; i++)
            {
                (var justOne, var candidateIndex) = box.GetColumn(i).IsJustOneElementUnsolved();
                if (!justOne)
                {
                    continue;
                }

                // calculate indexes of adjacent rows
                var col1 = (i + 1) % 3;
                var col2 = (i + 2) % 3;

                var col1Values = avnb1.GetColumn(col1).Segment.ToArray().Union(avnb2.GetColumn(col1).Segment.ToArray());
                var col2Values = avnb1.GetColumn(col2).Segment.ToArray().Union(avnb2.GetColumn(col2).Segment.ToArray());
                var colsValues = col1Values.Intersect(col2Values);
                var col0Values = avnb1.GetColumn(i).Segment.ToArray().Union(avnb2.GetColumn(i).Segment.ToArray());

                var colCandidateValues = colsValues.Where(x => !col0Values.Contains(x)).ToArray();

                if (colCandidateValues.Length == 1)
                {
                    (var r, var c) = Puzzle.GetLocationForBoxCell(index, (candidateIndex % 3) * 3 + i);
                    return new Solution
                    {
                        Solved = true,
                        Row = r,
                        Column = c,
                        Value = colCandidateValues[0]
                    };
                }
            }

            return new Solution
            {
                Solved = false
            };
        }
    }
}