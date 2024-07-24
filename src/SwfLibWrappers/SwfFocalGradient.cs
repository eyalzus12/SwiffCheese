using System.Collections.Generic;
using System.Linq;
using OneOf;
using SwfLib.Gradients;

namespace SwiffCheese.Wrappers;

public readonly struct SwfFocalGradient
{
    private readonly OneOf<FocalGradientRGB, FocalGradientRGBA> _internal;

    public SwfFocalGradient(FocalGradientRGB rgb) => _internal = rgb;
    public static implicit operator SwfFocalGradient(FocalGradientRGB rgb) => new(rgb);
    public SwfFocalGradient(FocalGradientRGBA rgba) => _internal = rgba;
    public static implicit operator SwfFocalGradient(FocalGradientRGBA rgba) => new(rgba);

    public IEnumerable<SwfGradientRecord> GradientRecords => _internal.Match(
        (FocalGradientRGB rgb) => rgb.GradientRecords.Select(_ => new SwfGradientRecord(_)),
        (FocalGradientRGBA rgba) => rgba.GradientRecords.Select(_ => new SwfGradientRecord(_))
    );

    public SpreadMode SpreadMode => _internal.Match(
        (FocalGradientRGB rgb) => rgb.SpreadMode,
        (FocalGradientRGBA rgba) => rgba.SpreadMode
    );

    public InterpolationMode InterpolationMode => _internal.Match(
        (FocalGradientRGB rgb) => rgb.InterpolationMode,
        (FocalGradientRGBA rgba) => rgba.InterpolationMode
    );

    public double FocalPoint => _internal.Match(
        (FocalGradientRGB rgb) => rgb.FocalPoint,
        (FocalGradientRGBA rgba) => rgba.FocalPoint
    );
}