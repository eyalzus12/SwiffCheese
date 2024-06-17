using OneOf;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public readonly struct SolidFillStyle
{
    private readonly OneOf<SolidFillStyleRGB, SolidFillStyleRGBA> _internal;

    public SolidFillStyle(SolidFillStyleRGB rgb) => _internal = rgb;
    public static implicit operator SolidFillStyle(SolidFillStyleRGB rgb) => new(rgb);
    public SolidFillStyle(SolidFillStyleRGBA rgba) => _internal = rgba;
    public static implicit operator SolidFillStyle(SolidFillStyleRGBA rgba) => new(rgba);

    public SwfColor Color => _internal.Match(
        (SolidFillStyleRGB rgb) => new SwfColor(rgb.Color),
        (SolidFillStyleRGBA rgba) => new SwfColor(rgba.Color)
    );

    public FillStyleType Type => _internal.Match(
        (SolidFillStyleRGB rgb) => rgb.Type,
        (SolidFillStyleRGBA rgba) => rgba.Type
    );
}