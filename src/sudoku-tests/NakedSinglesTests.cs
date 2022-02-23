public class NakedSinglesTests
{
    // These tests use a puzzle from: 
    // http://sudopedia.enjoysudoku.com/Valid_Test_Cases.html
    // That page is licensed with the GNU Free Documentation License
    // https://www.gnu.org/copyleft/fdl.html

    // This puzzle is composed of "naked singles".
    // 3.542.81.4879.15.6.29.5637485.793.416132.8957.74.6528.2413.9.655.867.192.965124.8
    // It requires identifying just one remaining solution per unit (row, column, box).

    // Example:
    // Solved cell: r4:c3; 2
    // Solved by: NakedSinglesSolver
    //     *
    // 3 0 5 | 4 2 0 | 8 1 0
    // 4 8 7 | 9 0 1 | 5 0 6
    // 0 2 9 | 0 5 6 | 3 7 4
    // ------+-------+------
    // 8 5 2 | 7 9 3 | 0 4 1*
    // 6 1 3 | 2 0 8 | 9 5 7
    // 0 7 4 | 0 6 5 | 2 8 0
    // ------+-------+------
    // 2 4 1 | 3 0 9 | 0 6 5
    // 5 0 8 | 6 7 0 | 1 9 2
    // 0 9 6 | 5 1 2 | 4 0 8

    string _board = "3.542.81.4879.15.6.29.5637485.793.416132.8957.74.6528.2413.9.655.867.192.965124.8";
    string _completedBoard = "365427819487931526129856374852793641613248957974165283241389765538674192796512438";

    [Fact]
    public void FindFirstSolution()
    {
        Puzzle puzzle = new(_board);
        puzzle.AddSolver(new NakedSinglesSolver());
        Assert.True(puzzle.Solve(), "A solved solution should be returned.");
    }

    [Fact]
    public void SolvePuzzle()
    {
        Puzzle puzzle = new(_board);
        puzzle.AddSolver(new NakedSinglesSolver());
        Assert.True(puzzle.Solve() && puzzle.ToString() == _completedBoard, "Puzzle should  be solved.");
    }

    [Fact]
    public void ColumnTest()
    {
        var puzzle = new Puzzle(_board);
        var solver = new NakedSinglesSolver(puzzle);
        Assert.True(solver.TrySolveColumn(2, out Solution? solution), "Column should  be solved.");
    }

    [Fact]
    public void RowTest()
    {
        var board = "3.542.81.4879.15.6.29.56374852793.416132.8957.74.6528.2413.9.655.867.192.965124.8";
        var puzzle = new Puzzle(board);
        var solver = new NakedSinglesSolver(puzzle);
        Assert.True(solver.TrySolveRow(3, out Solution? solution), "Row should  be solved.");
    }

    [Fact]
    public void BoxTest()
    {
        var board = "3.542.81.4879.15.6.29.563748527936416132.8957.74.6528.2413.9.655.867.192.965124.8";
        var puzzle = new Puzzle(board);
        var solver = new NakedSinglesSolver(puzzle);
        Assert.True(solver.TrySolveBox(3, out Solution? solution), "Box should  be solved.");
    }
}
