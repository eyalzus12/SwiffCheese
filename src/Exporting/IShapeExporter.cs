using SixLabors.ImageSharp;

namespace SwiffCheese.Exporting;

public interface IShapeExporter
{
    void BeginShape();
    void EndShape();
    void BeginFills();
    void EndFills();
    void BeginLines();
    void EndLines(bool close);
    void BeginFill(Color color);
    void EndFill();
    void LineStyle(float thickness = float.NaN, Color color = default);
    void MoveTo(Point pos);
    void LineTo(Point pos);
    void CurveTo(Point anchor, Point to);
    void FinalizePath();
}