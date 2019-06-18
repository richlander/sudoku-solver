using System;
using ROSi = System.ReadOnlySpan<int>;

namespace sudoku_solver
{
    public ref struct BoxCandidates
    {
        ROSi A0;
        ROSi B1;
        ROSi C2;
        ROSi D3;
        ROSi E4;
        ROSi F5;
        ROSi G6;
        ROSi H7;
        ROSi I8;

        public BoxCandidates(ROSi a0, ROSi b1, ROSi c2, ROSi d3, ROSi e4, ROSi f5, ROSi g6, ROSi h7, ROSi i8)
        {
            A0 = a0;
            B1 = b1;
            C2 = c2;
            D3 = d3;
            E4 = e4;
            F5 = f5;
            G6 = g6;
            H7 = h7;
            I8 = i8;
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

        public int GetPairs()
        {
            for (int i = 0; i < 9;i++)
            {
                var candidates = this[i];

                if (candidates.Length == 1)
                {
                    return i;
                }
                else if (candidates.Length == 2)
                {
                       
                }

            }
        }

    }
}