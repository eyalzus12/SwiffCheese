using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public readonly struct SolidFillStyle
{
    public SwfColor Color { get; }

    public SolidFillStyle(SolidFillStyleRGB rgb)
    {
        Color = rgb.Color;
    }

    public SolidFillStyle(SolidFillStyleRGBA rgba)
    {
        Color = rgba.Color;
    }

    public static implicit operator SolidFillStyle(SolidFillStyleRGB rgb) => new(rgb);
    public static implicit operator SolidFillStyle(SolidFillStyleRGBA rgba) => new(rgba);
}