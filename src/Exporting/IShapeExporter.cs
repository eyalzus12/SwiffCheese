using SwfLib.Data;
using SwfLib.Shapes.FillStyles;
using SwfLib.Shapes.LineStyles;
using SwiffCheese.Math;
using SwiffCheese.Wrappers;

namespace SwiffCheese.Exporting;

public interface IShapeExporter
{
    void BeginShape(WindingRule windingRule);
    void EndShape();
    void BeginFills();
    void EndFills();
    void BeginLines();
    void EndLines(bool close);
    void BeginSolidFill(SwfColor color);
    void BeginLinearGradientFill(SwfMatrix gradientMatrix, SwfGradient gradient);
    void BeginRadialGradientFill(SwfMatrix gradientMatrix, SwfGradient gradient);
    void BeginFocalGradientFill(SwfMatrix gradientMatrix, SwfFocalGradient gradient);
    void BeginBitmapFill(ushort bitmapId, SwfMatrix bitmapMatrix, bool smoothing, BitmapMode mode);
    void EndFill();
    void LineStyle(float thickness, SwfColor color, bool pixelHinting, StrokeScaleMode scaleMode, CapStyle startCaps, CapStyle endCaps, JoinStyle joints, double miterLimit, bool noClose);
    void MoveTo(Vector2I pos);
    void LineTo(Vector2I pos);
    void CurveTo(Vector2I anchor, Vector2I to);
    void FinalizePath();
}