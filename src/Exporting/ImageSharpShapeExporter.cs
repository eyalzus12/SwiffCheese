using System.Collections.Generic;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SwfLib.Data;
using SwfLib.Gradients;
using SwiffCheese.Utils;
using SwiffCheese.Wrappers;

namespace SwiffCheese.Exporting;

public class ImageSharpShapeExporter(Image<Rgba32> canvas, Size offset = default, float unitDivisor = 1) : IShapeExporter
{
    private readonly DrawingOptions _drawingOptions = new()
    {
        GraphicsOptions = new GraphicsOptions()
        {
            Antialias = false,
        }
    };

    private readonly PathBuilder _builder = new();
    private Brush? _fill;
    private Pen? _line;

    private Vector2 GetRealLocation(Point point) => ((Vector2)(point + offset)) / unitDivisor;

    public void BeginShape()
    {
        _builder.MoveTo(GetRealLocation(Point.Empty));
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
        if (close) _builder.CloseFigure();
        FinalizePath();
    }

    public void BeginFill(SwfColor color)
    {
        FinalizePath();
        _fill = new SolidBrush(color.SwfColorToImageSharpColor());
    }

    public void BeginLinearGradientFill(LinearGradientFillStyle fillStyle)
    {
        FinalizePath();
        SwfGradient gradient = fillStyle.Gradient;
        SwfGradientRecord[] records = [.. gradient.GradientRecords];
        List<ColorStop> colorStops = [];
        for (int i = 0; i < records.Length; ++i)
        {
            var record = records[i];
            if ((i > 0) && (record.Ratio == records[i - 1].Ratio))
                continue;
            colorStops.Add(new ColorStop(record.Ratio / 255.0f, record.Color.SwfColorToImageSharpColor()));
        }

        GradientRepetitionMode mode = gradient.SpreadMode switch
        {
            SpreadMode.Pad => GradientRepetitionMode.None,
            SpreadMode.Reflect => GradientRepetitionMode.Reflect,
            SpreadMode.Repeat => GradientRepetitionMode.Repeat,
            _ => GradientRepetitionMode.None
        };

        SwfMatrix gradientMat = fillStyle.GradientMatrix;
        _drawingOptions.Transform = gradientMat.SwfMatrixToMatrix3x2();
        PointF start = new(-16384, 0);
        PointF end = new(16384, 0);
        _fill = new LinearGradientBrush(start, end, mode, [.. colorStops]);
    }

    public void BeginRadialGradientFill(RadialGradientFillStyle fillStyle)
    {
        FinalizePath();
        SwfGradient gradient = fillStyle.Gradient;
        SwfGradientRecord[] records = [.. gradient.GradientRecords];
        List<ColorStop> colorStops = [];
        for (int i = 0; i < records.Length; ++i)
        {
            var record = records[i];
            if ((i > 0) && (record.Ratio == records[i - 1].Ratio))
                continue;
            colorStops.Add(new ColorStop(record.Ratio / 255.0f, record.Color.SwfColorToImageSharpColor()));
        }

        GradientRepetitionMode mode = gradient.SpreadMode switch
        {
            SpreadMode.Pad => GradientRepetitionMode.None,
            SpreadMode.Reflect => GradientRepetitionMode.Reflect,
            SpreadMode.Repeat => GradientRepetitionMode.Repeat,
            _ => GradientRepetitionMode.None
        };

        SwfMatrix gradientMat = fillStyle.GradientMatrix;
        _drawingOptions.Transform = gradientMat.SwfMatrixToMatrix3x2();
        PointF center = new(0, 0);
        _fill = new RadialGradientBrush(center, 16384, mode, [.. colorStops]);
    }

    public void EndFill()
    {
        FinalizePath();
    }

    public void LineStyle(float thickness = float.NaN, SwfColor color = default)
    {
        FinalizePath();
        _line = new SolidPen(color.SwfColorToImageSharpColor(), float.IsNaN(thickness) ? 1 : thickness);
    }

    public void MoveTo(Point pos)
    {
        _builder.MoveTo(GetRealLocation(pos));
    }

    public void LineTo(Point pos)
    {
        _builder.LineTo(GetRealLocation(pos));
    }

    public void CurveTo(Point anchor, Point to)
    {
        _builder.QuadraticBezierTo(GetRealLocation(anchor), GetRealLocation(to));
    }

    public void FinalizePath()
    {
        IPath path = _builder.Build();

        if (_fill is not null)
        {
            canvas.Mutate(x => x.Fill(_drawingOptions, _fill, path));
        }
        if (_line is not null)
        {
            canvas.Mutate(x => x.Draw(_drawingOptions, _line, path));
        }

        _builder.Clear(); _builder.MoveTo(GetRealLocation(Point.Empty));
        _fill = null;
        _line = null;
    }
}
