using OneOf;
using SwfLib.Shapes.LineStyles;

namespace SwiffCheese.Wrappers;

public readonly struct LineStyle
{
    public OneOf<LineStyleRGB, LineStyleRGBA, LineStyleEx> Internal { get; }

    public LineStyle(LineStyleRGB rgb) => Internal = rgb;
    public static LineStyle New(LineStyleRGB rgb) => new(rgb);
    public static implicit operator LineStyle(LineStyleRGB rgb) => new(rgb);
    public LineStyle(LineStyleRGBA rgba) => Internal = rgba;
    public static LineStyle New(LineStyleRGBA rgba) => new(rgba);
    public static implicit operator LineStyle(LineStyleRGBA rgba) => new(rgba);
    public LineStyle(LineStyleEx ex) => Internal = ex;
    public static LineStyle New(LineStyleEx ex) => new(ex);
    public static implicit operator LineStyle(LineStyleEx ex) => new(ex);

    public ushort Width => Internal.Match(
        rgb => rgb.Width,
        rgba => rgba.Width,
        ex => ex.Width
    );

    public SwfColor Color => Internal.Match(
        rgb => new SwfColor(rgb.Color),
        rgba => new SwfColor(rgba.Color),
        ex => new SwfColor(ex.Color)
    );
}