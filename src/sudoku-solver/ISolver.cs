using System.Collections.Generic;

public interface ISolver
{
    IEnumerable<Solution> FindSolutions();
    bool CheckEffective();
}