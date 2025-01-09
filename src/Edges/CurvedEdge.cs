using System.Numerics;

namespace SwiffCheese.Edges;

public class CurvedEdge : StraightEdge, IEdge
{
    public Vector2 Control { get; init; }
    public override IEdge ReverseWithStyle(int newFillStyleIndex) => new CurvedEdge()
    {
        From = To,
        To = From,
        Control = Control,
        FillStyleIndex = newFillStyleIndex,
        LineStyleIndex = LineStyleIndex,
        IsReverseEdge = !IsReverseEdge,
    };
}
