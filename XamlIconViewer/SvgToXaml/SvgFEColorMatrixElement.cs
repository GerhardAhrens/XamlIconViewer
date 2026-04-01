namespace XamlIconViewer.SVG
{
    using System.Diagnostics;
    using System.Windows.Media.Effects;
    using System.Xml.Linq;

    /// <summary>
    ///   Represents an &lt;feColorMatrix&gt; element.
    /// </summary>
    internal sealed class SvgFEColorMatrixElement : SvgFilterEffectBaseElement
    {

        public SvgFEColorMatrixElement(SvgDocument document, SvgBaseElement parent, XElement feColorMatrixElement)
          : base(document, parent, feColorMatrixElement)
        {
            // ...
        }

        public override BitmapEffect ToBitmapEffect()
        {
            Debug.WriteLine("feColorMatrix wird aktuelle nicht unterstützt!");
            return null;
        }
    }
}
