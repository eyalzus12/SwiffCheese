using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Xml.Linq;
using SwfLib.Data;
using SwfLib.Gradients;
using SwiffCheese.Wrappers;

namespace SwiffCheese.Exporting.Svg;

public class SvgShapeExporter(Vector2 position, Vector2 size, double unitDivisor = 20) : IShapeExporter
{
    private readonly XNamespace xmlns = XNamespace.Get("http://www.w3.org/2000/svg");

    public XDocument Document { get; private set; } = null!;

    private string _currentDrawCommand = "";
    private readonly StringBuilder _pathData = new();
    private XElement _svg = null!;
    private XElement _group = null!;
    private XElement _path = null!;
    private XElement? _defs;
    private int _gradientCount;

    private XElement Defs
    {
        get
        {
            if (_defs is null)
            {
                _defs = new(xmlns + "defs");
                _svg.Add(_defs);
            }
            return _defs;
        }
    }

    public void BeginShape()
    {
        double scale = 1.0 / unitDivisor;
        double offsetX = position.X / unitDivisor;
        double offsetY = position.Y / unitDivisor;
        string transform = SvgUtils.SvgMatrixString(scale, 0, 0, scale, -offsetX, -offsetY);

        _group = new XElement(xmlns + "g");
        _group.SetAttributeValue("transform", transform);

        double width = size.X / unitDivisor;
        double height = size.Y / unitDivisor;
        _svg = new(xmlns + "svg", _group);
        _svg.SetAttributeValue("width", width);
        _svg.SetAttributeValue("height", height);

        Document = new(new XDeclaration("1.0", "UTF-8", "no"), _svg);

        _path = new(xmlns + "path");
        _pathData.Clear();
        _gradientCount = 0;
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
        if (close)
            _pathData.Append('Z');
        FinalizePath();
    }

    public void BeginFill(SwfColor color)
    {
        FinalizePath();
        _path.SetAttributeValue("stroke", "none");
        _path.SetAttributeValue("fill", SvgUtils.ColorToHexString(color));
        _path.SetAttributeValue("fill-rule", "evenodd");
        if (color.Alpha != 255)
            _path.SetAttributeValue("fill-opacity", (color.Alpha / 255.0f).ToString());
    }

    public void BeginLinearGradientFill(LinearGradientFillStyle fillStyle)
    {
        FinalizePath();
        XElement gradientElement = new(xmlns + "linearGradient");
        SwfGradient gradient = fillStyle.Gradient;
        PopulateGradientElement(gradientElement, gradient.GradientRecords, fillStyle.GradientMatrix, gradient.SpreadMode, gradient.InterpolationMode);
        AddGradientElement(gradientElement);
    }

    public void BeginRadialGradientFill(RadialGradientFillStyle fillStyle)
    {
        FinalizePath();
        XElement gradientElement = new(xmlns + "radialGradient");
        SwfGradient gradient = fillStyle.Gradient;
        PopulateGradientElement(gradientElement, gradient.GradientRecords, fillStyle.GradientMatrix, gradient.SpreadMode, gradient.InterpolationMode, 0);
        AddGradientElement(gradientElement);
    }

    public void BeginFocalGradientFill(FocalGradientFillStyle fillStyle)
    {
        FinalizePath();
        XElement gradientElement = new(xmlns + "radialGradient");
        SwfFocalGradient gradient = fillStyle.Gradient;
        PopulateGradientElement(gradientElement, gradient.GradientRecords, fillStyle.GradientMatrix, gradient.SpreadMode, gradient.InterpolationMode, gradient.FocalPoint);
        AddGradientElement(gradientElement);
    }

    public void EndFill()
    {
        FinalizePath();
    }

    public void LineStyle(float thickness = float.NaN, SwfColor color = default)
    {
        FinalizePath();
        _path.SetAttributeValue("fill", "none");
        _path.SetAttributeValue("stroke", SvgUtils.ColorToHexString(color));
        _path.SetAttributeValue("stroke-width", thickness);
        if (color.Alpha != 255)
            _path.SetAttributeValue("stop-opacity", (color.Alpha / 255.0).ToString());
    }

