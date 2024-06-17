using OneOf;
using SwfLib.Data;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public readonly struct RadialGradientFillStyle
{
    private readonly OneOf<RadialGradientFillStyleRGB, RadialGradientFillStyleRGBA> _internal;

    public RadialGradientFillStyle(RadialGradientFillStyleRGB rgb) => _internal = rgb;
    public static implicit operator RadialGradientFillStyle(RadialGradientFillStyleRGB rgb) => new(rgb);
    public RadialGradientFillStyle(RadialGradientFillStyleRGBA rgba) => _internal = rgba;
    public static implicit operator RadialGradientFillStyle(RadialGradientFillStyleRGBA rgba) => new(rgba);

    public SwfMatrix GradientMatrix => _internal.Match(
        (RadialGradientFillStyleRGB rgb) => rgb.GradientMatrix,
        (RadialGradientFillStyleRGBA rgba) => rgba.GradientMatrix
    );

    public SwfGradient Gradient => _internal.Match(
        (RadialGradientFillStyleRGB rgb) => new SwfGradient(rgb.Gradient),
        (RadialGradientFillStyleRGBA rgba) => new SwfGradient(rgba.Gradient)
    );

    public FillStyleType Type => _internal.Match(
        (RadialGradientFillStyleRGB rgb) => rgb.Type,
        (RadialGradientFillStyleRGBA rgba) => rgba.Type
    );
}