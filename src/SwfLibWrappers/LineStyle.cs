using OneOf;
using SwfLib.Shapes.LineStyles;

namespace SwiffCheese.Wrappers;

public readonly struct LineStyle
{
    private readonly OneOf<LineStyleRGB, LineStyleRGBA> _internal;

    public LineStyle(LineStyleRGB rgb) => _internal = rgb;
    public LineStyle(LineStyleRGBA rgba) => _internal = rgba;

    public ushort Width => _internal.Match(
        (LineStyleRGB rgb) => rgb.Width,
        (LineStyleRGBA rgba) => rgba.Width
    );

    public SwfColor Color => _internal.Match(
        (LineStyleRGB rgb) => new SwfColor(rgb.Color),
        (LineStyleRGBA rgba) => new SwfColor(rgba.Color)
    );
}