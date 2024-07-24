using OneOf;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public readonly struct FillStyle
{
    private readonly OneOf<FillStyleRGB, FillStyleRGBA> _internal;

    public FillStyle(FillStyleRGB rgb) => _internal = rgb;
    public static implicit operator FillStyle(FillStyleRGB rgb) => new(rgb);
    public FillStyle(FillStyleRGBA rgba) => _internal = rgba;
    public static implicit operator FillStyle(FillStyleRGBA rgba) => new(rgba);

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

    public LinearGradientFillStyle AsLinearGradientFillStyle()
    {
        return _internal.Match(
            (FillStyleRGB rgb) => new LinearGradientFillStyle((LinearGradientFillStyleRGB)rgb),
            (FillStyleRGBA rgba) => new LinearGradientFillStyle((LinearGradientFillStyleRGBA)rgba)
        );
    }

    public RadialGradientFillStyle AsRadialGradientFillStyle()
    {
        return _internal.Match(
            (FillStyleRGB rgb) => new RadialGradientFillStyle((RadialGradientFillStyleRGB)rgb),
            (FillStyleRGBA rgba) => new RadialGradientFillStyle((RadialGradientFillStyleRGBA)rgba)
        );
    }

    public FocalGradientFillStyle AsFocalGradientFillStyle()
    {
        return _internal.Match(
            (FillStyleRGB rgb) => new FocalGradientFillStyle((FocalGradientFillStyleRGB)rgb),
            (FillStyleRGBA rgba) => new FocalGradientFillStyle((FocalGradientFillStyleRGBA)rgba)
        );
    }
}