using System.Collections.Generic;
using System.Linq;

namespace sudoku_solver
{
    public class HighestOccuringAdjacentSolver : ISolver
    {
        private Puzzle _puzzle;

        public HighestOccuringAdjacentSolver(Puzzle puzzle)
        {
            _puzzle = puzzle;
        }

        public bool IsEffective()
        {
            return true;
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
        }

        public Solution Solve(int index)
        {
            var box = _puzzle.GetBox(index);

            // adjacent neighboring boxes
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
                (var rowJustOne, var rowCandidateIndex) = box.GetRow(i).IsJustOneElementUnsolved();
                if (!rowJustOne)
                {
                    continue;
                }

                // calculate indexes of adjacent rows
                var row1 = (i + 1) % 3;
                var row2 = (i + 2) % 3;

                var row1Values = ahnb1.GetRow(row1).Segment.ToArray().Union(ahnb2.GetRow(row1).Segment.ToArray());
                var row2Values = ahnb1.GetRow(row2).Segment.ToArray().Union(ahnb2.GetRow(row2).Segment.ToArray());
                var rowsValues = row1Values.Intersect(row2Values);
                var row0Values = ahnb1.GetRow(i).Segment.ToArray().Union(ahnb2.GetRow(i).Segment.ToArray());

                var rowCandidateValues = rowsValues.Where(x => !row0Values.Contains(x)).ToArray();

                if (rowCandidateValues.Length == 1)
                {
                    (var r, var c) = Puzzle.GetLocationForBoxCell(index, i * 3 + rowCandidateIndex);
                    return new Solution
                    {
                        Solved = true,
                        Row = r,
                        Column = c,
                        Value = rowCandidateValues[0]
                    };
                }

                // find intersection of values for adjacent columns
                (var colJustOne, var candidateIndex) = box.GetColumn(i).IsJustOneElementUnsolved();
                if (!colJustOne)
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

        private int GetHighestOccuringValueNotInBox(Box box, int[] avn)
        {
            var line = box.AsLine();
            var avnb1 = _puzzle.GetBox(avn[0]).AsLine();
            var avnb2 = _puzzle.GetBox(avn[1]).AsLine();
            var ahnb1 = _puzzle.GetBox(avn[2]).AsLine();
            var ahnb2 = _puzzle.GetBox(avn[3]).AsLine();
            var values = new int[10];

            for (int i = 1; i < 10; i++)
            {
                values[avnb1[i]]++;
                values[avnb2[i]]++;
                values[ahnb1[i]]++;
                values[ahnb2[i]]++;
            }

            var highestValue = 0;
            for (int i = 1; i < 10; i++)
            {
                if (values[i] > values[highestValue] && !line.ContainsValue(values[i]))
                {
                    highestValue = i;
                }
            }

            return highestValue;
        }
    }
}
