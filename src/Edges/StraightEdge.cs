using SixLabors.ImageSharp;

namespace SwiffCheese.Edges;

public class StraightEdge : IEdge
{
    public Point From { get; init; }
    public Point To { get; init; }
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
