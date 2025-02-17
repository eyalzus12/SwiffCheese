using OneOf;
using SwfLib.Data;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public readonly struct FocalGradientFillStyle
{
    public OneOf<FocalGradientFillStyleRGB, FocalGradientFillStyleRGBA> Internal { get; }

    public FocalGradientFillStyle(FocalGradientFillStyleRGB rgb) => Internal = rgb;
    public static implicit operator FocalGradientFillStyle(FocalGradientFillStyleRGB rgb) => new(rgb);
    public FocalGradientFillStyle(FocalGradientFillStyleRGBA rgba) => Internal = rgba;
    public static implicit operator FocalGradientFillStyle(FocalGradientFillStyleRGBA rgba) => new(rgba);

    public SwfMatrix GradientMatrix => Internal.Match(
        rgb => rgb.GradientMatrix,
        rgba => rgba.GradientMatrix
    );

    public SwfFocalGradient Gradient => Internal.Match(
        rgb => new SwfFocalGradient(rgb.Gradient),
        rgba => new SwfFocalGradient(rgba.Gradient)
    );

    public FillStyleType Type => Internal.Match(
        rgb => rgb.Type,
        rgba => rgba.Type
    );
}