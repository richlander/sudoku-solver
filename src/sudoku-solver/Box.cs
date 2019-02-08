using System;
public ref struct Box
{
    public Span<int> FirstRow;
    public Span<int> InsideRow;
    public Span<int> LastRow;
}