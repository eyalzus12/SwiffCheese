using SwfLib.Data;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public readonly struct LinearGradientFillStyle
{
    public SwfMatrix GradientMatrix { get; }
    public SwfGradient Gradient { get; }

    public LinearGradientFillStyle(LinearGradientFillStyleRGB rgb)
    {
        GradientMatrix = rgb.GradientMatrix;
        Gradient = rgb.Gradient;
    }

    public LinearGradientFillStyle(LinearGradientFillStyleRGBA rgba)
    {
        GradientMatrix = rgba.GradientMatrix;
        Gradient = rgba.Gradient;
    }

    public static implicit operator LinearGradientFillStyle(LinearGradientFillStyleRGB rgb) => new(rgb);
    public static implicit operator LinearGradientFillStyle(LinearGradientFillStyleRGBA rgba) => new(rgba);
}