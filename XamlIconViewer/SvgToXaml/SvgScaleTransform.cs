namespace XamlIconViewer.SVG
{
    using System;
    using System.Globalization;
    using System.Windows.Media;

    internal sealed class SvgScaleTransform : SvgTransform
    {
        // Wiederverwendbares statisches Array vermeiden wiederholte Allokationen
        private static readonly char[] s_splitChars = new[] { ' ', '\t', ',' };

        public readonly double X;
        public readonly double Y;

        public SvgScaleTransform(double x, double y)
        {
            X = x;
            Y = y;
        }

        public SvgScaleTransform(double scale)
        {
            X = scale;
            Y = scale;
        }

        public override Transform ToTransform()
        {
            return new ScaleTransform(X, Y);
        }

        public static new SvgScaleTransform Parse(string transform)
        {
            string[] tokens = transform.Split(s_splitChars, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length == 1)
                return new SvgScaleTransform(Double.Parse(tokens[0].Trim(), CultureInfo.InvariantCulture.NumberFormat));

            if (tokens.Length == 2)
                return new SvgScaleTransform(Double.Parse(tokens[0].Trim(), CultureInfo.InvariantCulture.NumberFormat),
                                             Double.Parse(tokens[1].Trim(), CultureInfo.InvariantCulture.NumberFormat));

            throw new NotSupportedException();
        }
    }
}
