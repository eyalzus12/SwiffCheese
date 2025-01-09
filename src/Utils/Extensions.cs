using System.Numerics;
using SwfLib.Data;

namespace SwiffCheese.Utils;

public static class SwfUtils
{
    public static Matrix3x2 SwfMatrixToMatrix3x2(SwfMatrix mat) => new(
        (float)mat.ScaleX, (float)mat.RotateSkew0,
        (float)mat.RotateSkew1, (float)mat.ScaleY,
        mat.TranslateX, mat.TranslateY
    );

    public static int GetWidth(SwfRect rect) => rect.XMax - rect.XMin;
    public static int GetHeight(SwfRect rect) => rect.YMax - rect.YMin;
}
