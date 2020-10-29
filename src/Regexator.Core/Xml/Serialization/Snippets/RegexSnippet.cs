// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Regexator.Snippets;

namespace Regexator.Xml.Serialization.Snippets
{
    public class RegexSnippet
    {
        public static RegexSnippet ToSerializable(Regexator.Snippets.RegexSnippet item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new RegexSnippet()
            {
                Version = CreateVersion(item.Version),
                Format = CreateVersion(item.FormatVersion),
                Header = new HeaderElement(item),
                Snippet = SnippetElement.ToSerializable(item)
            };
        }

        public static Regexator.Snippets.RegexSnippet FromSerializable(RegexSnippet item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var snippet = new Regexator.Snippets.RegexSnippet(
                item.Header?.Category,
                item.Header?.Name,
                item.ParseCode(),
                item.ParseLiterals().ToArray())
            {
                Options = item.ParseRegexOptions(),
                Title = item.Header?.Title,
                Description = item.Header?.Description,
                Author = item.Header?.Author,
                HelpUrl = item.ParseUrl(),
                SnippetKinds = item.ParseSnippetKinds(),
                ExtendedInfo = item.ParseExtendedInfo()
            };

            if (System.Version.TryParse(item.Version, out Version version))
                snippet.Version = version;

            if (System.Version.TryParse(item.Format, out Version formatVersion))
                snippet.FormatVersion = formatVersion;

            foreach (string keyword in item.EnumerateKeywords())
                snippet.Keywords.Add(keyword);

            foreach (string engine in item.EnumerateEngines())
                snippet.Engines.Add(engine);

            return snippet;
        }

        public string ParseCode()
        {
            if (Snippet != null)
            {
                CodeElement code = Snippet.DefaultCode;
                if (code?.Text != null)
                    return XmlUtility.DecodeCDataEnd(code.Text);
            }

            return "";
        }

        public RegexOptions ParseRegexOptions()
        {
            if (Snippet?.DefaultCode != null && Enum.TryParse(Snippet.DefaultCode.Options, out RegexOptions options))
            {
                return options;
            }

            return RegexOptions.None;
        }

        public IEnumerable<Regexator.Snippets.SnippetLiteral> ParseLiterals()
        {
            if (Snippet?.Literals != null)
            {
                foreach (Regexator.Snippets.SnippetLiteral literal in Snippet.Literals
                    .Select(f => SnippetLiteral.FromSerializable(f)))
                {
                    yield return literal;
                }
            }
        }

        public Uri ParseUrl()
        {
            if (Header?.HelpUrl != null)
            {
                try
                {
                    return new Uri(Header.HelpUrl);
                }
                catch (UriFormatException)
                {
                }
            }

            return null;
        }

        public SnippetKinds ParseSnippetKinds()
        {
            var value = SnippetKinds.None;
            if (Header?.SnippetKinds != null)
            {
                foreach (string item in Header.SnippetKinds)
                {
                    if (Enum.TryParse(item, out SnippetKinds result))
                        value |= result;
                }
            }

            return value;
        }

        public ExtendedSnippetInfo ParseExtendedInfo()
        {
            if (Snippet?.Codes != null)
            {
                CodeElement code = Array.Find(Snippet.Codes, f => f.Kind != null && f.Kind != "Default");
                if (code != null)
                {
                    if (!Enum.TryParse(code.Kind, out SnippetCodeKind kind))
                        kind = SnippetCodeKind.None;

                    if (!Enum.TryParse(code.Options, out RegexOptions options))
                        options = RegexOptions.None;

                    return new ExtendedSnippetInfo(code.Text ?? "", options, kind);
                }
            }

            return null;
        }

        internal IEnumerable<string> EnumerateKeywords()
        {
            if (Header?.Keywords != null)
            {
                foreach (string item in Header.Keywords)
                    yield return item;
            }
        }

        internal IEnumerable<string> EnumerateEngines()
        {
            if (Header?.Engines != null)
            {
                foreach (string item in Header.Engines)
                    yield return item;
            }
        }

        private static string CreateVersion(Version version)
        {
            return new Version(
                Math.Max(0, version.Major),
                Math.Max(0, version.Minor),
                Math.Max(0, version.Build))
                .ToString(3);
        }

        [XmlAttribute]
        public string Version { get; set; }

        [XmlAttribute]
        public string Format { get; set; }

        public HeaderElement Header { get; set; }
        public SnippetElement Snippet { get; set; }
    }
}
