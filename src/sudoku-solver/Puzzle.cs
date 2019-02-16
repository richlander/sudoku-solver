using System;
using System.Collections.Generic;

public class Puzzle
{
    private Memory<int> _puzzle;
    public ReadOnlyMemory<int> Values = new int[] {1,2,3,4,5,6,7,8,9};
    public int UnsolvedMarker = 0;
    public int TotalsCells = 81;
    private Puzzle(Memory<int> puzzle)
    {
        _puzzle = puzzle;
    }

    private int Solved {get; set;}
    public int[] SolvedForBox {get; private set;}
    public int[] SolvedForRow { get; private set; }
    public int[] SolvedForColumn { get; private set; }

    public static Puzzle ReadPuzzle(Memory<int> puzzle)
    {
        var p = new Puzzle(puzzle);
        p.Validate();
        return p;
    }

    public bool IsSolved()
    {
        if (Solved == TotalsCells && Validate().Solved)
        {
            return true;
        }
        else if (Solved == TotalsCells)
        {
            throw new Exception("Something went wrong");
        }
        return false;
    }

    private PuzzleState Validate()
    {
        if (_puzzle.Length != TotalsCells)
        {
            throw new Exception("Puzzle incorrect length");
        }

        Solved = 0;
        SolvedForBox = new int[9];
        SolvedForRow = new int[9];
        SolvedForColumn = new int[9];

        for (int i = 0; i < 9; i++)
        {
            // Process box i
            var box = GetBox(i);
            var boxSequence = new int[]
            {
                box.FirstRow[0],
                box.FirstRow[1],
                box.FirstRow[2],
                box.InsideRow[0],
                box.InsideRow[1],
                box.InsideRow[2],
                box.LastRow[0],
                box.LastRow[1],
                box.LastRow[2]
            };
            var (boxCount, boxState) = ProcessSequence(boxSequence);
            if (!boxState.Valid) return boxState;
            SolvedForBox[i] = boxCount;

            // Process row i
            var row = GetRow(i);
            var (rowCount, rowState) = ProcessSequence(row.Segment);
            if (!rowState.Valid) return rowState;
            SolvedForRow[i] = rowCount;

            // Process column i
            var column = GetColumn(i);
            var (columnCount, columnState) = ProcessSequence(column.Segment);
            if (!columnState.Valid) return columnState;
            SolvedForColumn[i] = columnCount;

            // Update total solved count
            Solved += boxCount + rowCount + columnCount;
        }

        return new PuzzleState
        {
            Solved = Solved == TotalsCells,
            Valid = true
        };

        (int solvedCount, PuzzleState puzzleState) ProcessSequence(Span<int> sequence)
        {
            int count = 0;
            var values = new bool[10];
            for (int i = 0; i < sequence.Length; i++)
            {
                var value = sequence[i];
                if (value >= 1 && value <= 9)
                {
                    if (values[value])
                    {
                        return (0, new PuzzleState
                        {
                            Valid = false,
                            Description = "Character already seen"
                        });
                    }
                    values[value] = true;
                    count++;
                }
                else if (value == UnsolvedMarker)
                {}
                else
                {
                    throw new Exception($"Unknown character: {value}");
                }
            }
            return (count, new PuzzleState
                    {
                        Valid = true
                    });
        }
    }

    public Line GetRow(int index)
    {
        var start = GetOffsetForRow(index);

        return new Line
        {
            Segment = _puzzle.Slice(start, 9).Span 
        };
    }

    public Line GetColumn(int index)
    {
        var column = new int[9];
        var grid = _puzzle.Span;
        for (int i = 0;  i < 9; i++)
        {
            var cIndex = index + (i * 9);
            column[i] = grid[cIndex];
        }
        
        return new Line
        {
            Segment = column
        };
    }

    public Box GetBox(int index)
    {
        int start = GetOffsetForBox(index);

        var box = new Box
        {
            FirstRow = _puzzle.Slice(start, 3).Span,
            InsideRow = _puzzle.Slice(start + 9, 3).Span,
            LastRow = _puzzle.Slice(start + 18, 3).Span
        };

        return box;
    }

    public void Update(Solution solution)
    {
        if (!solution.Solved)
        {
            return;
        }

        if (solution.Type == SequenceType.Unknown)
        {
            throw new Exception("Something went wrong.");
        }
        else if (solution.Type == SequenceType.Box)
        {
            int start = GetOffsetForBox(solution.Index);
            int element = start + (solution.Cell / 3) * 9 + solution.Cell % 3;
            UpdateCell(element, solution.Value);
            SolvedForBox[solution.Index]++;       
        }
        else if (solution.Type == SequenceType.Row)
        {
            int start = GetOffsetForRow(solution.Index);
            int element = start + solution.Cell;
            UpdateCell(element, solution.Value);
            SolvedForColumn[solution.Index]++;
        }
        else if (solution.Type == SequenceType.Column)
        {
            var element = GetOffsetsForColumn(solution.Index)[solution.Cell];
            UpdateCell(element, solution.Value);
        }
    }

    private void UpdateCell(int index, int value)
    {
        var puzzle = _puzzle.Span;
        if (puzzle[index] != 0)
        {
            throw new Exception("Something went wrong! Oops.");    
        }

        puzzle[index] = value;
        Solved++;       
    }

    private int GetOffsetForBox(int index)
    {
        return (index / 3) * 27 + (index % 3) * 3;
    }

    private int[] GetOffsetsForColumn(int index)
    {
        var column = new int[9];
        for (int i = 0; i < 9; i++)
        {
            var cIndex = index + (i * 9);
            column[i] = cIndex;
        }
        return column;
    }

    private int GetOffsetForRow(int index)
    {
        return index * 9;
    }
}