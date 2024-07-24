using SixLabors.ImageSharp;
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
    void BeginLinearGradientFill(LinearGradientFillStyle fillStyle);
    void BeginRadialGradientFill(RadialGradientFillStyle fillStyle);
    void BeginFocalGradientFill(FocalGradientFillStyle fillStyle);
    void EndFill();
    void LineStyle(float thickness = float.NaN, SwfColor color = default);
    void MoveTo(Point pos);
    void LineTo(Point pos);
    void CurveTo(Point anchor, Point to);
    void FinalizePath();
}