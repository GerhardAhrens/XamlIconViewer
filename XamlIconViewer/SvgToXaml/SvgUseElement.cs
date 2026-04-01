//-----------------------------------------------------------------------
// <copyright file="SvgUseElement.cs" company="Lifeprojects.de">
//     Class: SvgUseElement
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
    using System;
    using System.Xml.Linq;

    /// <summary>
    ///   Represents an &lt;use&gt; element.
    /// </summary>
    internal sealed class SvgUseElement : SvgBaseElement
    {
        public SvgUseElement(SvgDocument document, SvgBaseElement parent, XElement useElement)
          : base(document, parent, useElement)
        {
        }

        public SvgBaseElement GetElement()
        {
            if (Reference == null)
            {
                throw new NotSupportedException();
            }

            if (Document.Elements.TryGetValue(Reference, out var element))
            {
                return element;
            }
            else
            {
                return null;
            }
        }

    }
}
