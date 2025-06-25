using OneOf;
using SwfLib.Data;
using SwfLib.Shapes.FillStyles;

namespace SwiffCheese.Wrappers;

public readonly struct BitmapFillStyle
{
    public OneOf<BitmapFillStyleRGB, BitmapFillStyleRGBA> Internal { get; }

    public BitmapFillStyle(BitmapFillStyleRGB rgb) => Internal = rgb;
    public static implicit operator BitmapFillStyle(BitmapFillStyleRGB rgb) => new(rgb);
    public BitmapFillStyle(BitmapFillStyleRGBA rgba) => Internal = rgba;
    public static implicit operator BitmapFillStyle(BitmapFillStyleRGBA rgba) => new(rgba);

    public SwfMatrix BitmapMatrix => Internal.Match(
        rgb => rgb.BitmapMatrix,
        rgba => rgba.BitmapMatrix
    );

    public bool Smoothing => Internal.Match(
        rgb => rgb.Smoothing,
        rgba => rgba.Smoothing
    );

    public BitmapMode Mode => Internal.Match(
        rgb => rgb.Mode,
        rgba => rgba.Mode
    );

    public ushort BitmapID => Internal.Match(
        rgb => rgb.BitmapID,
        rgba => rgba.BitmapID
    );
}