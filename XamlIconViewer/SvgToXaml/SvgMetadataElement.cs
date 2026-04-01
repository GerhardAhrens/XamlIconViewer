
namespace XamlIconViewer.SVG
{
    using System.Xml.Linq;

    /// <summary>
    ///   Represents a &lt;metadata&gt; element.
    /// </summary>
    internal sealed class SvgMetadataElement : SvgBaseElement
    {
        public SvgMetadataElement(SvgDocument document, SvgBaseElement parent, XElement metadataElement)
          : base(document, parent, metadataElement)
        {
        }
    }
}
