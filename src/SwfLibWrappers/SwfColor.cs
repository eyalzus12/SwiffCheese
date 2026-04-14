using SwfLib.Data;

namespace SwiffCheese.Wrappers;

public readonly struct SwfColor
{
    public byte Red { get; }
    public byte Green { get; }
    public byte Blue { get; }
    public byte Alpha { get; }

    public SwfColor(SwfRGB rgb)
    {
        Red = rgb.Red;
        Green = rgb.Green;
        Blue = rgb.Blue;
        Alpha = 255;
    }

    public SwfColor(SwfRGBA rgba)
    {
        Red = rgba.Red;
        Green = rgba.Green;
        Blue = rgba.Blue;
        Alpha = rgba.Alpha;
    }

    public static implicit operator SwfColor(SwfRGB rgb) => new(rgb);
    public static implicit operator SwfColor(SwfRGBA rgba) => new(rgba);
}