using System;
using ROSi = System.ReadOnlySpan<int>;

namespace sudoku_solver;

public ref struct BoxCandidates
{
    public Box Box;
    public ROSi A0;
    public ROSi B1;
    public ROSi C2;
    public ROSi D3;
    public ROSi E4;
    public ROSi F5;
    public ROSi G6;
    public ROSi H7;
    public ROSi I8;

    public BoxCandidates(Box box)
    {
        Box = box;
        A0 = BoxCandidates.GetCandidates(box,0);
        B1 = BoxCandidates.GetCandidates(box,1);
        C2 = BoxCandidates.GetCandidates(box,2);
        D3 = BoxCandidates.GetCandidates(box,3);
        E4 = BoxCandidates.GetCandidates(box,4);
        F5 = BoxCandidates.GetCandidates(box,5);
        G6 = BoxCandidates.GetCandidates(box,6);
        H7 = BoxCandidates.GetCandidates(box,7);
        I8 = BoxCandidates.GetCandidates(box,8);
    }

    public ROSi this[int i] => i switch
    {
        0 => A0,
        1 => B1,
        2 => C2,
        3 => D3,
        4 => E4,
        5 => F5,
        6 => G6,
        7 => H7,
        8 => I8,
        _ => throw new ArgumentException()
    };

    public (bool found, int[] multiples) GetMultiples()
    {
        var pairs = new int[18];
        var triples = new int[27];
        var pairIndex = 0;
        var triplesIndex = 0;

        for (int i = 0; i < 9;i++)
        {
            var candidates = this[i];

            if (candidates.Length == 2)
            {
                    pairs[pairIndex++] = candidates[0];
                    pairs[pairIndex++] = candidates[1];
            }
            else if (candidates.Length == 3)
            {
                    triples[triplesIndex++] = candidates[0];
                    triples[triplesIndex++] = candidates[1];
                    triples[triplesIndex++] = candidates[2];
            }
        }

        var index = 2;
        while (index <= pairIndex)
        {
            var innerIndex = index;
            while (innerIndex <= pairIndex)
            {
                if (pairs[index] == pairs[index-2] && 
                    pairs[index+1] == pairs[index-1])   
                {
                    return (true,new int[]{pairs[index], pairs[index+1]});
                }
                innerIndex +=2;
            }
            index+=2;
        }

        return (false,Array.Empty<int>());
    }

    public static ReadOnlySpan<int> GetCandidates(Box box, int index)
    {
        if (box[index] != 0)
        {
            return Array.Empty<int>();
        }

        // get complete row that includes current row of box
        var currentRowIndex = box.GetRowOffsetForCell(index);
        var currentRow = box.Puzzle.GetRow(currentRowIndex);

        // get complete column that includes the the first column of box
        var currentColIndex = box.GetColumnOffsetForCell(index);
        var currentCol = box.Puzzle.GetColumn(currentColIndex);

        bool[] values = new bool[10];
        Span<int> missingValues = new int[9];
        var currentBox = box.AsLine();

        for (int i = 0; i < 9; i++)
        {
            values[currentRow[i]] = true;
            values[currentCol[i]] = true;
            values[currentBox[i]] = true;
        }

        int count = -1;
        for (int i = 1; i < 10; i++)
        {
            if (!values[i])
            {
                count++;
                missingValues[count] = i;
            }
        }

        return missingValues.Slice(0,count+1);
    }
}
