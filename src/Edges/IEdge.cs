using System.Numerics;

namespace SwiffCheese.Edges;

public interface IEdge
{
    public Vector2 From { get; init; }
    public Vector2 To { get; init; }
    public int FillStyleIndex { get; init; }
    public int LineStyleIndex { get; init; }
    public bool IsReverseEdge { get; init; }
    public IEdge ReverseWithStyle(int newFillStyleIndex);
}
