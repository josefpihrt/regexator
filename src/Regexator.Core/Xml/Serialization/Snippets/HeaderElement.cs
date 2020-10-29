// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;

namespace Regexator.Xml.Serialization.Snippets
{
    [XmlRoot("Header")]
    public class HeaderElement
    {
        public HeaderElement()
        {
        }

        public HeaderElement(Regexator.Snippets.RegexSnippet item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            Category = item.Category;
            Name = item.Name;
            Title = item.Title;
            Description = item.Description;
            Author = item.Author;

            if (item.HelpUrl != null)
                HelpUrl = item.HelpUrl.ToString();

            SnippetKinds = item.SnippetKinds.ToString()
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(f => f.Trim())
                .ToArray();

            Keywords = (item.Keywords.Count > 0) ? item.Keywords.ToArray() : null;
            Engines = (item.Engines.Count > 0) ? item.Engines.ToArray() : null;
        }

        public string Category { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Shortcut { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string HelpUrl { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [XmlArrayItem("SnippetKind")]
        public string[] SnippetKinds { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [XmlArrayItem("Keywords")]
        public string[] Keywords { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [XmlArrayItem("Engine")]
        public string[] Engines { get; set; }
    }
}
