public class LastEmptySquare
{
    // These tests use a puzzle from: 
    // http://sudopedia.enjoysudoku.com/Valid_Test_Cases.html
    // That page is licensed with the GNU Free Documentation License
    // https://www.gnu.org/copyleft/fdl.html

    // This puzzle has one empty square.
    // 2564891733746159829817234565932748617128.6549468591327635147298127958634849362715

    // Example:
    // Solved cell: r5:c5; 3
    // Solved by: NakedSinglesSolver
    //         *
    // 2 5 6 | 4 8 9 | 1 7 3
    // 3 7 4 | 6 1 5 | 9 8 2
    // 9 8 1 | 7 2 3 | 4 5 6
    // ------+-------+------
    // 5 9 3 | 2 7 4 | 8 6 1
    // 7 1 2 | 8 3 6 | 5 4 9*
    // 4 6 8 | 5 9 1 | 3 2 7
    // ------+-------+------
    // 6 3 5 | 1 4 7 | 2 9 8
    // 1 2 7 | 9 5 8 | 6 3 4
    // 8 4 9 | 3 6 2 | 7 1 5

    string _board = "2564891733746159829817234565932748617128.6549468591327635147298127958634849362715";
    string _completeBoard = "256489173374615982981723456593274861712836549468591327635147298127958634849362715";

    [Fact]
    public void IsPuzzleComplete()
    {
        var puzzle = new Puzzle(_board);
        Assert.False(puzzle.IsSolved, "Puzzle should not be solved.");
    }

    [Fact]
    public void CompletePuzzle()
    {
        var puzzle = new Puzzle(_board);
        puzzle.AddSolver(new NakedSinglesSolver());
        Assert.True(puzzle.SolvePuzzle() && puzzle.ToString() == _completeBoard, "Puzzle should be solved.");
    }
}
