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

    public double FocalPoint
    {
        get
        {
            // https://github.com/SavchukSergey/SwfLib/blob/b77bef5e259416170c820e3169dc4350d8c95e06/SwfLib/SwfStreamReader.cs#L28
            // SwfLib has a bug when reading FocalPoint (and FIXED8 in general): it reads a U16 instead of an S16.
            // so we fix the value

            double wrong = _internal.Match(
                (FocalGradientRGB rgb) => rgb.FocalPoint,
                (FocalGradientRGBA rgba) => rgba.FocalPoint
            );
            // this should be safe since floats can represent 1/2^k exactly
            ushort wrongOriginal = (ushort)(wrong * 256);
            short correctOriginal = (short)wrongOriginal;
            double correct = correctOriginal / 256.0;

            return correct;
        }
    }
}