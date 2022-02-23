public class PositionTests
{
    [Fact]
    public void GetBoxIndexForCell()
    {
        int[] cells = {0, 1, 3, 4, 8, 9, 28, 54, 80};
        int[] boxes = {0, 0, 1, 1, 2, 0, 3, 6, 8};

        for (int i = 0; i < cells.Length; i++)
        {
            Assert.True(Puzzle.GetBoxIndexForCell(cells[i]) == boxes[i]);
        }
    }

    [Fact]
    public void GetFirstCellIndexForBox()
    {
        int[] firstCell = new int[] {0, 3, 6, 27, 30, 33, 54, 57, 60};

        for (int i = 0; i < firstCell.Length; i++)
        {
            Assert.True(Puzzle.GetFirstCellIndexForBox(i) == firstCell[i]);
        }
    }

    [Fact]
    public void GetBoxIndex()
    {
        // diagonal, left to right
        int[] box = new int[] {0, 0, 0, 4, 4, 4, 8, 8, 8};
        
        for (int i = 0; i < box.Length; i++)
        {
            Assert.True(Puzzle.GetBoxIndex(i, i) == box[i]);
        }

        // diagonal, right to left
        int[] box2 = new int[] {2, 2, 2, 4, 4, 4, 6, 6, 6};

        for (int i = 0; i < box.Length; i++)
        {
            Assert.True(Puzzle.GetBoxIndex(i, 8 - i) == box2[i]);
        }
    }

    [Fact]
    public void GetLocationForBoxCell()
    {
        // diagonly navigate 9 cells
        int[] row    = new int[] {0, 0, 0, 4, 4, 4, 8, 8, 8};
        int[] column = new int[] {0, 4, 8, 0, 4, 8, 0, 4, 8};

        for (int i = 0; i < row.Length; i++)
        {
            Assert.True(Puzzle.GetLocationForBoxCell(i, i) == (row[i], column[i]));
        }
    }

    [Fact]
    public void GetPositionsForBox()
    {
        Dictionary<int, int[]> tests = new()
        {
            {0, new int[]{0, 1, 2, 9, 10, 11, 18, 19, 20}},
            {1, new int[]{3, 4, 5, 12, 13, 14, 21, 22, 23}},
            {2, new int[]{6, 7, 8, 15, 16, 17, 24, 25, 26}},
            {3, new int[]{27, 28, 29, 36, 37, 38, 45, 46, 47}},
            {4, new int[]{30, 31, 32, 39, 40, 41, 48, 49, 50}},
            {5, new int[]{33, 34, 35, 42, 43, 44, 51, 52, 53}},
            {6, new int[]{54, 55, 56, 63, 64, 65, 72, 73, 74}},
            {7, new int[]{57, 58, 59, 66, 67, 68, 75, 76, 77}},
            {8, new int[]{60, 61, 62, 69, 70, 71, 78, 79, 80}},
        };

        foreach(int test in tests.Keys)
        {
            Assert.True(Puzzle.GetPositionsForBox(test).Sum() == tests[test].Sum());
        }
    }

    [Fact]
    public void GetFirstRowIndexForBox()
    {
        int[] firstRow = new int[] {0, 0, 0, 3, 3, 3, 6, 6, 6};

        for (int i = 0; i < firstRow.Length; i++)
        {
            Assert.True(Puzzle.GetFirstRowIndexForBox(i) == firstRow[i]);
        }
    }

    [Fact]
    public void GetFirstColumnIndexForBox()
    {
        int[] firstColumn = new int[] {0, 3, 6, 0, 3, 6, 0, 3, 6};

        for (int i = 0; i < firstColumn.Length; i++)
        {
            Assert.True(Puzzle.GetFirstColumnIndexForBox(i) == firstColumn[i]);
        }
    }

    [Fact]
    public void GetBoxIndexesForHorizontalNeighbors()
    {
        Dictionary<int, int[]> tests = new()
        {
            {0, new int[] {1, 2}},
            {1, new int[] {2, 0}},
            {2, new int[] {0, 1}},
            {3, new int[] {4, 5}},
            {7, new int[] {8, 6}},
            {8, new int[] {6, 7}}
        };

        foreach (int test in tests.Keys)
        {
            int[] neighbors = Puzzle.GetBoxIndexesForHorizontalNeighbors(test);
            int[] expected = tests[test];
            for (int i = 0; i < 2; i++)
            {
                Assert.True(neighbors[i] == expected[i]);
            }
        }
    }
}
