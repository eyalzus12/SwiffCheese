namespace SwiffCheese.Exporting.Svg;

public readonly record struct SvgMatrix(
    double ScaleX, double RotateSkew0,
    double RotateSkew1, double ScaleY,
    double TranslateX, double TranslateY
)
{
    public SvgMatrix() : this(1, 0, 0, 1, 0, 0) { }
}
