using System;
using System.Collections.Generic;
using System.Linq;
using SwfLib.Data;
using SwfLib.Tags;
using SwfLib.Tags.ShapeTags;

namespace SwiffCheese.Wrappers;

public readonly struct DefineShapeXTag(ShapeBaseTag shape)
{
    public ushort ShapeID { get; } = shape.ShapeID;
    public SwfRect ShapeBounds { get; } = shape.ShapeBounds;
    public SwfRect? EdgeBounds { get; } = shape is DefineShape4Tag shape4 ? shape4.EdgeBounds : null;
    public List<FillStyle> FillStyles { get; } = shape switch
    {
        DefineShapeTag shape1 => [.. shape1.FillStyles.Select(FillStyle.New)],
        DefineShape2Tag shape2 => [.. shape2.FillStyles.Select(FillStyle.New)],
        DefineShape3Tag shape3 => [.. shape3.FillStyles.Select(FillStyle.New)],
        DefineShape4Tag shape4 => [.. shape4.FillStyles.Select(FillStyle.New)],
        _ => throw new ArgumentException(),
    };
    public List<LineStyle> LineStyles { get; } = shape switch
    {
        DefineShapeTag shape1 => [.. shape1.LineStyles.Select(LineStyle.New)],
        DefineShape2Tag shape2 => [.. shape2.LineStyles.Select(LineStyle.New)],
        DefineShape3Tag shape3 => [.. shape3.LineStyles.Select(LineStyle.New)],
        DefineShape4Tag shape4 => [.. shape4.LineStyles.Select(LineStyle.New)],
        _ => throw new ArgumentException(),
    };
    public List<ShapeRecord> ShapeRecords { get; } = shape switch
    {
        DefineShapeTag shape1 => [.. shape1.ShapeRecords.Select(ShapeRecord.New)],
        DefineShape2Tag shape2 => [.. shape2.ShapeRecords.Select(ShapeRecord.New)],
        DefineShape3Tag shape3 => [.. shape3.ShapeRecords.Select(ShapeRecord.New)],
        DefineShape4Tag shape4 => [.. shape4.ShapeRecords.Select(ShapeRecord.New)],
        _ => throw new ArgumentException(),
    };
    public SwfTagType TagType { get; } = shape.TagType;
    public WindingRule WindingRule { get; } = shape is DefineShape4Tag shape4 && shape4.UsesFillWindingRule ? WindingRule.NonZero : WindingRule.EvenOdd;
    public bool UsesNonScalingStrokes { get; } = shape is DefineShape4Tag shape4 && shape4.UsesNonScalingStrokes;
    public bool UsesScalingStrokes { get; } = shape is DefineShape4Tag shape4 && shape4.UsesScalingStrokes;

    public static implicit operator DefineShapeXTag(ShapeBaseTag shape) => new(shape);
}

public enum WindingRule
{
    EvenOdd,
    NonZero,
}