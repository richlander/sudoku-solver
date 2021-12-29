using System;

namespace sudoku_solver;

public ref struct TwoBoxes
{
    public TwoBoxes(Box boxOne, Box boxTwo)
    {
        BoxOne = boxOne;
        BoxTwo = boxTwo;
    }
    Box BoxOne;
    Box BoxTwo;

    public Box this[int i] => (i == 0) ? BoxOne : BoxTwo;

    public Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    public ref struct Enumerator
    {
        private TwoBoxes _twoBoxes;
        private int _index;

        public Enumerator(TwoBoxes twoBoxes)
        {
            _twoBoxes = twoBoxes;
            _index = -1;
        }

        public Box Current => (_index == 0) ? _twoBoxes.BoxOne : _twoBoxes.BoxTwo;

        public bool MoveNext()
        {
            _index++;
            return _index < 2;
        }
    }
}
