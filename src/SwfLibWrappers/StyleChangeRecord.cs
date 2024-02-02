using System.Collections.Generic;
using System.Linq;
using OneOf;
using SwfLib.Shapes.Records;

namespace SwiffCheese.Wrappers;

public class StyleChangeRecord
{
    private readonly OneOf<StyleChangeShapeRecordRGB, StyleChangeShapeRecordRGBA> _internal;
    
    public StyleChangeRecord(StyleChangeShapeRecordRGB rgb)
    {
        _internal = rgb;
    }

    public StyleChangeRecord(StyleChangeShapeRecordRGBA rgba)
    {
        _internal = rgba;
    }

    public uint? FillStyle0 => _internal.Match(
        (StyleChangeShapeRecordRGB rgb) => rgb.FillStyle0,
        (StyleChangeShapeRecordRGBA rgba) => rgba.FillStyle0
    );

    public uint? FillStyle1 => _internal.Match(
        (StyleChangeShapeRecordRGB rgb) => rgb.FillStyle1,
        (StyleChangeShapeRecordRGBA rgba) => rgba.FillStyle1
    );

    public uint? LineStyle => _internal.Match(
        (StyleChangeShapeRecordRGB rgb) => rgb.LineStyle,
        (StyleChangeShapeRecordRGBA rgba) => rgba.LineStyle
    );

    public bool StateMoveTo => _internal.Match(
        (StyleChangeShapeRecordRGB rgb) => rgb.StateMoveTo,
        (StyleChangeShapeRecordRGBA rgba) => rgba.StateMoveTo
    );

    public bool StateNewStyles => _internal.Match(
        (StyleChangeShapeRecordRGB rgb) => rgb.StateNewStyles,
        (StyleChangeShapeRecordRGBA rgba) => rgba.StateNewStyles
    );

    public int MoveDeltaX => _internal.Match(
        (StyleChangeShapeRecordRGB rgb) => rgb.MoveDeltaX,
        (StyleChangeShapeRecordRGBA rgba) => rgba.MoveDeltaX
    );

    public int MoveDeltaY => _internal.Match(
        (StyleChangeShapeRecordRGB rgb) => rgb.MoveDeltaY,
        (StyleChangeShapeRecordRGBA rgba) => rgba.MoveDeltaY
    );

    public ShapeRecordType Type => _internal.Match(
        (StyleChangeShapeRecordRGB rgb) => rgb.Type,
        (StyleChangeShapeRecordRGBA rgba) => rgba.Type
    );

    public IEnumerable<FillStyle> FillStyles => _internal.Match(
        (StyleChangeShapeRecordRGB rgb) => rgb.FillStyles.Select(_ => new FillStyle(_)),
        (StyleChangeShapeRecordRGBA rgba) => rgba.FillStyles.Select(_ => new FillStyle(_))
    );

    public IEnumerable<LineStyle> LineStyles => _internal.Match(
        (StyleChangeShapeRecordRGB rgb) => rgb.LineStyles.Select(_ => new LineStyle(_)),
        (StyleChangeShapeRecordRGBA rgba) => rgba.LineStyles.Select(_ => new LineStyle(_))
    );
}