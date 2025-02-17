using OneOf;
using SwfLib.Gradients;

namespace SwiffCheese.Wrappers;

public readonly struct SwfGradientRecord
{
    public OneOf<GradientRecordRGB, GradientRecordRGBA> Internal { get; }

    public SwfGradientRecord(GradientRecordRGB rgb) => Internal = rgb;
    public static implicit operator SwfGradientRecord(GradientRecordRGB rgb) => new(rgb);
    public SwfGradientRecord(GradientRecordRGBA rgba) => Internal = rgba;
    public static implicit operator SwfGradientRecord(GradientRecordRGBA rgba) => new(rgba);

    public byte Ratio => Internal.Match(
        rgb => rgb.Ratio,
        rgba => rgba.Ratio
    );

    public SwfColor Color => Internal.Match(
        rgb => new SwfColor(rgb.Color),
        rgba => new SwfColor(rgba.Color)
    );
}