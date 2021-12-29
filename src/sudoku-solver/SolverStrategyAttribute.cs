using System;
using System.Reflection;

namespace sudoku_solver;

// SolverStrategyAttribute is modelled after CompilerTraitAttribute
// https://github.com/dotnet/roslyn/blob/master/src/Test/Utilities/Portable/CompilerTraitAttribute.cs
public class SolverStrategyAttribute : Attribute
{
    public SolverStrategyAttribute(Strategy strategy)
    {
    }
}
