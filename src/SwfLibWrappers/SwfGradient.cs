using System.Collections.Generic;
using System.Linq;
using SwfLib.Gradients;

namespace SwiffCheese.Wrappers;

public readonly struct SwfGradient
{
    public SpreadMode SpreadMode { get; }
    public InterpolationMode InterpolationMode { get; }
    public List<SwfGradientRecord> GradientRecords { get; }

    public SwfGradient(BaseGradientRGB rgb)
    {
        SpreadMode = rgb.SpreadMode;
        InterpolationMode = rgb.InterpolationMode;
        GradientRecords = [.. rgb.GradientRecords.Select(SwfGradientRecord.New)];
    }

    public SwfGradient(BaseGradientRGBA rgba)
    {
        SpreadMode = rgba.SpreadMode;
        InterpolationMode = rgba.InterpolationMode;
        GradientRecords = [.. rgba.GradientRecords.Select(SwfGradientRecord.New)];
    }

    public static implicit operator SwfGradient(BaseGradientRGB rgb) => new(rgb);
    public static implicit operator SwfGradient(BaseGradientRGBA rgba) => new(rgba);
}