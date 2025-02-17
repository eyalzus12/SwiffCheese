using OneOf;
using SwfLib.Data;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public readonly struct RadialGradientFillStyle
{
    public OneOf<RadialGradientFillStyleRGB, RadialGradientFillStyleRGBA> Internal { get; }

    public RadialGradientFillStyle(RadialGradientFillStyleRGB rgb) => Internal = rgb;
    public static implicit operator RadialGradientFillStyle(RadialGradientFillStyleRGB rgb) => new(rgb);
    public RadialGradientFillStyle(RadialGradientFillStyleRGBA rgba) => Internal = rgba;
    public static implicit operator RadialGradientFillStyle(RadialGradientFillStyleRGBA rgba) => new(rgba);

    public SwfMatrix GradientMatrix => Internal.Match(
        rgb => rgb.GradientMatrix,
        rgba => rgba.GradientMatrix
    );

    public SwfGradient Gradient => Internal.Match(
        rgb => new SwfGradient(rgb.Gradient),
        rgba => new SwfGradient(rgba.Gradient)
    );

    public FillStyleType Type => Internal.Match(
        rgb => rgb.Type,
        rgba => rgba.Type
    );
}