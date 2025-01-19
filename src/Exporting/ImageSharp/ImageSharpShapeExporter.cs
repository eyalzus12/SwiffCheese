using System.Collections.Generic;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SwfLib.Gradients;
using SwiffCheese.Exporting.ImageSharp.Brushes;
using SwiffCheese.Math;
using SwiffCheese.Wrappers;

namespace SwiffCheese.Exporting.ImageSharp;

public class ImageSharpShapeExporter(Image<Rgba32> canvas, Vector2 offset = default, float unitDivisor = 1) : IShapeExporter
{
    public static Color SwfColorToImageSharpColor(SwfColor color) =>
        Color.FromRgba(color.Red, color.Green, color.Blue, color.Alpha);

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

    private Vector2 GetRealLocation(Vector2I point) => new Vector2(point.X / unitDivisor, point.Y / unitDivisor) + offset;

    public void BeginShape()
    {
        _builder.MoveTo(GetRealLocation(Vector2I.Zero));
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
        _fill = new SolidBrush(SwfColorToImageSharpColor(color));
    }

    public void BeginLinearGradientFill(LinearGradientFillStyle fillStyle)
    {
        FinalizePath();
        SwfGradient gradient = fillStyle.Gradient;
        SwfGradientRecord[] records = [.. gradient.GradientRecords];
        List<ColorStop> colorStops = [];
        for (int i = 0; i < records.Length; ++i)
        {
            SwfGradientRecord record = records[i];
            if ((i > 0) && (record.Ratio == records[i - 1].Ratio))
                continue;
            colorStops.Add(new ColorStop(record.Ratio / 255.0f, SwfColorToImageSharpColor(record.Color)));
        }

        GradientRepetitionMode mode = gradient.SpreadMode switch
        {
            SpreadMode.Pad => GradientRepetitionMode.None,
            SpreadMode.Reflect => GradientRepetitionMode.Reflect,
            SpreadMode.Repeat => GradientRepetitionMode.Repeat,
            _ => GradientRepetitionMode.None
        };

        Matrix3x2 gradientMat = ImageSharpUtils.SwfMatrixToMatrix3x2(fillStyle.GradientMatrix);
        // apply draw transform
        gradientMat *= 1 / unitDivisor;
        gradientMat.M31 += offset.X; gradientMat.M32 += offset.Y;

        _fill = new TransformedLinearGradientBrush(new PointF(-16384, 0), new PointF(16384, 0), gradientMat, mode, [.. colorStops]);
    }

    public void BeginRadialGradientFill(RadialGradientFillStyle fillStyle)
    {
        FinalizePath();
        SwfGradient gradient = fillStyle.Gradient;
        SwfGradientRecord[] records = [.. gradient.GradientRecords];
        List<ColorStop> colorStops = [];
        for (int i = 0; i < records.Length; ++i)
        {
            SwfGradientRecord record = records[i];
            if ((i > 0) && (record.Ratio == records[i - 1].Ratio))
                continue;
            colorStops.Add(new ColorStop(record.Ratio / 255.0f, SwfColorToImageSharpColor(record.Color)));
        }

        GradientRepetitionMode mode = gradient.SpreadMode switch
        {
            SpreadMode.Pad => GradientRepetitionMode.None,
            SpreadMode.Reflect => GradientRepetitionMode.Reflect,
            SpreadMode.Repeat => GradientRepetitionMode.Repeat,
            _ => GradientRepetitionMode.None
        };

        Matrix3x2 gradientMat = ImageSharpUtils.SwfMatrixToMatrix3x2(fillStyle.GradientMatrix);
        // apply draw transform
        gradientMat *= 1 / unitDivisor;
        gradientMat.M31 += offset.X; gradientMat.M32 += offset.Y;

        _fill = new TransformedRadialGradientBrush(new PointF(0, 0), new PointF(0, 0), 16384, gradientMat, mode, [.. colorStops]);
    }

    public void BeginFocalGradientFill(FocalGradientFillStyle fillStyle)
    {
        FinalizePath();
        SwfFocalGradient gradient = fillStyle.Gradient;
        SwfGradientRecord[] records = [.. gradient.GradientRecords];
        List<ColorStop> colorStops = [];
        for (int i = 0; i < records.Length; ++i)
        {
            SwfGradientRecord record = records[i];
            if ((i > 0) && (record.Ratio == records[i - 1].Ratio))
                continue;
            colorStops.Add(new ColorStop(record.Ratio / 255.0f, SwfColorToImageSharpColor(record.Color)));
        }

        GradientRepetitionMode mode = gradient.SpreadMode switch
        {
            SpreadMode.Pad => GradientRepetitionMode.None,
            SpreadMode.Reflect => GradientRepetitionMode.Reflect,
            SpreadMode.Repeat => GradientRepetitionMode.Repeat,
            _ => GradientRepetitionMode.None
        };

        Matrix3x2 gradientMat = ImageSharpUtils.SwfMatrixToMatrix3x2(fillStyle.GradientMatrix);
        // apply draw transform
        gradientMat *= 1 / unitDivisor;
        gradientMat.M31 += offset.X; gradientMat.M32 += offset.Y;

        _fill = new TransformedRadialGradientBrush(new PointF(0, 0), new PointF((float)(16384 * gradient.FocalPoint), 0), 16384, gradientMat, mode, [.. colorStops]);
    }

    public void EndFill()
    {
        FinalizePath();
    }

    public void LineStyle(float thickness = float.NaN, SwfColor color = default)
    {
        FinalizePath();
        _line = new SolidPen(SwfColorToImageSharpColor(color), float.IsNaN(thickness) ? 1 : thickness);
    }

    public void MoveTo(Vector2I pos)
    {
        _builder.MoveTo(GetRealLocation(pos));
    }

    public void LineTo(Vector2I pos)
    {
        _builder.LineTo(GetRealLocation(pos));
    }

    public void CurveTo(Vector2I anchor, Vector2I to)
    {
        _builder.QuadraticBezierTo(GetRealLocation(anchor), GetRealLocation(to));
    }

    public void FinalizePath()
    {
        IPath path = _builder.Build();

        if (_fill is not null) canvas.Mutate(x => x.Fill(_DrawingOptions, _fill, path));
        if (_line is not null) canvas.Mutate(x => x.Draw(_DrawingOptions, _line, path));

        _builder.Clear(); _builder.MoveTo(GetRealLocation(Vector2I.Zero));
        _fill = null;
        _line = null;
    }
}
