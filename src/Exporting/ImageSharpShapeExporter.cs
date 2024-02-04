using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SwiffCheese.Exporting;

public class ImageSharpShapeExporter : IShapeExporter
{
    private readonly PathBuilder _builder = new();
    private readonly Image<Rgba32> _canvas;
    private readonly Size _offset;
    private Brush? _fill;
    private Pen? _line;

    public ImageSharpShapeExporter(Image<Rgba32> canvas, Size offset = default)
    {
        _canvas = canvas;
        _offset = offset;
    }

    public void BeginShape()
    {
        _builder.MoveTo((Point)_offset);
    }

    public void EndShape()
    {

    }

    public void BeginFills()
    {

    }

    public void EndFills()
    {

    }

    public void BeginLines()
    {

    }

    public void EndLines(bool close)
    {
        if(close) _builder.CloseFigure();
        FinalizePath();
    }

    public void BeginFill(Color color)
    {
        FinalizePath();
        _fill = new SolidBrush(color);
    }

    public void EndFill()
    {
        FinalizePath();
    }

    public void LineStyle(float thickness = float.NaN, Color color = default)
    {
        FinalizePath();
        _line = new SolidPen(color, float.IsNaN(thickness) ? 1 : thickness);
    }

    public void MoveTo(Point pos)
    {
        _builder.MoveTo(Point.Add(pos, _offset));
    }

    public void LineTo(Point pos)
    {
        _builder.LineTo(Point.Add(pos, _offset));
    }

    public void CurveTo(Point anchor, Point to)
    {
        _builder.QuadraticBezierTo(Point.Add(anchor, _offset), Point.Add(to, _offset));
    }

    public void FinalizePath()
    {
        IPath path = _builder.Build();

        if (_fill is not null)
        {
            _canvas.Mutate(x => x.Fill(_fill, path));
        }
        if (_line is not null)
        {
            _canvas.Mutate(x => x.Draw(_line, path));
        }

        _builder.Clear(); _builder.MoveTo((Point)_offset);
        _fill = null;
        _line = null;
    }
}
