using System;
using System.Collections.Generic;

namespace sudoku_solver
{
    public static class PuzzleTest
    {
        public static IEnumerable<Puzzle> Get()
        {
            var tests = new List<string>()
            {
                "2564891733746159829817234565932748617128.6549468591327635147298127958634849362715",
                "3.542.81.4879.15.6.29.5637485.793.416132.8957.74.6528.2413.9.655.867.192.965124.8",
                "..2.3...8.....8....31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..",
                ".94...13..............76..2.8..1.....32.........2...6.....5.4.......8..7..63.4..8"
            };

            foreach(var test in tests)
            {
                var puzzle = new int[81];
                var index = 0;
                foreach(var c in test)
                {
                    var value = 0;
                    if (c != '.')
                    {
                        value = (int)c - (int)'0';
                    }
                    puzzle[index] = value;
                    index++;
                }
                var p = Puzzle.ReadPuzzle(puzzle);
                yield return p;
            }
        }
    }
}