    public void MoveTo(Vector2 pos)
    {
        _currentDrawCommand = "";
        _pathData.Append($"M{pos.X} {pos.Y} ");
    }

    public void LineTo(Vector2 pos)
    {
        if (_currentDrawCommand != "L")
        {
            _currentDrawCommand = "L";
            _pathData.Append('L');
        }
        _pathData.Append($"{pos.X} {pos.Y} ");
    }

    public void CurveTo(Vector2 anchor, Vector2 to)
    {
        if (_currentDrawCommand != "Q")
        {
            _currentDrawCommand = "Q";
            _pathData.Append('Q');
        }
        _pathData.Append($"{anchor.X} {anchor.Y} {to.X} {to.Y} ");
    }

    public void FinalizePath()
    {
        string pathData = _pathData.ToString().Trim();
        if (pathData.Length > 0)
        {
            _path.SetAttributeValue("d", pathData);
            _group.Add(_path);
        }
        _path = new XElement(xmlns + "path");
        _pathData.Clear();
        _currentDrawCommand = "";
    }

    private void PopulateGradientElement(XElement gradient, IEnumerable<SwfGradientRecord> records, SwfMatrix matrix, SpreadMode spreadMode, InterpolationMode interpolationMode, double? focalPointRatio = null)
    {
        double scaleX = SvgUtils.RoundPixels400(matrix.ScaleX * unitDivisor);
        double rotateSkew0 = SvgUtils.RoundPixels400(matrix.RotateSkew0 * unitDivisor);
        double rotateSkew1 = SvgUtils.RoundPixels400(matrix.RotateSkew1 * unitDivisor);
        double scaleY = SvgUtils.RoundPixels400(matrix.ScaleY * unitDivisor);
        double translateX = SvgUtils.RoundPixels400(matrix.TranslateX);
        double translateY = SvgUtils.RoundPixels400(matrix.TranslateY);
        string transform = SvgUtils.SvgMatrixString(scaleX, rotateSkew0, rotateSkew1, scaleY, translateX, translateY);
        gradient.SetAttributeValue("gradientTransform", transform);

        gradient.SetAttributeValue("gradientUnits", "userSpaceOnUse");
        // linear
        if (focalPointRatio is null)
        {
            gradient.SetAttributeValue("x1", "-819.2");
            gradient.SetAttributeValue("x2", "819.2");
        }
        // radial or focal
        else
        {
            gradient.SetAttributeValue("r", "819.2");
            gradient.SetAttributeValue("cx", "0");
            gradient.SetAttributeValue("cy", "0");
            if (focalPointRatio != 0)
            {
                gradient.SetAttributeValue("fx", 819.2 * focalPointRatio);
                gradient.SetAttributeValue("fy", "0");
            }
        }

        gradient.SetAttributeValue("spreadMethod", spreadMode switch
        {
            SpreadMode.Pad => "pad",
            SpreadMode.Reflect => "reflect",
            SpreadMode.Repeat => "repeat",
            _ => null,
        });

        if (interpolationMode == InterpolationMode.Linear)
            gradient.SetAttributeValue("color-interpolation", "linearRGB");

        foreach (SwfGradientRecord record in records)
        {
            XElement entry = new(xmlns + "stop");
            entry.SetAttributeValue("offset", record.Ratio / 255.0);
            entry.SetAttributeValue("stop-color", SvgUtils.ColorToHexString(record.Color));
            if (record.Color.Alpha != 255)
                entry.SetAttributeValue("stop-opacity", (record.Color.Alpha / 255.0).ToString());
            gradient.Add(entry);
        }
    }

    private void AddGradientElement(XElement gradientElement)
    {
        string gradientId = $"gradient{_gradientCount++}";
        gradientElement.SetAttributeValue("id", gradientId);
        _path.SetAttributeValue("stroke", "none");
        _path.SetAttributeValue("fill", $"url(#{gradientId})");
        _path.SetAttributeValue("fill-rule", "evenodd");
        Defs.Add(gradientElement);
    }
}