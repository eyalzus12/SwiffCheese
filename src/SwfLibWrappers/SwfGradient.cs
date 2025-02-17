using System.Collections.Generic;
using System.Linq;
using OneOf;
using SwfLib.Gradients;

namespace SwiffCheese.Wrappers;

public readonly struct SwfGradient
{
    public OneOf<GradientRGB, GradientRGBA> Internal { get; }

    public SwfGradient(GradientRGB rgb) => Internal = rgb;
    public static implicit operator SwfGradient(GradientRGB rgb) => new(rgb);
    public SwfGradient(GradientRGBA rgba) => Internal = rgba;
    public static implicit operator SwfGradient(GradientRGBA rgba) => new(rgba);

    public IEnumerable<SwfGradientRecord> GradientRecords => Internal.Match(
        rgb => rgb.GradientRecords.Select(_ => new SwfGradientRecord(_)),
        rgba => rgba.GradientRecords.Select(_ => new SwfGradientRecord(_))
    );

    public SpreadMode SpreadMode => Internal.Match(
        rgb => rgb.SpreadMode,
        rgba => rgba.SpreadMode
    );

    public InterpolationMode InterpolationMode => Internal.Match(
        rgb => rgb.InterpolationMode,
        rgba => rgba.InterpolationMode
    );
}