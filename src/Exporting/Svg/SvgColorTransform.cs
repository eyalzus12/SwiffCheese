namespace SwiffCheese.Exporting.Svg;

public readonly record struct SvgColorTransform(
    double RedMult, double RedAdd,
    double GreenMult, double GreenAdd,
    double BlueMult, double BlueAdd,
    double AlphaMult, double AlphaAdd
)
{
    public SvgColorTransform() : this(1, 0, 1, 0, 1, 0, 1, 0) { }
}
