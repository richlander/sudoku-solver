// Expects a single string of Colorku color info
// Outputs Sudoku numbers

if (args.Length != 1 || args[0].Length != 81)
{
    Console.WriteLine("Incorrect input");
}

Console.WriteLine();

string s = args[0];
for(int i = 0; i < s.Length; i++)
{
    char c = s[i];

}

foreach (char c in args[0])
{
    int n = c switch
    {
        'Y' => 1,
        'O' => 2,
        'R' => 3,
        'p' => 4,
        'P' => 5,
        'b' => 6,
        'B' => 7,
        'g' => 8,
        'G' => 9,
        '.' => 0,
        _ => throw new Exception("{c} is an invalid character.")
    };

    Console.Write(n);
}

Console.WriteLine();
