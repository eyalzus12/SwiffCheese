using OneOf;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public class FillStyle
{
    private readonly OneOf<FillStyleRGB, FillStyleRGBA> _internal;

    public FillStyle(FillStyleRGB rgb)
    {
        _internal = rgb;
    }

    public FillStyle(FillStyleRGBA rgba)
    {
        _internal = rgba;
    }

    public FillStyleType Type => _internal.Match(
        (FillStyleRGB rgb) => rgb.Type,
        (FillStyleRGBA rgba) => rgba.Type
    );

    public SolidFillStyle AsSolidFillStyle()
    {
        return _internal.Match(
            (FillStyleRGB rgb) => new SolidFillStyle((SolidFillStyleRGB)rgb),
            (FillStyleRGBA rgba) => new SolidFillStyle((SolidFillStyleRGBA)rgba)
        );
    }
}