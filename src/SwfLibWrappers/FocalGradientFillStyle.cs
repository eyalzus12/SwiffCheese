using OneOf;
using SwfLib.Data;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public readonly struct FocalGradientFillStyle
{
    private readonly OneOf<FocalGradientFillStyleRGB, FocalGradientFillStyleRGBA> _internal;

    public FocalGradientFillStyle(FocalGradientFillStyleRGB rgb) => _internal = rgb;
    public static implicit operator FocalGradientFillStyle(FocalGradientFillStyleRGB rgb) => new(rgb);
    public FocalGradientFillStyle(FocalGradientFillStyleRGBA rgba) => _internal = rgba;
    public static implicit operator FocalGradientFillStyle(FocalGradientFillStyleRGBA rgba) => new(rgba);

    public SwfMatrix GradientMatrix => _internal.Match(
        (FocalGradientFillStyleRGB rgb) => rgb.GradientMatrix,
        (FocalGradientFillStyleRGBA rgba) => rgba.GradientMatrix
    );

    public SwfFocalGradient Gradient => _internal.Match(
        (FocalGradientFillStyleRGB rgb) => new SwfFocalGradient(rgb.Gradient),
        (FocalGradientFillStyleRGBA rgba) => new SwfFocalGradient(rgba.Gradient)
    );

    public FillStyleType Type => _internal.Match(
        (FocalGradientFillStyleRGB rgb) => rgb.Type,
        (FocalGradientFillStyleRGBA rgba) => rgba.Type
    );
}