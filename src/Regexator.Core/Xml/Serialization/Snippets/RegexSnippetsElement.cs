// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Regexator.Xml.Serialization.Snippets
{
    [XmlRoot("RegexSnippets")]
    public class RegexSnippetsElement
    {
        public RegexSnippetsElement()
        {
        }

        public RegexSnippetsElement(RegexSnippet snippet)
            : this(new RegexSnippet[] { snippet })
        {
        }

        public RegexSnippetsElement(RegexSnippet[] snippets)
        {
            Snippets = snippets ?? throw new ArgumentNullException(nameof(snippets));
        }

        public IEnumerable<RegexSnippet> EnumerateSnippets()
        {
            if (Snippets != null)
            {
                foreach (RegexSnippet item in Snippets)
                    yield return item;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [XmlElement("RegexSnippet")]
        public RegexSnippet[] Snippets { get; set; }
    }
}
