using SwfLib.Data;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public readonly struct FocalGradientFillStyle
{
    public SwfMatrix GradientMatrix { get; }
    public SwfFocalGradient Gradient { get; }

    public FocalGradientFillStyle(FocalGradientFillStyleRGB rgb)
    {
        GradientMatrix = rgb.GradientMatrix;
        Gradient = rgb.Gradient;
    }

    public FocalGradientFillStyle(FocalGradientFillStyleRGBA rgba)
    {
        GradientMatrix = rgba.GradientMatrix;
        Gradient = rgba.Gradient;
    }

    public static implicit operator FocalGradientFillStyle(FocalGradientFillStyleRGB rgb) => new(rgb);
    public static implicit operator FocalGradientFillStyle(FocalGradientFillStyleRGBA rgba) => new(rgba);
}