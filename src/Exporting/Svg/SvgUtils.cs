using System;
using System.Numerics;
using SwfLib.Data;
using SwiffCheese.Wrappers;

namespace SwiffCheese.Exporting.Svg;

internal static class SvgUtils
{
    internal static string MatrixToSvgString(SwfMatrix matrix, float unitDivisor)
    {
        double translateX = RoundPixels400(matrix.TranslateX / unitDivisor);
        double translateY = RoundPixels400(matrix.TranslateY / unitDivisor);
        double rotateSkew0 = RoundPixels400(matrix.RotateSkew0);
        double rotateSkew1 = RoundPixels400(matrix.RotateSkew1);
        double scaleX = RoundPixels400(matrix.ScaleX);
        double scaleY = RoundPixels400(matrix.ScaleY);
        return $"matrix({scaleX}, {rotateSkew0}, {rotateSkew1}, {scaleY}, {translateX}, {translateY})";
    }

    internal static string TransMatrixToSvgString(Vector2 translate, float unitDivisor)
    {
        double translateX = RoundPixels400(translate.X / unitDivisor);
        double translateY = RoundPixels400(translate.Y / unitDivisor);
        return $"matrix(1, 0, 0, 1, {translateX}, {translateY})";
    }

    internal static double RoundPixels20(double pixels)
    {
        return Math.Round(pixels, 3);
    }

    internal static double RoundPixels400(double pixels)
    {
        return Math.Round(pixels, 4);
    }


    internal static string ColorToHexString(SwfColor color)
    {
        return $"#{color.Red:X2}{color.Green:X2}{color.Blue:X2}";
    }
}