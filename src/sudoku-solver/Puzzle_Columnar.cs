namespace sudoku_solver;

public partial class Puzzle
{
    public Puzzle AsColumnar()
    {
        int[] board = new int[81];
        
        // enumerate 9 columns
        for(int i = 0; i < 9; i++)
        {
            // enumerate 9 cells
            for (int j = 0; j < 9 ; j++)
            {
                Line column = GetColumn(i);
                int position = (i * 9) + j;
                board[position] = column[j];
            }
        }

        return new Puzzle(board);
    }
}