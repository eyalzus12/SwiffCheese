using SwfLib.Data;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public readonly struct BitmapFillStyle
{
    public SwfMatrix BitmapMatrix { get; }
    public bool Smoothing { get; }
    public BitmapMode Mode { get; }
    public ushort BitmapID { get; }

    public BitmapFillStyle(BitmapFillStyleRGB rgb)
    {
        BitmapMatrix = rgb.BitmapMatrix;
        Smoothing = rgb.Smoothing;
        Mode = rgb.Mode;
        BitmapID = rgb.BitmapID;
    }

    public BitmapFillStyle(BitmapFillStyleRGBA rgba)
    {
        BitmapMatrix = rgba.BitmapMatrix;
        Smoothing = rgba.Smoothing;
        Mode = rgba.Mode;
        BitmapID = rgba.BitmapID;
    }

    public static implicit operator BitmapFillStyle(BitmapFillStyleRGB rgb) => new(rgb);
    public static implicit operator BitmapFillStyle(BitmapFillStyleRGBA rgba) => new(rgba);
}