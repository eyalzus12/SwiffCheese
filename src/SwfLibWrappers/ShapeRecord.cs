using OneOf;
using SwfLib.Shapes.Records;

namespace SwiffCheese.Wrappers;

public readonly struct ShapeRecord
{
    public OneOf<IShapeRecordRGB, IShapeRecordRGBA, IShapeRecordEx> Internal { get; }

    public ShapeRecord(IShapeRecordRGB record) => Internal = OneOf<IShapeRecordRGB, IShapeRecordRGBA, IShapeRecordEx>.FromT0(record);
    public static ShapeRecord New(IShapeRecordRGB record) => new(record);
    public ShapeRecord(IShapeRecordRGBA record) => Internal = OneOf<IShapeRecordRGB, IShapeRecordRGBA, IShapeRecordEx>.FromT1(record);
    public static ShapeRecord New(IShapeRecordRGBA record) => new(record);
    public ShapeRecord(IShapeRecordEx record) => Internal = OneOf<IShapeRecordRGB, IShapeRecordRGBA, IShapeRecordEx>.FromT2(record);
    public static ShapeRecord New(IShapeRecordEx record) => new(record);

    public ShapeRecordType Type => Internal.Match(
        rgb => rgb.Type,
        rgba => rgba.Type,
        rgba => rgba.Type
    );

    public StyleChangeRecord ToStyleChangeRecord() => Internal.Match(
        rgb => new StyleChangeRecord((StyleChangeShapeRecordRGB)rgb),
        rgba => new StyleChangeRecord((StyleChangeShapeRecordRGBA)rgba),
        ex => new StyleChangeRecord((StyleChangeShapeRecordEx)ex)
    );

    public StraightEdgeShapeRecord ToStraightEdgeRecord() => Internal.Match(
        rgb => (StraightEdgeShapeRecord)rgb,
        rgba => (StraightEdgeShapeRecord)rgba,
        ex => (StraightEdgeShapeRecord)ex
    );

    public CurvedEdgeShapeRecord ToCurvedEdgeRecord() => Internal.Match(
        rgb => (CurvedEdgeShapeRecord)rgb,
        rgba => (CurvedEdgeShapeRecord)rgba,
        ex => (CurvedEdgeShapeRecord)ex
    );

    public EndShapeRecord ToEndShapeRecord() => Internal.Match(
        rgb => (EndShapeRecord)rgb,
        rgba => (EndShapeRecord)rgba,
        ex => (EndShapeRecord)ex
    );
}