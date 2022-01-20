public class PointedPairsTests
{
    string _board = "017903600000080000900000507072010430000402070064370250701000065000030000005601720";

    [Fact]
    public void FindFirstSolution()
    {
        Puzzle puzzle = new(_board);
        ICandidateSolver solver = new PointingMultiplesCandidateSolver();
        puzzle.UpdateCandidates();
        bool solved = solver.TryFindCandidates(puzzle, out Candidates? candidates);
        Assert.True(solved && candidates is not null && candidates[9][0] == 3, "A solved solution should be returned.");
    }
}