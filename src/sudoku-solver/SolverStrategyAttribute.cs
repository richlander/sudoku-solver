using System;
using System.Reflection;

namespace sudoku_solver
{
    public class SolverStrategyAttribute : Attribute
    {
        public SolverStrategyAttribute(Strategy strategy)
        {

        }
    }
}