using SwiffCheese.Math;

namespace SwiffCheese.Edges;

public interface IEdge
{
    public Vector2I From { get; init; }
    public Vector2I To { get; init; }
    public int FillStyleIndex { get; init; }
    public int LineStyleIndex { get; init; }
    public bool IsReverseEdge { get; init; }
    public IEdge ReverseWithStyle(int newFillStyleIndex);
}
