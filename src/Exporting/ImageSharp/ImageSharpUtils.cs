using System.Numerics;
using SwfLib.Data;

namespace SwiffCheese.Exporting.ImageSharp;

internal static class ImageSharpUtils
{
    internal static Matrix3x2 SwfMatrixToMatrix3x2(SwfMatrix mat) => new(
        (float)mat.ScaleX, (float)mat.RotateSkew0,
        (float)mat.RotateSkew1, (float)mat.ScaleY,
        mat.TranslateX, mat.TranslateY
    );
}
