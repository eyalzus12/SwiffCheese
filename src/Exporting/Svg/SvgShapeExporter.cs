using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Xml.Linq;
using SwfLib.Data;
using SwfLib.Gradients;
using SwiffCheese.Wrappers;

namespace SwiffCheese.Exporting.Svg;

public class SvgShapeExporter(Vector2 position, Vector2 size, float unitDivisor = 20) : IShapeExporter
{
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
                _defs = new("defs");
                _svg.Add(_defs);
            }
            return _defs;
        }
    }

    public void BeginShape()
    {
        _svg = new("svg", _group);
        Document = new(new XDeclaration("1.0", "UTF-8", "no"), _svg);

        _svg.SetAttributeValue("xmlns:xlink", "http://www.w3.org/1999/xlink");
        _svg.SetElementValue("xmlns", "http://www.w3.org/2000/svg");

        _svg.SetAttributeValue("Width", $"{size.X / unitDivisor}px");
        _svg.SetAttributeValue("Height", $"{size.Y / unitDivisor}px");

        _group = new XElement("g");
        _group.SetAttributeValue("transform", SvgUtils.TransMatrixToSvgString(position, unitDivisor));

        _svg.Add(_group);

        _path = new("path");
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
        XElement gradientElement = new("linearGradient");
        SwfGradient gradient = fillStyle.Gradient;
        PopulateGradientElement(gradientElement, gradient.GradientRecords, fillStyle.GradientMatrix, gradient.SpreadMode, gradient.InterpolationMode);
        AddGradientElement(gradientElement);
    }

    public void BeginRadialGradientFill(RadialGradientFillStyle fillStyle)
    {
        FinalizePath();
        XElement gradientElement = new("radialGradient");
        SwfGradient gradient = fillStyle.Gradient;
        PopulateGradientElement(gradientElement, gradient.GradientRecords, fillStyle.GradientMatrix, gradient.SpreadMode, gradient.InterpolationMode, 0);
        AddGradientElement(gradientElement);
    }

    public void BeginFocalGradientFill(FocalGradientFillStyle fillStyle)
    {
        FinalizePath();
        XElement gradientElement = new("radialGradient");
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
    }

    public void MoveTo(Vector2 pos)
    {
        _currentDrawCommand = "";
        pos /= unitDivisor;
        _pathData.Append($"M{SvgUtils.RoundPixels20(pos.X)} {SvgUtils.RoundPixels20(pos.Y)} ");
    }

    public void LineTo(Vector2 pos)
    {
        if (_currentDrawCommand != "L")
        {
            _currentDrawCommand = "L";
            _pathData.Append('L');
        }
        pos /= unitDivisor;
        _pathData.Append($"{SvgUtils.RoundPixels20(pos.X)} {SvgUtils.RoundPixels20(pos.Y)} ");
    }

    public void CurveTo(Vector2 anchor, Vector2 to)
    {
        if (_currentDrawCommand != "Q")
        {
            _currentDrawCommand = "Q";
            _pathData.Append('Q');
        }
        anchor /= unitDivisor;
        to /= unitDivisor;
        _pathData.Append($"{SvgUtils.RoundPixels20(anchor.X)} {SvgUtils.RoundPixels20(anchor.Y)} ${SvgUtils.RoundPixels20(to.X)} ${SvgUtils.RoundPixels20(to.Y)} ");
    }

    public void FinalizePath()
    {
        _path.SetAttributeValue("d", _pathData.ToString().Trim());
        _group.Add(_path);
        _path = new XElement("path");
        _pathData.Clear();
        _currentDrawCommand = "";
    }

    private void PopulateGradientElement(XElement gradient, IEnumerable<SwfGradientRecord> records, SwfMatrix matrix, SpreadMode spreadMode, InterpolationMode interpolationMode, double? focalPointRatio = null)
    {
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
                gradient.SetAttributeValue("fx", (819.2 * focalPointRatio).ToString());
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

        gradient.SetAttributeValue("gradientTransform", SvgUtils.MatrixToSvgString(matrix, unitDivisor));

        foreach (SwfGradientRecord record in records)
        {
            XElement entry = new("stop");
            entry.SetAttributeValue("offset", record.Ratio / 255.0);
            entry.SetAttributeValue("stop-color", SvgUtils.ColorToHexString(record.Color));
            if (record.Color.Alpha != 255)
                entry.SetAttributeValue("stop-opacity", (record.Color.Alpha / 255.0f).ToString());
            gradient.Add(entry);
        }
    }

    private void AddGradientElement(XElement gradientElement)
    {
        string gradientId = $"gradient{_gradientCount++}";
        gradientElement.SetAttributeValue("id", gradientId);
        _path.SetAttributeValue("stroke", "none");
        _path.SetAttributeValue("fill", $"#url({gradientId})");
        _path.SetAttributeValue("fill-rule", "evenodd");
        Defs.Add(gradientElement);
    }
}