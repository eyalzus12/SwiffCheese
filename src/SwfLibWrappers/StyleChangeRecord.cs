using System;
using System.Collections.Generic;
using System.Linq;
using SwfLib.Shapes.Records;

namespace SwiffCheese.Wrappers;

public readonly struct StyleChangeRecord(StyleChangeShapeRecord record)
{
    public uint? FillStyle0 { get; } = record.FillStyle0;
    public uint? FillStyle1 { get; } = record.FillStyle1;
    public uint? LineStyle { get; } = record.LineStyle;
    public bool StateMoveTo { get; } = record.StateMoveTo;
    public bool StateNewStyles { get; } = record.StateNewStyles;
    public int MoveDeltaX { get; } = record.MoveDeltaX;
    public int MoveDeltaY { get; } = record.MoveDeltaY;
    public ShapeRecordType Type { get; } = record.Type;
    public List<FillStyle> FillStyles { get; } = record switch
    {
        StyleChangeShapeRecordRGB rgb => [.. rgb.FillStyles.Select(FillStyle.New)],
        StyleChangeShapeRecordRGBA rgba => [.. rgba.FillStyles.Select(FillStyle.New)],
        StyleChangeShapeRecordEx ex => [.. ex.FillStyles.Select(FillStyle.New)],
        _ => throw new ArgumentException(),
    };
    public List<LineStyle> LineStyles { get; } = record switch
    {
        StyleChangeShapeRecordRGB rgb => [.. rgb.LineStyles.Select(Wrappers.LineStyle.New)],
        StyleChangeShapeRecordRGBA rgba => [.. rgba.LineStyles.Select(Wrappers.LineStyle.New)],
        StyleChangeShapeRecordEx ex => [.. ex.LineStyles.Select(Wrappers.LineStyle.New)],
        _ => throw new ArgumentException(),
    };

    public static implicit operator StyleChangeRecord(StyleChangeShapeRecordRGB rgb) => new(rgb);
    public static implicit operator StyleChangeRecord(StyleChangeShapeRecordRGBA rgba) => new(rgba);
    public static implicit operator StyleChangeRecord(StyleChangeShapeRecordEx ex) => new(ex);
}