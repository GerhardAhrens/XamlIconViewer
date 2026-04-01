
namespace XamlIconViewer.SVG
{
    using System.Xml.Linq;

    /// <summary>
    ///   Represents a &lt;marker&gt; element.
    /// </summary>
    internal sealed class SvgMarkerElement : SvgContainerBaseElement
    {
        public SvgMarkerElement(SvgDocument document, SvgBaseElement parent, XElement markerElement)
          : base(document, parent, markerElement)
        {
        }
    }
}
