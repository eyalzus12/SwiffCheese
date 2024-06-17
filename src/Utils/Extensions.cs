using System.Numerics;
using SixLabors.ImageSharp;
using SwfLib.Data;
using SwiffCheese.Wrappers;

namespace SwiffCheese.Utils;

public static class Extensions
{
    public static Color SwfColorToImageSharpColor(this SwfColor color) =>
        Color.FromRgba(color.Red, color.Green, color.Blue, color.Alpha);

    public static Matrix3x2 SwfMatrixToMatrix3x2(this SwfMatrix mat) => new(
        (float)mat.ScaleX, (float)mat.RotateSkew0,
        (float)mat.RotateSkew1, (float)mat.ScaleY,
        mat.TranslateX, mat.TranslateY
    );

    public static int Width(this SwfRect rect) => rect.XMax - rect.XMin;
    public static int Height(this SwfRect rect) => rect.YMax - rect.YMin;
}
