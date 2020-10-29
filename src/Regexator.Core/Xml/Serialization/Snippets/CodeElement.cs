// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Regexator.Snippets;
using Regexator.Text;

namespace Regexator.Xml.Serialization.Snippets
{
    public class CodeElement
    {
        public CodeElement()
        {
        }

        public CodeElement(string code, RegexOptions options)
            : this(code, options, SnippetCodeKind.None)
        {
        }

        public CodeElement(string code, RegexOptions options, SnippetCodeKind kind)
        {
            Text = code ?? throw new ArgumentNullException(nameof(code));

            if (options != RegexOptions.None)
                Options = RegexOptionsUtility.Format(options);

            if (kind != SnippetCodeKind.None)
                Kind = kind.ToString();
        }

        [XmlAttribute]
        public string Kind { get; set; }

        [XmlAttribute]
        public string Options { get; set; }

        [XmlIgnore]
        public string Text { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [XmlText]
        public XmlNode[] TextCData
        {
            get
            {
                if (Text != null)
                {
                    string data = XmlUtility.EncodeCDataEnd(Text).EnsureCarriageReturnLinefeed();
                    return new XmlNode[] { new XmlDocument().CreateCDataSection(data) };
                }

                return null;
            }
            set
            {
                Text = (value?.Length > 0)
                    ? XmlUtility.DecodeCDataEnd(value[0].Value)
                    : "";
            }
        }
    }
}
