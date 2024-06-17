using System.Collections.Generic;
using System.Linq;
using OneOf;
using SwfLib.Gradients;

namespace SwiffCheese.Wrappers;

public readonly struct SwfGradient
{
    private readonly OneOf<GradientRGB, GradientRGBA> _internal;

    public SwfGradient(GradientRGB rgb) => _internal = rgb;
    public SwfGradient(GradientRGBA rgba) => _internal = rgba;

    public IEnumerable<SwfGradientRecord> GradientRecords => _internal.Match(
        (GradientRGB rgb) => rgb.GradientRecords.Select(_ => new SwfGradientRecord(_)),
        (GradientRGBA rgba) => rgba.GradientRecords.Select(_ => new SwfGradientRecord(_))
    );

    public SpreadMode SpreadMode => _internal.Match(
        (GradientRGB rgb) => rgb.SpreadMode,
        (GradientRGBA rgba) => rgba.SpreadMode
    );

    public InterpolationMode InterpolationMode => _internal.Match(
        (GradientRGB rgb) => rgb.InterpolationMode,
        (GradientRGBA rgba) => rgba.InterpolationMode
    );
}