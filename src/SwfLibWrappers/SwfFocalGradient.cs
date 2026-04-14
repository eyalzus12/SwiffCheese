using System.Collections.Generic;
using SwfLib.Gradients;

namespace SwiffCheese.Wrappers;

public readonly struct SwfFocalGradient
{
    private readonly SwfGradient _base;
    public SpreadMode SpreadMode => _base.SpreadMode;
    public InterpolationMode InterpolationMode => _base.InterpolationMode;
    public List<SwfGradientRecord> GradientRecords => _base.GradientRecords;
    public double FocalPoint { get; }


    public SwfFocalGradient(FocalGradientRGB rgb)
    {
        _base = rgb;
        FocalPoint = FixFocalPoint(rgb.FocalPoint);
    }

    public SwfFocalGradient(FocalGradientRGBA rgba)
    {
        _base = rgba;
        FocalPoint = FixFocalPoint(rgba.FocalPoint);
    }

    public static implicit operator SwfFocalGradient(FocalGradientRGB rgb) => new(rgb);
    public static implicit operator SwfFocalGradient(FocalGradientRGBA rgba) => new(rgba);

    // https://github.com/SavchukSergey/SwfLib/blob/b77bef5e259416170c820e3169dc4350d8c95e06/SwfLib/SwfStreamReader.cs#L28
    // SwfLib has a bug when reading FocalPoint (and FIXED8 in general): it reads a U16 instead of an S16.
    // so we fix the value
    private static double FixFocalPoint(double wrong)
    {
        // this should be safe since floats can represent 1/2^k exactly
        ushort wrongOriginal = (ushort)(wrong * 256);
        short correctOriginal = (short)wrongOriginal;
        double correct = correctOriginal / 256.0;

        return correct;
    }
}