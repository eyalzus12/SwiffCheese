using OneOf;
using SwfLib.Data;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public readonly struct LinearGradientFillStyle
{
    private readonly OneOf<LinearGradientFillStyleRGB, LinearGradientFillStyleRGBA> _internal;

    public LinearGradientFillStyle(LinearGradientFillStyleRGB rgb) => _internal = rgb;
    public static implicit operator LinearGradientFillStyle(LinearGradientFillStyleRGB rgb) => new(rgb);
    public LinearGradientFillStyle(LinearGradientFillStyleRGBA rgba) => _internal = rgba;
    public static implicit operator LinearGradientFillStyle(LinearGradientFillStyleRGBA rgba) => new(rgba);

    public SwfMatrix GradientMatrix => _internal.Match(
        (LinearGradientFillStyleRGB rgb) => rgb.GradientMatrix,
        (LinearGradientFillStyleRGBA rgba) => rgba.GradientMatrix
    );

    public SwfGradient Gradient => _internal.Match(
        (LinearGradientFillStyleRGB rgb) => new SwfGradient(rgb.Gradient),
        (LinearGradientFillStyleRGBA rgba) => new SwfGradient(rgba.Gradient)
    );

    public FillStyleType Type => _internal.Match(
        (LinearGradientFillStyleRGB rgb) => rgb.Type,
        (LinearGradientFillStyleRGBA rgba) => rgba.Type
    );
}