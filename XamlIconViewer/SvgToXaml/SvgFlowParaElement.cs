namespace XamlIconViewer.SVG
{
    using System.Xml.Linq;

    /// <summary>
    ///   Represents a &lt;flowPara&gt; element.
    /// </summary>
    internal sealed class SvgFlowParaElement : SvgDrawableContainerBaseElement
    {

        public SvgFlowParaElement(SvgDocument document, SvgBaseElement parent, XElement flowParaElement)
          : base(document, parent, flowParaElement)
        {
        }
    }
}
