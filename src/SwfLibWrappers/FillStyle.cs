using OneOf;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public readonly struct FillStyle
{
    public OneOf<FillStyleRGB, FillStyleRGBA> Internal { get; }

    public FillStyle(FillStyleRGB rgb) => Internal = rgb;
    public static FillStyle New(FillStyleRGB rgb) => new(rgb);
    public static implicit operator FillStyle(FillStyleRGB rgb) => new(rgb);
    public FillStyle(FillStyleRGBA rgba) => Internal = rgba;
    public static FillStyle New(FillStyleRGBA rgba) => new(rgba);
    public static implicit operator FillStyle(FillStyleRGBA rgba) => new(rgba);

    public FillStyleType Type => Internal.Match(
        rgb => rgb.Type,
        rgba => rgba.Type
    );

    public SolidFillStyle ToSolidFillStyle() => Internal.Match(
        rgb => new SolidFillStyle((SolidFillStyleRGB)rgb),
        rgba => new SolidFillStyle((SolidFillStyleRGBA)rgba)
    );

    public LinearGradientFillStyle ToLinearGradientFillStyle() => Internal.Match(
        rgb => new LinearGradientFillStyle((LinearGradientFillStyleRGB)rgb),
        rgba => new LinearGradientFillStyle((LinearGradientFillStyleRGBA)rgba)
    );

    public RadialGradientFillStyle ToRadialGradientFillStyle() => Internal.Match(
        rgb => new RadialGradientFillStyle((RadialGradientFillStyleRGB)rgb),
        rgba => new RadialGradientFillStyle((RadialGradientFillStyleRGBA)rgba)
    );

    public FocalGradientFillStyle ToFocalGradientFillStyle() => Internal.Match(
        rgb => new FocalGradientFillStyle((FocalGradientFillStyleRGB)rgb),
        rgba => new FocalGradientFillStyle((FocalGradientFillStyleRGBA)rgba)
    );
}