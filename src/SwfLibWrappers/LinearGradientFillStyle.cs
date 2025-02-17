using OneOf;
using SwfLib.Data;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public readonly struct LinearGradientFillStyle
{
    public OneOf<LinearGradientFillStyleRGB, LinearGradientFillStyleRGBA> Internal { get; }

    public LinearGradientFillStyle(LinearGradientFillStyleRGB rgb) => Internal = rgb;
    public static implicit operator LinearGradientFillStyle(LinearGradientFillStyleRGB rgb) => new(rgb);
    public LinearGradientFillStyle(LinearGradientFillStyleRGBA rgba) => Internal = rgba;
    public static implicit operator LinearGradientFillStyle(LinearGradientFillStyleRGBA rgba) => new(rgba);

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