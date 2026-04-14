using SwfLib.Data;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public readonly struct RadialGradientFillStyle
{
    public SwfMatrix GradientMatrix { get; }
    public SwfGradient Gradient { get; }

    public RadialGradientFillStyle(RadialGradientFillStyleRGB rgb)
    {
        GradientMatrix = rgb.GradientMatrix;
        Gradient = rgb.Gradient;
    }

    public RadialGradientFillStyle(RadialGradientFillStyleRGBA rgba)
    {
        GradientMatrix = rgba.GradientMatrix;
        Gradient = rgba.Gradient;
    }

    public static implicit operator RadialGradientFillStyle(RadialGradientFillStyleRGB rgb) => new(rgb);
    public static implicit operator RadialGradientFillStyle(RadialGradientFillStyleRGBA rgba) => new(rgba);
}