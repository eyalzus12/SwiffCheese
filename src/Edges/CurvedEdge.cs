using Microsoft.Xna.Framework;

namespace SwiffCheese;

public class CurvedEdge : StraightEdge, IEdge
{
    public Point Control { get; init; }
    public override IEdge ReverseWithStyle(int newFillStyleIndex) => new CurvedEdge()
    {
        From = To,
        To = From,
        Control = Control,
        FillStyleIndex = FillStyleIndex,
        LineStyleIndex = LineStyleIndex,
        IsReverseEdge = !IsReverseEdge,
    };
}
