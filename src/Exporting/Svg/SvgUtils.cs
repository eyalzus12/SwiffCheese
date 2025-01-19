using SwiffCheese.Wrappers;

namespace SwiffCheese.Exporting.Svg;

internal static class SvgUtils
{
    internal static string SvgMatrixString(double scaleX, double rotateSkew0, double rotateSkew1, double scaleY, double translateX, double translateY)
    {
        return $"matrix({scaleX}, {rotateSkew0}, {rotateSkew1}, {scaleY}, {translateX}, {translateY})";
    }

    internal static double RoundPixels20(double pixels)
    {
        return System.Math.Round(pixels, 3);
    }

    internal static double RoundPixels400(double pixels)
    {
        return System.Math.Round(pixels, 4);
    }

    internal static string ColorToHexString(SwfColor color)
    {
        return $"#{color.Red:X2}{color.Green:X2}{color.Blue:X2}";
    }
}