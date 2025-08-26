using System.Collections.Generic;
using System.Linq;
using OneOf;
using SwfLib.Gradients;

namespace SwiffCheese.Wrappers;

public readonly struct SwfFocalGradient
{
    public OneOf<FocalGradientRGB, FocalGradientRGBA> Internal { get; }

    public SwfFocalGradient(FocalGradientRGB rgb) => Internal = rgb;
    public static implicit operator SwfFocalGradient(FocalGradientRGB rgb) => new(rgb);
    public SwfFocalGradient(FocalGradientRGBA rgba) => Internal = rgba;
    public static implicit operator SwfFocalGradient(FocalGradientRGBA rgba) => new(rgba);

    public IEnumerable<SwfGradientRecord> GradientRecords => Internal.Match(
        rgb => rgb.GradientRecords.Select(SwfGradientRecord.New),
        rgba => rgba.GradientRecords.Select(SwfGradientRecord.New)
    );

    public SpreadMode SpreadMode => Internal.Match(
        rgb => rgb.SpreadMode,
        rgba => rgba.SpreadMode
    );

    public InterpolationMode InterpolationMode => Internal.Match(
        rgb => rgb.InterpolationMode,
        rgba => rgba.InterpolationMode
    );


    // https://github.com/SavchukSergey/SwfLib/blob/b77bef5e259416170c820e3169dc4350d8c95e06/SwfLib/SwfStreamReader.cs#L28
    // SwfLib has a bug when reading FocalPoint (and FIXED8 in general): it reads a U16 instead of an S16.
    // so we fix the value

    private double WrongFocalPoint => Internal.Match(
        rgb => rgb.FocalPoint,
        rgba => rgba.FocalPoint
    );

    public double FocalPoint
    {
        get
        {
            double wrong = WrongFocalPoint;

            // this should be safe since floats can represent 1/2^k exactly
            ushort wrongOriginal = (ushort)(wrong * 256);
            short correctOriginal = (short)wrongOriginal;
            double correct = correctOriginal / 256.0;

            return correct;
        }
    }
}