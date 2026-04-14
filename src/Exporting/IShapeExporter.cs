using SwfLib.Data;
using SwiffCheese.Math;
using SwiffCheese.Wrappers;

namespace SwiffCheese.Exporting;

public interface IShapeExporter
{
    void BeginShape();
    void EndShape();
    void BeginFills();
    void EndFills();
    void BeginLines();
    void EndLines(bool close);
    void BeginFill(SwfColor color);
    void BeginLinearGradientFill(SwfMatrix gradientMatrix, SwfGradient gradient);
    void BeginRadialGradientFill(SwfMatrix gradientMatrix, SwfGradient gradient);
    void BeginFocalGradientFill(SwfMatrix gradientMatrix, SwfFocalGradient gradient);
    void EndFill();
    void LineStyle(float thickness = float.NaN, SwfColor color = default);
    void MoveTo(Vector2I pos);
    void LineTo(Vector2I pos);
    void CurveTo(Vector2I anchor, Vector2I to);
    void FinalizePath();
}