using OneOf;
using SwfLib.Data;

namespace SwiffCheese.Wrappers;

public readonly struct SwfColor
{
    private readonly OneOf<SwfRGB, SwfRGBA> _internal;

    public SwfColor(SwfRGB rgb)
    {
        _internal = rgb;
    }

    public SwfColor(SwfRGBA rgba)
    {
        _internal = rgba;
    }

    public byte Red => _internal.Match((SwfRGB rgb) => rgb.Red, (SwfRGBA rgba) => rgba.Red);
    public byte Green => _internal.Match((SwfRGB rgb) => rgb.Green, (SwfRGBA rgba) => rgba.Green);
    public byte Blue => _internal.Match((SwfRGB rgb) => rgb.Blue, (SwfRGBA rgba) => rgba.Blue);
    public byte Alpha => _internal.Match((SwfRGB rgb) => (byte)255, (SwfRGBA rgba) => rgba.Alpha);
}