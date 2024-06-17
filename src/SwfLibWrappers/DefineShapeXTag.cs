using System;
using System.Collections.Generic;
using System.Linq;
using OneOf;
using SwfLib.Data;
using SwfLib.Tags.ShapeTags;

namespace SwiffCheese.Wrappers;

public readonly struct DefineShapeXTag
{
    private readonly OneOf<DefineShapeTag, DefineShape2Tag, DefineShape3Tag> _internal;

    public DefineShapeXTag(ShapeBaseTag shape)
    {
        _internal = shape switch
        {
            DefineShapeTag shape1 => shape1,
            DefineShape2Tag shape2 => shape2,
            DefineShape3Tag shape3 => shape3,
            _ => throw new NotImplementedException($"Shape type {shape.GetType().Name} is not supported")
        };
    }

    public DefineShapeXTag(DefineShapeTag shape) => _internal = shape;
    public static implicit operator DefineShapeXTag(DefineShapeTag shape) => new(shape);
    public DefineShapeXTag(DefineShape2Tag shape) => _internal = shape;
    public static implicit operator DefineShapeXTag(DefineShape2Tag shape) => new(shape);
    public DefineShapeXTag(DefineShape3Tag shape) => _internal = shape;
    public static implicit operator DefineShapeXTag(DefineShape3Tag shape) => new(shape);

    public IEnumerable<FillStyle> FillStyles => _internal.Match(
        (DefineShapeTag shape) => shape.FillStyles.Select(_ => new FillStyle(_)),
        (DefineShape2Tag shape) => shape.FillStyles.Select(_ => new FillStyle(_)),
        (DefineShape3Tag shape) => shape.FillStyles.Select(_ => new FillStyle(_))
    );

    public IEnumerable<LineStyle> LineStyles => _internal.Match(
        (DefineShapeTag shape) => shape.LineStyles.Select(_ => new LineStyle(_)),
        (DefineShape2Tag shape) => shape.LineStyles.Select(_ => new LineStyle(_)),
        (DefineShape3Tag shape) => shape.LineStyles.Select(_ => new LineStyle(_))
    );

    public IEnumerable<ShapeRecord> ShapeRecords => _internal.Match(
        (DefineShapeTag shape) => shape.ShapeRecords.Select(_ => new ShapeRecord(_)),
        (DefineShape2Tag shape) => shape.ShapeRecords.Select(_ => new ShapeRecord(_)),
        (DefineShape3Tag shape) => shape.ShapeRecords.Select(_ => new ShapeRecord(_))
    );

    public ushort ShapeID => _internal.Match(
        (DefineShapeTag shape) => shape.ShapeID,
        (DefineShape2Tag shape) => shape.ShapeID,
        (DefineShape3Tag shape) => shape.ShapeID
    );

    public SwfRect ShapeBounds => _internal.Match(
        (DefineShapeTag shape) => shape.ShapeBounds,
        (DefineShape2Tag shape) => shape.ShapeBounds,
        (DefineShape3Tag shape) => shape.ShapeBounds
    );
}