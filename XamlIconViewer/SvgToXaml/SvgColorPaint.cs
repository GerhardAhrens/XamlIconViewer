namespace XamlIconViewer.SVG
{
    using System.Windows.Media;

    /// <summary>
    ///   A paint with a solid color.
    /// </summary>
    internal sealed class SvgColorPaint : SvgPaint
    {
        public readonly SvgColor Color;

        public SvgColorPaint(SvgColor color)
        {
            Color = color;
        }

        public override Brush ToBrush(SvgBaseElement element)
        {
            return new SolidColorBrush(Color.ToColor());
        }
    }
}
