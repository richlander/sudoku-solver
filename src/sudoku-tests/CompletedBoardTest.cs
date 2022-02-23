public class CompletedTest
{
    // These tests use a puzzle from: 
    // http://sudopedia.enjoysudoku.com/Valid_Test_Cases.html
    // That page is licensed with the GNU Free Documentation License
    // https://www.gnu.org/copyleft/fdl.html

    // The targeted puzzle is already complete and should be detected as such
    // Puzzle: 974236158638591742125487936316754289742918563589362417867125394253649871491873625

    string _board = "974236158638591742125487936316754289742918563589362417867125394253649871491873625";

    [Fact]
    public void CheckCompletedPuzzle()
    {
        var puzzle = new Puzzle(_board);
        Assert.True(puzzle.IsSolved,"Puzzle should be solved.");
    }

    [Fact]
    public void AttemptToSolveCompletedPuzzle()
    {
        var puzzle = new Puzzle(_board);
        puzzle.AddSolver(new NakedSinglesSolver());
        Assert.False(puzzle.TrySolve(out Solution? solution), "No more solutions should be available.");
        Assert.True(puzzle.Solve(),"Puzzle should be solved.");
    }
}
