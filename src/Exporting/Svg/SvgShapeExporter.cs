using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using SwfLib.Data;
using SwfLib.Gradients;
using SwiffCheese.Math;
using SwiffCheese.Wrappers;

namespace SwiffCheese.Exporting.Svg;

public readonly record struct SvgSize(double Width, double Height);
public readonly record struct SvgMatrix(double ScaleX = 1, double RotateSkew0 = 0, double RotateSkew1 = 0, double ScaleY = 1, double TranslateX = 0, double TranslateY = 0);

public class SvgShapeExporter(SvgSize size, SvgMatrix transform) : IShapeExporter
{

    private const double SWF_UNIT_DIVISOR = 20;
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
        string transformString = SvgUtils.SvgMatrixString(
            transform.ScaleX, transform.RotateSkew0, transform.RotateSkew1, transform.ScaleY,
            transform.TranslateX, transform.TranslateY
        );

        _group = new XElement(xmlns + "g");
        _group.SetAttributeValue("transform", transformString);

        _svg = new(xmlns + "svg", _group);
        _svg.SetAttributeValue("width", size.Width);
        _svg.SetAttributeValue("height", size.Height);

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
            _path.SetAttributeValue("fill-opacity", color.Alpha / 255.0f);
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
        double strokeWidth = thickness / SWF_UNIT_DIVISOR;
        _path.SetAttributeValue("fill", "none");
        _path.SetAttributeValue("stroke", SvgUtils.ColorToHexString(color));
        _path.SetAttributeValue("stroke-width", strokeWidth);
        if (color.Alpha != 255)
            _path.SetAttributeValue("stroke-opacity", color.Alpha / 255.0);
        _path.SetAttributeValue("stroke-linecap", "round");
        _path.SetAttributeValue("stroke-linejoin", "miter-clip");
    }

    public void MoveTo(Vector2I pos)
    {
        _currentDrawCommand = "";
        double x = pos.X / SWF_UNIT_DIVISOR;
        double y = pos.Y / SWF_UNIT_DIVISOR;
        _pathData.Append($"M{x} {y} ");
    }

    public void LineTo(Vector2I pos)
    {
        if (_currentDrawCommand != "L")
        {
            _currentDrawCommand = "L";
            _pathData.Append('L');
        }
        double x = pos.X / SWF_UNIT_DIVISOR;
        double y = pos.Y / SWF_UNIT_DIVISOR;
        _pathData.Append($"{x} {y} ");
    }

    public void CurveTo(Vector2I anchor, Vector2I to)
    {
        if (_currentDrawCommand != "Q")
        {
            _currentDrawCommand = "Q";
            _pathData.Append('Q');
        }
        double ax = anchor.X / SWF_UNIT_DIVISOR;
        double ay = anchor.Y / SWF_UNIT_DIVISOR;
        double x = to.X / SWF_UNIT_DIVISOR;
        double y = to.Y / SWF_UNIT_DIVISOR;
        _pathData.Append($"{ax} {ay} {x} {y} ");
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
        // edge case. not mentioned in spec, and flash seems to just do this thing?
        // weird af
        if (matrix.ScaleX == 0 && matrix.ScaleY == 0 && matrix.RotateSkew1 == 0 && matrix.RotateSkew0 == 0)
        {
            matrix.ScaleY = matrix.ScaleX = 1;
            matrix.RotateSkew1 = matrix.RotateSkew0 = 0;
        }

        double scaleX = SvgUtils.RoundPixels400(matrix.ScaleX);
        double rotateSkew0 = SvgUtils.RoundPixels400(matrix.RotateSkew0);
        double rotateSkew1 = SvgUtils.RoundPixels400(matrix.RotateSkew1);
        double scaleY = SvgUtils.RoundPixels400(matrix.ScaleY);
        double translateX = SvgUtils.RoundPixels400(matrix.TranslateX / SWF_UNIT_DIVISOR);
        double translateY = SvgUtils.RoundPixels400(matrix.TranslateY / SWF_UNIT_DIVISOR);
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
                entry.SetAttributeValue("stop-opacity", record.Color.Alpha / 255.0);
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