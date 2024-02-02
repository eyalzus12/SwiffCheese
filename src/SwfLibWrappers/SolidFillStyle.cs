using OneOf;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public class SolidFillStyle
{
    private readonly OneOf<SolidFillStyleRGB, SolidFillStyleRGBA> _internal;

    public SolidFillStyle(SolidFillStyleRGB rgb)
    {
        _internal = rgb;
    }

    public SolidFillStyle(SolidFillStyleRGBA rgba)
    {
        _internal = rgba;
    }

    public SwfColor Color => _internal.Match(
        (SolidFillStyleRGB rgb) => new SwfColor(rgb.Color),
        (SolidFillStyleRGBA rgba) => new SwfColor(rgba.Color)
    );

    public FillStyleType Type => _internal.Match(
        (SolidFillStyleRGB rgb) => rgb.Type,
        (SolidFillStyleRGBA rgba) => rgba.Type
    );
}