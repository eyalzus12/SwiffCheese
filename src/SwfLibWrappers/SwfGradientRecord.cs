using OneOf;
using SwfLib.Gradients;

namespace SwiffCheese.Wrappers;

public readonly struct SwfGradientRecord
{
    private readonly OneOf<GradientRecordRGB, GradientRecordRGBA> _internal;

    public SwfGradientRecord(GradientRecordRGB rgb) => _internal = rgb;
    public static implicit operator SwfGradientRecord(GradientRecordRGB rgb) => new(rgb);
    public SwfGradientRecord(GradientRecordRGBA rgba) => _internal = rgba;
    public static implicit operator SwfGradientRecord(GradientRecordRGBA rgba) => new(rgba);

    public byte Ratio => _internal.Match(
        (GradientRecordRGB rgb) => rgb.Ratio,
        (GradientRecordRGBA rgba) => rgba.Ratio
    );

    public SwfColor Color => _internal.Match(
        (GradientRecordRGB rgb) => new SwfColor(rgb.Color),
        (GradientRecordRGBA rgba) => new SwfColor(rgba.Color)
    );
}