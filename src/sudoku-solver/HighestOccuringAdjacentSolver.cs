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
            // too hard to tell w/o doing all the work of the solution
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

            // get adjacent neighboring boxes
            var ahnb1 = _puzzle.GetBox(avn[0]);
            var ahnb2 = _puzzle.GetBox(avn[1]);
            var avnb1 = _puzzle.GetBox(avn[2]);
            var avnb2 = _puzzle.GetBox(avn[3]);

            // iterate over the three rows in the box
            for (int i = 0; i < 3; i++)
            {
                // for each row, the neighboring rows are the same
                // the columns differ per cell
                var row1Index = i;
                var row2Index = (i + 1) % 3;
                var row3Index = (i + 2) % 3;

                // there are nine row to consider (three boxes)

                // baseline box
                var row1 = box.GetRow(row1Index);
                var row2 = box.GetRow(row2Index);
                var row3 = box.GetRow(row3Index);

                // horizontal adjacent box 1 -- rows
                var ahnb1Row1 = ahnb1.GetRow(row1Index);
                var ahnb1Row2 = ahnb1.GetRow(row2Index);
                var ahnb1Row3 = ahnb1.GetRow(row3Index);

                // horizontal adjacent box 2 -- rows
                var ahnb2Row1 = ahnb2.GetRow(row1Index);
                var ahnb2Row2 = ahnb2.GetRow(row2Index);
                var ahnb2Row3 = ahnb2.GetRow(row3Index);

                // determine union of values of rows
                var row1Union = ahnb1Row1.Union(ahnb2Row1);
                var row2Union = ahnb1Row2.Union(ahnb2Row2);
                var row3Union = ahnb1Row3.Union(ahnb2Row3);

                // determine intersection of values of row 1
                var row1Values = row1Union.Intersect(row1.Segment);

                // determine disjoint set with baseline rows
                var row2Values = row2Union.DisjointSet(row2.Segment);
                var row3Values = row3Union.DisjointSet(row3.Segment);

                // determine disjoint set with baseline row
                var row2Candidates = row2Values.DisjointSet(row1Values);
                var row3Candidates = row3Values.DisjointSet(row1Values);

                // row candidates
                var rowCandidates = row2Candidates.Intersect(row3Candidates); 

                if (rowCandidates.Length != 1)
                {
                    continue;
                }

                // cells
                for (int y = 0; y < 3; y++)
                {
                    var col1Index = (index / 3) * 3;
                    var col2Index = index + (i + 1) % 3;
                    var col3Index = index + (i + 2) % 3;

                    // baseline box
                    var col1 = box.GetRow(col1Index);
                    var col2 = box.GetRow(col2Index);
                    var col3 = box.GetRow(col3Index);

                    // vertical adjacent box 1 -- cols
                    var avnb1Col1 = avnb1.GetColumn(col1Index);
                    var avnb1Col2 = avnb1.GetColumn(col2Index);
                    var avnb1Col3 = avnb1.GetColumn(col3Index);

                    // vertical adjacent box 2 -- cols
                    var avnb2Col1 = avnb2.GetColumn(col1Index);
                    var avnb2Col2 = avnb2.GetColumn(col2Index);
                    var avnb2Col3 = avnb2.GetColumn(col3Index);

                    // determine intersection of values of cols
                    var col1Intersection = avnb1Col1.Intersect(avnb2Col1);
                    var col2Intersection = avnb1Col2.Intersect(avnb2Col2);
                    var col3Intersection = avnb1Col3.Intersect(avnb2Col3);

                    // determine intersection of values of col 1
                    var col1Values = col1Intersection.Intersect(col1.Segment);

                    // determine disjoint set with baseline cols
                    var col2Values = col2Intersection.DisjointSet(col2.Segment);
                    var col3Values = col3Intersection.DisjointSet(col3.Segment);

                    // determine disjoint set with baseline col
                    var col2Candidates = col2Values.DisjointSet(col1Values);
                    var col3Candidates = col3Values.DisjointSet(col1Values);

                    // col candidates
                    var colCandidates = col2Candidates.Intersect(col3Candidates);

                    if (colCandidates.Length == 1 && colCandidates[0] == row2Candidates[0])
                    {
                        return new Solution
                        {
                            Solved = true
                        };
                    }
                }
            }
           
            return new Solution
            {
                Solved = false
            };
        }
    }
}
