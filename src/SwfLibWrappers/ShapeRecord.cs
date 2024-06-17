using OneOf;
using SwfLib.Shapes.Records;

namespace SwiffCheese.Wrappers;

public readonly struct ShapeRecord
{
    private readonly OneOf<IShapeRecordRGB, IShapeRecordRGBA> _internal;

    public ShapeRecord(IShapeRecordRGB record) => _internal = OneOf<IShapeRecordRGB, IShapeRecordRGBA>.FromT0(record);
    public ShapeRecord(IShapeRecordRGBA record) => _internal = OneOf<IShapeRecordRGB, IShapeRecordRGBA>.FromT1(record);

    public ShapeRecordType Type => _internal.Match(
        (IShapeRecordRGB rgb) => rgb.Type,
        (IShapeRecordRGBA rgba) => rgba.Type
    );

    public StyleChangeRecord AsStyleChangeRecord() => _internal.Match(
        (IShapeRecordRGB rgb) => new StyleChangeRecord((StyleChangeShapeRecordRGB)rgb),
        (IShapeRecordRGBA rgba) => new StyleChangeRecord((StyleChangeShapeRecordRGBA)rgba)
    );

    public StraightEdgeShapeRecord AsStraightEdgeRecord() => _internal.Match(
        (IShapeRecordRGB rgb) => (StraightEdgeShapeRecord)rgb,
        (IShapeRecordRGBA rgba) => (StraightEdgeShapeRecord)rgba
    );

    public CurvedEdgeShapeRecord AsCurvedEdgeRecord() => _internal.Match(
        (IShapeRecordRGB rgb) => (CurvedEdgeShapeRecord)rgb,
        (IShapeRecordRGBA rgba) => (CurvedEdgeShapeRecord)rgba
    );

    public EndShapeRecord AsEndShapeRecord() => _internal.Match(
        (IShapeRecordRGB rgb) => (EndShapeRecord)rgb,
        (IShapeRecordRGBA rgba) => (EndShapeRecord)rgba
    );
}