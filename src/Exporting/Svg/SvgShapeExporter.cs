using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using SwfLib.Data;
using SwfLib.Gradients;
using SwfLib.Shapes.FillStyles;
using SwfLib.Shapes.LineStyles;
using SwiffCheese.Math;
using SwiffCheese.Wrappers;

namespace SwiffCheese.Exporting.Svg;

public class SvgShapeExporter(SvgSize size, SvgMatrix transform, SvgColorTransform colorTransform) : IShapeExporter
{
    private const double SWF_UNIT_DIVISOR = 20;
    private const double SWF_GRADIENT_SIZE = 819.2;
    private readonly XNamespace xmlns = XNamespace.Get("http://www.w3.org/2000/svg");

    public XDocument Document { get; private set; } = null!;

    private WindingRule _windingRule;
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

    public void BeginShape(WindingRule windingRule)
    {
        _windingRule = windingRule;

        _svg = new(xmlns + "svg");
        _svg.SetAttributeValue("width", size.Width);
        _svg.SetAttributeValue("height", size.Height);

        _group = new XElement(xmlns + "g");
        _svg.Add(_group);

        if (transform != new SvgMatrix())
        {
            string transformString = SvgUtils.SvgMatrixString(
                transform.ScaleX, transform.RotateSkew0,
                transform.RotateSkew1, transform.ScaleY,
                transform.TranslateX, transform.TranslateY
            );
            _group.SetAttributeValue("transform", transformString);
        }

        if (colorTransform != new SvgColorTransform())
        {
            string colorTransformString = SvgUtils.SvgColorMatrixString(
                colorTransform.RedMult, colorTransform.RedAdd,
                colorTransform.GreenMult, colorTransform.GreenAdd,
                colorTransform.BlueMult, colorTransform.BlueAdd,
                colorTransform.AlphaMult, colorTransform.AlphaAdd
            );
            XElement feColorMatrix = new(xmlns + "feColorMatrix");
            feColorMatrix.SetAttributeValue("values", colorTransformString);

            XElement filter = new(xmlns + "filter", feColorMatrix);
            filter.SetAttributeValue("id", "cxform");

            Defs.Add(filter);
            _group.SetAttributeValue("filter", "url(#cxform)");
        }

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

    public void BeginSolidFill(SwfColor color)
    {
        FinalizePath();
        _path.SetAttributeValue("stroke", "none");
        _path.SetAttributeValue("fill", SvgUtils.ColorToHexString(color));
        _path.SetAttributeValue("fill-rule", SvgUtils.WindingRuleToString(_windingRule));
        if (color.Alpha != 255)
            _path.SetAttributeValue("fill-opacity", color.Alpha / 255.0f);
    }

    public void BeginLinearGradientFill(SwfMatrix gradientMatrix, SwfGradient gradient)
    {
        FinalizePath();
        XElement gradientElement = new(xmlns + "linearGradient");
        PopulateGradientElement(gradientElement, gradient.GradientRecords, gradientMatrix, gradient.SpreadMode, gradient.InterpolationMode);
        AddGradientElement(gradientElement);
    }

    public void BeginRadialGradientFill(SwfMatrix gradientMatrix, SwfGradient gradient)
    {
        FinalizePath();
        XElement gradientElement = new(xmlns + "radialGradient");
        PopulateGradientElement(gradientElement, gradient.GradientRecords, gradientMatrix, gradient.SpreadMode, gradient.InterpolationMode, 0);
        AddGradientElement(gradientElement);
    }

    public void BeginFocalGradientFill(SwfMatrix gradientMatrix, SwfFocalGradient gradient)
    {
        FinalizePath();
        XElement gradientElement = new(xmlns + "radialGradient");
        PopulateGradientElement(gradientElement, gradient.GradientRecords, gradientMatrix, gradient.SpreadMode, gradient.InterpolationMode, gradient.FocalPoint);
        AddGradientElement(gradientElement);
    }

    public void BeginBitmapFill(ushort bitmapId, SwfMatrix bitmapMatrix, bool smoothing, BitmapMode mode)
    {
        // TODO: implement
        FinalizePath();
        _path.SetAttributeValue("stroke", "none");
        _path.SetAttributeValue("fill", "none");
        _path.SetAttributeValue("fill-rule", SvgUtils.WindingRuleToString(_windingRule));
    }

    public void EndFill()
    {
        FinalizePath();
    }

    public void LineStyle(float thickness, SwfColor color, bool pixelHinting, StrokeScaleMode scaleMode, CapStyle startCaps, CapStyle endCaps, JoinStyle joints, double miterLimit, bool noClose)
    {
        FinalizePath();
        double strokeWidth = thickness / SWF_UNIT_DIVISOR;
        _path.SetAttributeValue("fill", "none");
        _path.SetAttributeValue("stroke", SvgUtils.ColorToHexString(color));
        _path.SetAttributeValue("stroke-width", strokeWidth);
        if (color.Alpha != 255)
            _path.SetAttributeValue("stroke-opacity", color.Alpha / 255.0);

        _path.SetAttributeValue("stroke-linecap", startCaps switch
        {
            CapStyle.Round => "round",
            CapStyle.NoCap => "butt",
            CapStyle.Square => "square",
            _ => "round",
        });

        // svg does not support changing the cap style of each end separately

        _path.SetAttributeValue("stroke-linejoin", joints switch
        {
            JoinStyle.Round => "round",
            JoinStyle.Bevel => "bevel",
            JoinStyle.Miter => "miter-clip",
            _ => "round",
        });
        if (joints == JoinStyle.Miter && miterLimit >= 1)
        {
            _path.SetAttributeValue("stroke-miterlimit", miterLimit);
        }

        // svg only supports scaling and no scaling (can't do only horizontal or vertical)
        if (scaleMode == StrokeScaleMode.None)
        {
            _path.SetAttributeValue("vector-effect", "non-scaling-stroke");
        }

        // svg does not support using caps instead of a join at the end of a closed stroke (noClose)
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
        // Undocumented edge case. Flash's behavior seems to differ from other renderers.
        // Required for Seven's eyes and Bubble Tag's bubble.
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
            gradient.SetAttributeValue("x1", -SWF_GRADIENT_SIZE);
            gradient.SetAttributeValue("x2", SWF_GRADIENT_SIZE);
        }
        // radial or focal
        else
        {
            gradient.SetAttributeValue("r", SWF_GRADIENT_SIZE);
            gradient.SetAttributeValue("cx", 0);
            gradient.SetAttributeValue("cy", 0);
            if (focalPointRatio != 0)
            {
                gradient.SetAttributeValue("fx", SWF_GRADIENT_SIZE * focalPointRatio);
                gradient.SetAttributeValue("fy", 0);
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

        // Ratio handling logic taken from jpexs BitmapExporter: https://github.com/jindrapetrik/jpexs-decompiler/blob/bf4b6e33b77b91e442eb91b8174a07e329ccca8d/libsrc/ffdec_lib/src/com/jpexs/decompiler/flash/exporters/shape/BitmapExporter.java#L260
        // Required for Hugin's glasses to colorswap correctly.
        SwfGradientRecord? prev = null;
        int lastRatio = -1;
        foreach (SwfGradientRecord record in records)
        {
            // same offset as previous. move a tiny bit forward.
            if (prev is not null && prev.Value.Ratio == record.Ratio)
            {
                if (lastRatio < 255) lastRatio++;
            }
            // new relevant offset. set.
            else if (record.Ratio > lastRatio)
            {
                lastRatio = record.Ratio;
            }
            // new offset is before... move forward.
            else
            {
                if (lastRatio < 255) lastRatio++;
            }

            XElement entry = new(xmlns + "stop");
            entry.SetAttributeValue("offset", lastRatio / 255.0);
            entry.SetAttributeValue("stop-color", SvgUtils.ColorToHexString(record.Color));
            if (record.Color.Alpha != 255)
                entry.SetAttributeValue("stop-opacity", record.Color.Alpha / 255.0);
            gradient.Add(entry);

            if (lastRatio >= 255) break;

            prev = record;
        }
    }

    private void AddGradientElement(XElement gradientElement)
    {
        string gradientId = $"gradient{_gradientCount++}";
        gradientElement.SetAttributeValue("id", gradientId);
        _path.SetAttributeValue("stroke", "none");
        _path.SetAttributeValue("fill", $"url(#{gradientId})");
        _path.SetAttributeValue("fill-rule", SvgUtils.WindingRuleToString(_windingRule));
        Defs.Add(gradientElement);
    }
}