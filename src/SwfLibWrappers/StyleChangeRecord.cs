using System.Collections.Generic;
using System.Linq;
using OneOf;
using SwfLib.Shapes.Records;

namespace SwiffCheese.Wrappers;

public readonly struct StyleChangeRecord
{
    public OneOf<StyleChangeShapeRecordRGB, StyleChangeShapeRecordRGBA, StyleChangeShapeRecordEx> Internal { get; }

    public StyleChangeRecord(StyleChangeShapeRecordRGB rgb) => Internal = rgb;
    public static implicit operator StyleChangeRecord(StyleChangeShapeRecordRGB rgb) => new(rgb);
    public StyleChangeRecord(StyleChangeShapeRecordRGBA rgba) => Internal = rgba;
    public static implicit operator StyleChangeRecord(StyleChangeShapeRecordRGBA rgba) => new(rgba);
    public StyleChangeRecord(StyleChangeShapeRecordEx ex) => Internal = ex;
    public static implicit operator StyleChangeRecord(StyleChangeShapeRecordEx ex) => new(ex);

    public uint? FillStyle0 => Internal.Match(
        rgb => rgb.FillStyle0,
        rgba => rgba.FillStyle0,
        ex => ex.FillStyle0
    );

    public uint? FillStyle1 => Internal.Match(
        rgb => rgb.FillStyle1,
        rgba => rgba.FillStyle1,
        ex => ex.FillStyle1
    );

    public uint? LineStyle => Internal.Match(
        rgb => rgb.LineStyle,
        rgba => rgba.LineStyle,
        ex => ex.LineStyle
    );

    public bool StateMoveTo => Internal.Match(
        rgb => rgb.StateMoveTo,
        rgba => rgba.StateMoveTo,
        ex => ex.StateMoveTo
    );

    public bool StateNewStyles => Internal.Match(
        rgb => rgb.StateNewStyles,
        rgba => rgba.StateNewStyles,
        ex => ex.StateNewStyles
    );

    public int MoveDeltaX => Internal.Match(
        rgb => rgb.MoveDeltaX,
        rgba => rgba.MoveDeltaX,
        ex => ex.MoveDeltaX
    );

    public int MoveDeltaY => Internal.Match(
        rgb => rgb.MoveDeltaY,
        rgba => rgba.MoveDeltaY,
        ex => ex.MoveDeltaY
    );

    public ShapeRecordType Type => Internal.Match(
        rgb => rgb.Type,
        rgba => rgba.Type,
        ex => ex.Type
    );

    public IEnumerable<FillStyle> FillStyles => Internal.Match(
        rgb => rgb.FillStyles.Select(_ => new FillStyle(_)),
        rgba => rgba.FillStyles.Select(_ => new FillStyle(_)),
        ex => ex.FillStyles.Select(_ => new FillStyle(_))
    );

    public IEnumerable<LineStyle> LineStyles => Internal.Match(
        rgb => rgb.LineStyles.Select(_ => new LineStyle(_)),
        rgba => rgba.LineStyles.Select(_ => new LineStyle(_)),
        ex => ex.LineStyles.Select(_ => new LineStyle(_))
    );
}