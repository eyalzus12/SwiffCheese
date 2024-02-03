using SixLabors.ImageSharp;

namespace SwiffCheese.Edges;

public interface IEdge
{
    public Point From { get; init; }
    public Point To { get; init; }
    public int FillStyleIndex { get; init; }
    public int LineStyleIndex { get; init; }
    public bool IsReverseEdge { get; init; }
    public IEdge ReverseWithStyle(int newFillStyleIndex);
}
