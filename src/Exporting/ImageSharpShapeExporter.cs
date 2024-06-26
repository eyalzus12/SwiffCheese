using System.Collections.Generic;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SwfLib.Gradients;
using SwiffCheese.Exporting.Brushes;
using SwiffCheese.Utils;
using SwiffCheese.Wrappers;

namespace SwiffCheese.Exporting;

public class ImageSharpShapeExporter(Image<Rgba32> canvas, Vector2 offset = default, float unitDivisor = 1) : IShapeExporter
{
    private static readonly DrawingOptions _DrawingOptions = new()
    {
        GraphicsOptions = new GraphicsOptions()
        {
            Antialias = false,
        },
    };

    private readonly PathBuilder _builder = new();
    private Brush? _fill;
    private Pen? _line;

    private Vector2 GetRealLocation(Point point) => (Vector2)point / unitDivisor + offset;

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

        Matrix3x2 gradientMat = fillStyle.GradientMatrix.SwfMatrixToMatrix3x2();
        // apply draw transform
        gradientMat *= 1 / unitDivisor;
        gradientMat.M31 += offset.X; gradientMat.M32 += offset.Y;

        Matrix3x2.Invert(gradientMat, out Matrix3x2 inverseMat);
        _fill = new TransformedLinearGradientBrush(new PointF(-16384, 0), new PointF(16384, 0), inverseMat, mode, [.. colorStops]);
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

        Matrix3x2 gradientMat = fillStyle.GradientMatrix.SwfMatrixToMatrix3x2();
        // apply draw transform
        gradientMat *= 1 / unitDivisor;
        gradientMat.M31 += offset.X; gradientMat.M32 += offset.Y;

        Matrix3x2.Invert(gradientMat, out Matrix3x2 inverseMat);
        _fill = new TransformedRadialGradientBrush(new PointF(0, 0), 16384, inverseMat, mode, [.. colorStops]);
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

        if (_fill is not null) canvas.Mutate(x => x.Fill(_DrawingOptions, _fill, path));
        if (_line is not null) canvas.Mutate(x => x.Draw(_DrawingOptions, _line, path));

        _builder.Clear(); _builder.MoveTo(GetRealLocation(Point.Empty));
        _fill = null;
        _line = null;
    }
}
