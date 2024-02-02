using Microsoft.Xna.Framework;
using SwiffCheese.Wrappers;

namespace SwiffCheese;

public static class Extensions
{
    public static Color SwfColorToXnaColor(this SwfColor color)
    {
        return new(color.Red, color.Green, color.Blue, color.Alpha);
    }
}
