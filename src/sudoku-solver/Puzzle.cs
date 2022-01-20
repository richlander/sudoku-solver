global using ROSi = System.ReadOnlySpan<int>;
global using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace sudoku_solver;

// Puzzle is the entrypoint and hub for solving Sudoku puzzles.
// It exposes high-level APIs for consumers.
// It exposes low-level APIs for solvers.
// It exposes the following major concepts for solvers:
// - Board -- the board that solvers operate on.
// - Candidates -- solvers primarily remove candidates.
// - Solution -- a new found solution.
// - ISolver -- the interface that solvers must implement.
public partial class Puzzle
{
    public static readonly int UnsolvedMarker = 0;
    private int[] _board;
    private Candidates? _candidates;
    public const int TotalsCells = 81;
    public int InitialSolved {get; private set;}
    public int Solved {get; private set;}
    public int[] SolvedForBox {get; private set;}
    public int[] SolvedForRow {get; private set;}
    public int[] SolvedForColumn {get; private set;}

    public Puzzle(string board) : this(ReadPuzzleAsString(board))
    {
    }
    public Puzzle(int[] board)
    {
        if (board.Length != TotalsCells)
        {
            throw new Exception("Puzzle incorrect length");
        }

        _board = board;
        if (!IsValid())
        {
            throw new Exception("Puzzle is not valid.");
        }

        UpdateCounts();
    }

    public int this[int i] => _board[i];

    public ReadOnlySpan<int> Board => _board.AsSpan();
    public Candidates Candidates => _candidates ?? UpdateCandidates();
    public IEnumerable<ISolver> Solvers {get; set;}

    public IEnumerable<ICandidateSolver> CandidateSolvers {get; set;}

    [MemberNotNull(nameof(_candidates))]
    public Candidates? UpdateCandidates()
    {
        BasicCandidatesSolver solver = new();
        solver.TryFindCandidates(this, out Candidates? candidates);
        _candidates = candidates ?? throw new Exception();
        return candidates;
    }

    public void AddSolver(ISolver solver)
    {
        Solvers = new List<ISolver>()
        {
            solver
        };
    }

    public void AddCandidateSolver(ICandidateSolver solver)
    {
        CandidateSolvers = new List<ICandidateSolver>()
        {
            solver
        };
    }

    // Approach chosen is to collect the first solution from the solvers
    // then reset to first (assumed to be cheapest/simplest) solver.
    public bool TrySolve([NotNullWhen(true)] out Solution? solution)
    {
        if (Solvers is null)
        {
            solution = null;
            return false;
        }

        foreach(ISolver solver in Solvers)
        {
            if (solver.TrySolve(this, out solution))
            {
                return true;
            }
        }
    
        solution = null;
        return false;
    }

    public bool TryFindCandidates([NotNullWhen(true)] out Candidates? candidates)
    {
        if (CandidateSolvers is null)
        {
            candidates = null;
            return false;
        }

        foreach(ICandidateSolver solver in CandidateSolvers)
        {
            if (solver.TryFindCandidates(this, out candidates))
            {
                return true;
            }
        }

        candidates = null;
        return false;
    }

    public bool Solve()
    {
        if (TrySolve(out Solution? solution))
        {
            Update(solution);
            return true;
        }

        if (TryFindCandidates(out Candidates? candidates))
        {
            ISolver solver = new SingleCandidateRemainingSolver();
            while (solver.TrySolve(this, out solution))
            {
                Update(solution);
                return true;
            }
        }

        return false;
    }

    public bool SolvePuzzle()
    {
        while(Solve())
        {
        }

        return IsSolved;
    }

    public bool IsSolved => Solved == TotalsCells;

    public bool IsValid()
    {
        for (int i = 0; i < 9; i++)
        {
            if (GetBox(i).CountValidSolved() == -1 ||
                GetRow(i).CountValidSolved() == -1 ||
                GetColumn(i).CountValidSolved() == -1)
            {
                return false;
            }
        }

        return true;
    }

    private void UpdateCounts()
    {
        Solved = 0;
        SolvedForBox = new int[9];
        SolvedForRow = new int[9];
        SolvedForColumn = new int[9];

        for (int i = 0; i < 9; i++)
        {
            // Process box i
            SolvedForBox[i] = GetBox(i).CountValidSolved();
            Solved += SolvedForBox[i];
            // Process row i
            SolvedForRow[i] = GetRow(i).CountValidSolved();
            // Process column i
            SolvedForColumn[i] = GetColumn(i).CountValidSolved();
        }

        InitialSolved = Solved;
    }

    public bool Update(Solution solution)
    {
        int row = solution.Row;
        int column = solution.Column;
        int value = solution.Value;
        var index = row * 9 + column;
        if (_board[index] != 0)
        {
            throw new Exception("Something went wrong! Oops.");    
        }

        _board[index] = value;
        Solved++;
        SolvedForRow[row]++;
        SolvedForColumn[column]++;
        int box = GetBoxIndex(row,column);
        SolvedForBox[box]++;

        if (_candidates is not null)
        {
            _candidates.RemoveCandidates(index);
        }

        return GetBox(box).CountValidSolved() != -1 &&
            GetColumn(column).CountValidSolved() != -1 &&
            GetRow(row).CountValidSolved() != -1;
    }

    public override string ToString()
    {
        var buffer = new StringBuilder();
        foreach(int num in _board)
        {
            char cell = num == 0 ? '.' : (char)('0' + num);
            buffer.Append(cell);
        }
        return buffer.ToString();
    }

    private static int[] ReadPuzzleAsString(string board)
    {
        if (board.Length != 81)
        {
            throw new ArgumentException("Board must be 81 characters long.",nameof(board));
        }

        int[] puzzle = new int[81];
        for(int i = 0; i < 81; i++)
        {
            puzzle[i] = board[i] == '.' ? 0 : (int)board[i] - (int)'0';
        }
        return puzzle;
    }
}
