// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;

namespace Regexator.Xml.Serialization.Snippets
{
    [XmlRoot("Snippet")]
    public class SnippetElement
    {
        public static SnippetElement ToSerializable(Regexator.Snippets.RegexSnippet item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new SnippetElement()
            {
                Literals = (item.Literals.Count > 0)
                    ? item.Literals.Select(f => SnippetLiteral.ToSerializable(f)).ToArray()
                    : null,
                Codes = CreateCodes(item).ToArray()
            };
        }

        private static IEnumerable<CodeElement> CreateCodes(Regexator.Snippets.RegexSnippet item)
        {
            yield return new CodeElement(item.Code, item.Options);

            if (item.ExtendedInfo != null)
                yield return new CodeElement(item.ExtendedInfo.Code, item.ExtendedInfo.Options, item.ExtendedInfo.Kind);
        }

        internal CodeElement DefaultCode
        {
            get
            {
                if (Codes != null)
                    return Array.Find(Codes, f => f.Kind == null || f.Kind == "Default");

                return null;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [XmlArrayItem("Literal")]
        public SnippetLiteral[] Literals { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [XmlArrayItem("Code")]
        public CodeElement[] Codes { get; set; }
    }
}
