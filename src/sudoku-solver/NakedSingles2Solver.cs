using System;
using System.Collections.Generic;
using System.Linq;

namespace sudoku_solver
{
    public class NakedSingles2Solver : ISolver
    {
        private Puzzle _puzzle;

        public NakedSingles2Solver(Puzzle puzzle)
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
            for (int i = 0; i < 9; i++)
            {
                var box = _puzzle.GetBox(i);
                for (int y = 0; y < 3; y++)
                {
                    (var rowJustOne, var rowCandidateIndex) = box.GetRow(y).IsJustOneElementUnsolved();
                    (var colJustOne, var candidateIndex) = box.GetColumn(y).IsJustOneElementUnsolved();

                    if (rowJustOne || colJustOne)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // find a box row/column with just one missing value
        // find a value that is present in the other two rows/columns but not this box
        // the value can be placed in the open spot if everything lines up
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
                var row2Index = (i + 1) % 3;
                var row3Index = (i + 2) % 3;

                // baseline box
                var row2 = box.GetRow(row2Index);
                var row3 = box.GetRow(row3Index);

                // horizontal adjacent box 1 -- rows
                var ahnb1Row2 = ahnb1.GetRow(row2Index);
                var ahnb1Row3 = ahnb1.GetRow(row3Index);

                // horizontal adjacent box 2 -- rows
                var ahnb2Row2 = ahnb2.GetRow(row2Index);
                var ahnb2Row3 = ahnb2.GetRow(row3Index);

                // get complete row that includes the the first row of box
                var firstRowIndex = box.GetRowOffsetForBox() + i;
                var firstRow = _puzzle.GetRow(firstRowIndex);

                // determine union of values of rows
                var row2Union = ahnb1Row2.Union(ahnb2Row2);
                var row3Union = ahnb1Row3.Union(ahnb2Row3);

                // the boundaries of the adjacent boxes don't matter
                // the appropriate rows have been merged (Union) at this point

                // determine union of values of row 1 -- these values are all off-limits

                // determine disjoint set with baseline rows -- box row values are off-limits
                var row2Values = row2Union.DisjointSet(row2.Segment);
                var row3Values = row3Union.DisjointSet(row3.Segment);

                // determine disjoint set with baseline row -- looking for values now in that row
                var row2Candidates = row2Values.DisjointSet(firstRow.Segment);
                var row3Candidates = row3Values.DisjointSet(firstRow.Segment);

                // row candidates -- values in both row 2 and 3 but not in row 1 or in the box
                var rowCandidates = row2Candidates.Intersect(row3Candidates);

                if (rowCandidates.Length == 1)
                {
                    (var r, var c) = Puzzle.GetLocationForBoxCell(index, i * 3 + rowCandidateIndex);
                    return new Solution
                    {
                        Solved = true,
                        Row = r,
                        Column = c,
                        Value = rowCandidates[0],
                        Solver = this
                    };
                }

                // find intersection of values for adjacent columns
                (var colJustOne, var candidateIndex) = box.GetColumn(i).IsJustOneElementUnsolved();
                if (!colJustOne)
                {
                    continue;
                }

                var col1Index = i;
                var col2Index = (i + 1) % 3;
                var col3Index = (i + 2) % 3;

                // baseline box
                var col1 = box.GetColumn(col1Index);
                var col2 = box.GetColumn(col2Index);
                var col3 = box.GetColumn(col3Index);

                // vertical adjacent box 1 -- cols
                var avnb1Col1 = avnb1.GetColumn(col1Index);
                var avnb1Col2 = avnb1.GetColumn(col2Index);
                var avnb1Col3 = avnb1.GetColumn(col3Index);

                // vertical adjacent box 2 -- cols
                var avnb2Col1 = avnb2.GetColumn(col1Index);
                var avnb2Col2 = avnb2.GetColumn(col2Index);
                var avnb2Col3 = avnb2.GetColumn(col3Index);

                // get complete column that includes the the first column of box
                var firstColIndex = box.GetColumnOffsetForBox() + i;
                var firstCol = _puzzle.GetColumn(firstColIndex);

                // determine union of values of cols
                var col2Union = avnb1Col2.Union(avnb2Col2);
                var col3Union = avnb1Col3.Union(avnb2Col3);

                // determine disjoint set with baseline cols -- box col values are off-limits
                var col2Values = col2Union.DisjointSet(col2.Segment);
                var col3Values = col3Union.DisjointSet(col3.Segment);

                // determine disjoint set with baseline col -- looking for values now in that col
                var col2Candidates = col2Values.DisjointSet(firstCol.Segment);
                var col3Candidates = col3Values.DisjointSet(firstCol.Segment);

                // col candidates -- values in both col 2 and 3 but not in col 1 or in the box
                var colCandidates = col2Candidates.Intersect(col3Candidates);


                if (colCandidates.Length == 1)
                {
                    (var r, var c) = Puzzle.GetLocationForBoxCell(index, (candidateIndex % 3) * 3 + i);
                    return new Solution
                    {
                        Solved = true,
                        Row = r,
                        Column = c,
                        Value = colCandidates[0],
                        Solver = this
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