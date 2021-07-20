using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;
using sudoku_solver;

List<string> tests = new()
{
    "994236158638591742125487936316754289742918563589362417867125394253649871491873625",
    "974236158638591742125487936316754289742918563589362417867125394253649871491873625",
    "2564891733746159829817234565932748617128.6549468591327635147298127958634849362715",
    "3.542.81.4879.15.6.29.5637485.793.416132.8957.74.6528.2413.9.655.867.192.965124.8",
    "..5.9.1841.85742.6...812.57..4.51.6251246.87.6..28.541...12.4....1.3872.32..4.61.",
    "..2.3...8.....8....31.2.....6..5.27..1.....5.2.4.6..31....8.6.5.......13..531.4..",
    ".94...13..............76..2.8..1.....32.........2...6.....5.4.......8..7..63.4..8",
    "7.5.2...1...5..3...31..9.82.5...6...69.....75...2...3.51.8..49...3..1...8...3.1.6",
    "6...1.5.2...639....8........3....7..8..2.1..4..1....9........5....387...4.8.9...6",
    "3...8...1.6..2..9...5.9.7.....479.......1.....296.548..3.....6..5.....1..1426357.",
};

Puzzle puzzle = new(tests[tests.Count -1]);

List<ISolver> solvers = new()
{
    new NakedSinglesSolver(puzzle),
    new HiddenSinglesSolver(puzzle),
    new NakedMultiplesSolver(puzzle)
};

SolverSet solver = new(solvers);

if (puzzle.IsSolved())
{
    WriteLine("Puzzle is solved!");
}

while (solver.TrySolve(out Solution solution))
{
    if (!puzzle.Update(solution))
    {
        WriteLine("Something is busted!");
        break;
    }
    string solverKind = solution.SolverKind is null ? $"{solution.Solver}" : $"{solution.Solver}:{solution.SolverKind}";
    WriteLine($"Solved cell: {solution.GetLocation()}; {solution.Value}");
    WriteLine($"Solved by: {solverKind}");
    
    puzzle.DrawPuzzle(solution);
    WriteLine();

    if (puzzle.IsSolved())
    {
        WriteLine("Puzzle is solved!");
        break;
    }
}

WriteLine($"Solved cells: {puzzle.Solved}; Remaining: {81 - puzzle.Solved}");
WriteLine(puzzle);

WriteLine($"{solver.SolutionCount} solutions found.");
//WriteLine($"{iterations} solutions attempted.");
