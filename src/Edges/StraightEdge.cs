using System.Numerics;

namespace SwiffCheese.Edges;

public class StraightEdge : IEdge
{
    public Vector2 From { get; init; }
    public Vector2 To { get; init; }
    public int FillStyleIndex { get; init; }
    public int LineStyleIndex { get; init; }
    public bool IsReverseEdge { get; init; } = false;
    public virtual IEdge ReverseWithStyle(int newFillStyleIndex) => new StraightEdge()
    {
        From = To,
        To = From,
        FillStyleIndex = newFillStyleIndex,
        LineStyleIndex = LineStyleIndex,
        IsReverseEdge = !IsReverseEdge,
    };
}
