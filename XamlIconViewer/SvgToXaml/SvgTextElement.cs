
namespace XamlIconViewer.SVG
{
    using System.Xml.Linq;

    /// <summary>
    ///   Represents a &lt;text&gt; element.
    /// </summary>
    internal sealed class SvgTextElement : SvgDrawableContainerBaseElement
    {

        public SvgTextElement(SvgDocument document, SvgBaseElement parent, XElement svgElement)
          : base(document, parent, svgElement)
        {
        }
    }
}
