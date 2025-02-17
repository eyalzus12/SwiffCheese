using OneOf;
using SwfLib.Data;

namespace SwiffCheese.Wrappers;

public readonly struct SwfColor
{
    public OneOf<SwfRGB, SwfRGBA> Internal { get; }

    public SwfColor(SwfRGB rgb) => Internal = rgb;
    public static implicit operator SwfColor(SwfRGB rgb) => new(rgb);
    public SwfColor(SwfRGBA rgba) => Internal = rgba;
    public static implicit operator SwfColor(SwfRGBA rgba) => new(rgba);

    public byte Red => Internal.Match(rgb => rgb.Red, rgba => rgba.Red);
    public byte Green => Internal.Match(rgb => rgb.Green, rgba => rgba.Green);
    public byte Blue => Internal.Match(rgb => rgb.Blue, rgba => rgba.Blue);
    public byte Alpha => Internal.Match(rgb => (byte)255, rgba => rgba.Alpha);
}