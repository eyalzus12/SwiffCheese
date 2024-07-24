using OneOf;
using SwfLib.Shapes.Records;

namespace SwiffCheese.Wrappers;

public readonly struct ShapeRecord
{
    private readonly OneOf<IShapeRecordRGB, IShapeRecordRGBA, IShapeRecordEx> _internal;

    public ShapeRecord(IShapeRecordRGB record) => _internal = OneOf<IShapeRecordRGB, IShapeRecordRGBA, IShapeRecordEx>.FromT0(record);
    public ShapeRecord(IShapeRecordRGBA record) => _internal = OneOf<IShapeRecordRGB, IShapeRecordRGBA, IShapeRecordEx>.FromT1(record);
    public ShapeRecord(IShapeRecordEx record) => _internal = OneOf<IShapeRecordRGB, IShapeRecordRGBA, IShapeRecordEx>.FromT2(record);

    public ShapeRecordType Type => _internal.Match(
        (IShapeRecordRGB rgb) => rgb.Type,
        (IShapeRecordRGBA rgba) => rgba.Type,
        (IShapeRecordEx rgba) => rgba.Type
    );

    public StyleChangeRecord AsStyleChangeRecord() => _internal.Match(
        (IShapeRecordRGB rgb) => new StyleChangeRecord((StyleChangeShapeRecordRGB)rgb),
        (IShapeRecordRGBA rgba) => new StyleChangeRecord((StyleChangeShapeRecordRGBA)rgba),
        (IShapeRecordEx ex) => new StyleChangeRecord((StyleChangeShapeRecordEx)ex)
    );

    public StraightEdgeShapeRecord AsStraightEdgeRecord() => _internal.Match(
        (IShapeRecordRGB rgb) => (StraightEdgeShapeRecord)rgb,
        (IShapeRecordRGBA rgba) => (StraightEdgeShapeRecord)rgba,
        (IShapeRecordEx ex) => (StraightEdgeShapeRecord)ex
    );

    public CurvedEdgeShapeRecord AsCurvedEdgeRecord() => _internal.Match(
        (IShapeRecordRGB rgb) => (CurvedEdgeShapeRecord)rgb,
        (IShapeRecordRGBA rgba) => (CurvedEdgeShapeRecord)rgba,
        (IShapeRecordEx ex) => (CurvedEdgeShapeRecord)ex
    );

    public EndShapeRecord AsEndShapeRecord() => _internal.Match(
        (IShapeRecordRGB rgb) => (EndShapeRecord)rgb,
        (IShapeRecordRGBA rgba) => (EndShapeRecord)rgba,
        (IShapeRecordEx ex) => (EndShapeRecord)ex
    );
}