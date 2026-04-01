
namespace XamlIconViewer.SVG
{
    using System.Xml.Linq;

    /// <summary>
    ///   Represents a &lt;symbol&gt; element.
    /// </summary>
    internal sealed class SvgSymbolElement : SvgContainerBaseElement
    {
        public SvgSymbolElement(SvgDocument document, SvgBaseElement parent, XElement svgElement)
          : base(document, parent, svgElement)
        {
        }
    }
}
