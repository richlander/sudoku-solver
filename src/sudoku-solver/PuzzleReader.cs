using System;
using System.IO;
using System.Text.Json;
using static System.Console;

public class PuzzleReader
{
    public static Puzzle ReadPuzzle(FileInfo file)
    {
        using var stream = file.OpenRead();
        var doc = JsonDocument.Parse(stream);
        var cellIndex = 0;

        int[] puzzleArray = new int[81];

        foreach(var row in doc.RootElement.EnumerateArray())
        {
            foreach (var element in row.EnumerateArray())
            {
                var value = element.GetInt32();
                puzzleArray[cellIndex] = value;
                cellIndex++;
            }
        }

        var puzzle = new Puzzle(puzzleArray.AsMemory());
        return puzzle;
    }
}