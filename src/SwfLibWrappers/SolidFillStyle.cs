using OneOf;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public readonly struct SolidFillStyle
{
    public OneOf<SolidFillStyleRGB, SolidFillStyleRGBA> Internal { get; }

    public SolidFillStyle(SolidFillStyleRGB rgb) => Internal = rgb;
    public static implicit operator SolidFillStyle(SolidFillStyleRGB rgb) => new(rgb);
    public SolidFillStyle(SolidFillStyleRGBA rgba) => Internal = rgba;
    public static implicit operator SolidFillStyle(SolidFillStyleRGBA rgba) => new(rgba);

    public SwfColor Color => Internal.Match(
        rgb => new SwfColor(rgb.Color),
        rgba => new SwfColor(rgba.Color)
    );

    public FillStyleType Type => Internal.Match(
        rgb => rgb.Type,
        rgba => rgba.Type
    );
}