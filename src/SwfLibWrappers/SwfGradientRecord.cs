using SwfLib.Gradients;

namespace SwiffCheese.Wrappers;

public readonly struct SwfGradientRecord
{
    public byte Ratio { get; }
    public SwfColor Color { get; }

    public SwfGradientRecord(GradientRecordRGB rgb)
    {
        Ratio = rgb.Ratio;
        Color = rgb.Color;
    }

    public SwfGradientRecord(GradientRecordRGBA rgba)
    {
        Ratio = rgba.Ratio;
        Color = rgba.Color;
    }

    public static SwfGradientRecord New(GradientRecordRGB rgb) => new(rgb);
    public static SwfGradientRecord New(GradientRecordRGBA rgba) => new(rgba);

    public static implicit operator SwfGradientRecord(GradientRecordRGB rgb) => new(rgb);
    public static implicit operator SwfGradientRecord(GradientRecordRGBA rgba) => new(rgba);
}