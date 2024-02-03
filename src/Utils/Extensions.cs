using SixLabors.ImageSharp;
using SwfLib.Data;
using SwiffCheese.Wrappers;

namespace SwiffCheese.Utils;

public static class Extensions
{
    public static Color SwfColorToImageSharpColor(this SwfColor color)
    {
        return Color.FromRgba(color.Red, color.Green, color.Blue, color.Alpha);
    }
    
    public static int Width(this SwfRect rect) => rect.XMax - rect.XMin;
    public static int Height(this SwfRect rect) => rect.YMax - rect.YMin;
}
