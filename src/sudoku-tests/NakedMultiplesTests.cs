public class NakedMultiplesTests
{
    // These tests use a puzzle from: 
    // http://sudopedia.enjoysudoku.com/Valid_Test_Cases.html
    // That page is licensed with the GNU Free Documentation License
    // https://www.gnu.org/copyleft/fdl.html

    // This puzzle is composed of "naked multiples".
    // 79458213626..31.......76..268.71.32443286......724386..2.6574.3.4.1286.7.763942.8
    // It requires removing candidates based on cells in the same unit with only those candidates.
    // For example, two cells might have only 5 and 9 as candidates. 
    // That means that 5 and 9 can be removed as candidates from all other cells in the unit. 

    // Example:
    // Solved cell: r2:c7; 7
    // Solved by: NakedMultiplesSolver:
    //                 *
    // 7 9 4 | 5 8 2 | 1 3 6
    // 2 6 0 | 0 3 1 | 7 0 0*
    // 0 0 0 | 0 7 6 | 0 0 2
    // ------+-------+------
    // 6 8 0 | 7 1 0 | 3 2 4
    // 4 3 2 | 8 6 0 | 0 0 0
    // 0 0 7 | 2 4 3 | 8 6 0
    // ------+-------+------
    // 0 2 0 | 6 5 7 | 4 0 3
    // 0 4 0 | 1 2 8 | 6 0 7
    // 0 7 6 | 3 9 4 | 2 0 8
    // 79458213626..317......76..268.71.32443286......724386..2.6574.3.4.1286.7.763942.8

    string _board = "79458213626..31.......76..268.71.32443286......724386..2.6574.3.4.1286.7.763942.8";
    string _nextSolution = "79458213626..317......76..268.71.32443286......724386..2.6574.3.4.1286.7.763942.8";

    [Fact]
    public void FindNextSolution()
    {
        Puzzle puzzle = new(_board);
        puzzle.AddSolver(new NakedMultiplesSolver());
        Assert.True(puzzle.TrySolve(out Solution? solution) && puzzle.ToString() == _nextSolution, "A solved solution should be returned.");
    }
}
