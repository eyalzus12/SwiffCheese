using SwfLib.Shapes.LineStyles;

namespace SwiffCheese.Wrappers;

public readonly struct LineStyle
{
    public ushort Width { get; }
    public SwfColor Color { get; }
    public CapStyle StartCapStyle { get; }
    public JoinStyle JoinStyle { get; }
    public bool NoHScale { get; }
    public bool NoVScale { get; }
    public bool PixelHinting { get; }
    public bool NoClose { get; }
    public CapStyle EndCapStyle { get; }
    public double MilterLimitFactor { get; }
    public FillStyle? FillStyle { get; }

    public LineStyle(LineStyleRGB rgb)
    {
        Width = rgb.Width;
        Color = rgb.Color;
    }

    public LineStyle(LineStyleRGBA rgba)
    {
        Width = rgba.Width;
        Color = rgba.Color;
    }

    public LineStyle(LineStyleEx ex)
    {
        Width = ex.Width;
        Color = ex.Color;
        StartCapStyle = ex.StartCapStyle;
        JoinStyle = ex.JoinStyle;
        NoHScale = ex.NoHScale;
        NoVScale = ex.NoVScale;
        PixelHinting = ex.PixelHinting;
        NoClose = ex.NoClose;
        EndCapStyle = ex.EndCapStyle;
        MilterLimitFactor = ex.MilterLimitFactor;
        FillStyle = ex.FillStyle;
    }

    public static LineStyle New(LineStyleRGB rgb) => new(rgb);
    public static LineStyle New(LineStyleRGBA rgba) => new(rgba);
    public static LineStyle New(LineStyleEx ex) => new(ex);

    public static implicit operator LineStyle(LineStyleRGB rgb) => new(rgb);
    public static implicit operator LineStyle(LineStyleRGBA rgba) => new(rgba);
    public static implicit operator LineStyle(LineStyleEx ex) => new(ex);
}