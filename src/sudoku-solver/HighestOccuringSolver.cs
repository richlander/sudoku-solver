using System;
using System.Collections.Generic;
using System.Linq;

namespace sudoku_solver
{
    public class HighestOccuringSolver : ISolver
    {
        private Puzzle _puzzle;

        public HighestOccuringSolver(Puzzle puzzle)
        {
            _puzzle = puzzle;
        }

        public IEnumerable<Solution> FindSolution()
        {
            throw new NotImplementedException();
        }

        public bool IsEffective()
        {
            return true;
        }

        public Solution Solve(int index)
        {
            return new Solution();
        }

        private Solution SolveBox(int index)
        {
            var box = _puzzle.GetBox(index);

            // adjacent vertical neightboring box (avnb)
            // first two values in array are vertical neighbors
            // second two values in array are horizontal neighbors
            var avn = GetAdjectNeighboringBoxIndices(index);
            var highestValue = GetHighestOccuringValueNotInBox(box, avn);
            
            return new Solution();
        }

        private int GetHighestOccuringValueNotInBox(Box box, int[] avn)
        {
            var line = box.AsLine();
            var avnb1 = _puzzle.GetBox(avn[0]).AsLine();
            var avnb2 = _puzzle.GetBox(avn[1]).AsLine();
            var ahnb1 = _puzzle.GetBox(avn[2]).AsLine();
            var ahnb2 = _puzzle.GetBox(avn[3]).AsLine();
            var values = new int[10];

            for (int i = 1; i < 10; i++)
            {
                values[avnb1[i]]++;
                values[avnb2[i]]++;
                values[ahnb1[i]]++;
                values[ahnb2[i]]++;
            }

            var highestValue = 0;
            for (int i = 1; i < 10; i++)
            {
                if (values[i] > values[highestValue] && !line.ContainsValue(values[i]))
                {
                    highestValue = i;
                }
            }

            return highestValue;
        }

        private int[] GetAdjectNeighboringBoxIndices(int index)
        {
            int[] avn = null;
            // avnb1, avnb2, ahnb1, ahnb2

            if (index == 0)
            {
                avn = new int[] {3,6,1,2};
            } 
            else if (index == 1)
            {
                avn = new int[] {4,7,0,2};
            }
            else if (index == 2)
            {
                avn = new int[] {5,8,0,1};
            }
            else if (index == 3)
            {
                avn = new int[] {0,6,4,5};
            }
            else if (index == 4)
            {
                avn = new int[] {1,7,3,5};
            }
            else if (index == 5)
            {
                avn = new int[] {2,8,3,4};
            }
            else if (index == 6)
            {
                avn = new int[] {0,3,7,8};
            }
            else if (index == 7)
            {
                avn = new int[] {1,4,6,8};
            }           
            else if (index == 8)
            {
                avn = new int[] {2,5,6,7};
            }
            return avn;
        }
    }
}