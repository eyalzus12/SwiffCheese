using System;
using System.Collections.Generic;
using System.Linq;
using OneOf;
using SwfLib.Data;
using SwfLib.Tags.ShapeTags;

namespace SwiffCheese.Wrappers;

public readonly struct DefineShapeXTag
{
    public OneOf<DefineShapeTag, DefineShape2Tag, DefineShape3Tag, DefineShape4Tag> Internal { get; }

    public DefineShapeXTag(ShapeBaseTag shape)
    {
        Internal = shape switch
        {
            DefineShapeTag shape1 => shape1,
            DefineShape2Tag shape2 => shape2,
            DefineShape3Tag shape3 => shape3,
            DefineShape4Tag shape4 => shape4,
            _ => throw new NotImplementedException($"Shape type {shape.GetType().Name} is not supported")
        };
    }

    public DefineShapeXTag(DefineShapeTag shape) => Internal = shape;
    public static implicit operator DefineShapeXTag(DefineShapeTag shape) => new(shape);
    public DefineShapeXTag(DefineShape2Tag shape) => Internal = shape;
    public static implicit operator DefineShapeXTag(DefineShape2Tag shape) => new(shape);
    public DefineShapeXTag(DefineShape3Tag shape) => Internal = shape;
    public static implicit operator DefineShapeXTag(DefineShape3Tag shape) => new(shape);

    public IEnumerable<FillStyle> FillStyles => Internal.Match(
        shape => shape.FillStyles.Select(_ => new FillStyle(_)),
        shape => shape.FillStyles.Select(_ => new FillStyle(_)),
        shape => shape.FillStyles.Select(_ => new FillStyle(_)),
        shape => shape.FillStyles.Select(_ => new FillStyle(_))
    );

    public IEnumerable<LineStyle> LineStyles => Internal.Match(
        shape => shape.LineStyles.Select(_ => new LineStyle(_)),
        shape => shape.LineStyles.Select(_ => new LineStyle(_)),
        shape => shape.LineStyles.Select(_ => new LineStyle(_)),
        shape => shape.LineStyles.Select(_ => new LineStyle(_))
    );

    public IEnumerable<ShapeRecord> ShapeRecords => Internal.Match(
        shape => shape.ShapeRecords.Select(_ => new ShapeRecord(_)),
        shape => shape.ShapeRecords.Select(_ => new ShapeRecord(_)),
        shape => shape.ShapeRecords.Select(_ => new ShapeRecord(_)),
        shape => shape.ShapeRecords.Select(_ => new ShapeRecord(_))
    );

    public ushort ShapeID => Internal.Match(
        shape => shape.ShapeID,
        shape => shape.ShapeID,
        shape => shape.ShapeID,
        shape => shape.ShapeID
    );

    public SwfRect ShapeBounds => Internal.Match(
        shape => shape.ShapeBounds,
        shape => shape.ShapeBounds,
        shape => shape.ShapeBounds,
        shape => shape.ShapeBounds
    );
}