//-----------------------------------------------------------------------
// <copyright file="SvgReader.cs" company="Lifeprojects.de">
//     Class: SvgReader
//     Copyright © Lifeprojects.de 2023
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>06.10.2023</date>
//
// <summary>
// Die Klasse gehört zur Funktion "SvgToXaml",
// eine SVG-Vektor-Grafik für XAML nutzbar zu machen.
// </summary>
// <remark>
// Die Klasse wurde ursprünglich vom
// Copyright (C) 2009,2011 Boris Richter <himself@boris-richter.net>
// erstellt, und von mir für NET 7 überarbeitet und angepasst.
// </remark>
//-----------------------------------------------------------------------

namespace XamlIconViewer.SVG
{
    using System.IO;
    using System.Windows.Media;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    ///   Provides methods to read (and render) SVG documents.
    /// </summary>
    public static class SvgReader
    {

        /// <summary>
        ///   Loads an SVG document and renders it into a 
        ///   <see cref="DrawingImage"/>.
        /// </summary>
        /// <param name="reader">
        ///   A <see cref="XmlReader"/> to read the XML structure of the SVG 
        ///   document.
        /// </param>
        /// <param name="options">
        ///   <see cref="SvgReaderOptions"/> to use for parsing respectively 
        ///   rendering the SVG document.
        /// </param>
        /// <returns>
        ///   A <see cref="DrawingImage"/> containing the rendered SVG document.
        /// </returns>
        public static DrawingImage Load(XmlReader reader, SvgReaderOptions options)
        {
            if (options == null)
            {
                options = new SvgReaderOptions();
            }

            XDocument document = XDocument.Load(reader);
            if (document.Root.Name.NamespaceName != "http://www.w3.org/2000/svg")
            {
                throw new XmlException("Root element is not in namespace 'http://www.w3.org/2000/svg'.");
            }

            if (document.Root.Name.LocalName != "svg")
            {
                throw new XmlException("Root element is not an <svg> element.");
            }

            return new SvgDocument(document.Root, options).Draw();
        }

        /// <summary>
        ///   Loads an SVG document and renders it into a 
        ///   <see cref="DrawingImage"/>.
        /// </summary>
        /// <param name="reader">
        ///   A <see cref="XmlReader"/> to read the XML structure of the SVG 
        ///   document.
        /// </param>
        /// <returns>
        ///   A <see cref="DrawingImage"/> containing the rendered SVG document.
        /// </returns>
        public static DrawingImage Load(XmlReader reader)
        {
            return Load(reader, null);
        }

        /// <summary>
        ///   Loads an SVG document and renders it into a 
        ///   <see cref="DrawingImage"/>.
        /// </summary>
        /// <param name="stream">
        ///   A <see cref="Stream"/> to read the XML structure of the SVG 
        ///   document.
        /// </param>
        /// <param name="options">
        ///   <see cref="SvgReaderOptions"/> to use for parsing respectively 
        ///   rendering the SVG document.
        /// </param>
        /// <returns>
        ///   A <see cref="DrawingImage"/> containing the rendered SVG document.
        /// </returns>
        public static DrawingImage Load(Stream stream, SvgReaderOptions options)
        {
            using (XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
            {
                return Load(reader, options);
            }
        }

        /// <summary>
        ///   Loads an SVG document and renders it into a 
        ///   <see cref="DrawingImage"/>.
        /// </summary>
        /// <param name="stream">
        ///   A <see cref="Stream"/> to read the XML structure of the SVG 
        ///   document.
        /// </param>
        /// <returns>
        ///   A <see cref="DrawingImage"/> containing the rendered SVG document.
        /// </returns>
        public static DrawingImage Load(Stream stream)
        {
            return Load(stream, null);
        }
    }
}
