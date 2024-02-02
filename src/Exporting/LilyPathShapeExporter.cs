using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LilyPath;
using System.Linq;

namespace SwiffCheese;

public class LilyPathShapeExporter : IShapeExporter
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly DrawBatch _batch;
    private readonly PathBuilder _path = new();
    private Brush? _fill;
    private Pen? _line;
    private Point _currentPoint = Point.Zero;

    public LilyPathShapeExporter(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _batch = new(_graphicsDevice);
    }

    public void BeginShape()
    {
        _batch.Begin();
        _currentPoint = Point.Zero;
    }

    public void EndShape()
    {
        _batch.End();
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
        //if(close) _path.CloseFigure();
        FinalizePath();
    }

    public void BeginFill(Color color)
    {
        FinalizePath();
        _fill = new SolidColorBrush(color);
    }

    public void EndFill()
    {
        FinalizePath();
    }

    public void LineStyle(float thickness = float.NaN, Color color = default)
    {
        FinalizePath();
        _line = new Pen(color, float.IsNaN(thickness) ? 1 : thickness);
    }

    public void MoveTo(Point pos)
    {
        _currentPoint = pos;
    }

    public void LineTo(Point pos)
    {
        _path.AddPath(new[] { _currentPoint.ToVector2(), pos.ToVector2() });
        _currentPoint = pos;
    }

    public void CurveTo(Point anchor, Point to)
    {
        _path.AddBezier(_currentPoint.ToVector2(), anchor.ToVector2(), to.ToVector2());
        _currentPoint = to;
    }

    public void FinalizePath()
    {
        if (_fill is not null)
        {
            System.Console.WriteLine(_path.Buffer.Length);
            _batch.FillPath(_fill, _path.Buffer);
        }
        if (_line is not null)
        {
            _batch.DrawPath(_path.Stroke(_line, PathType.Closed));
        }

        _path.Reset();
        _currentPoint = Point.Zero;
        _fill = null;
        _line = null;
    }
}
