// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Regexator.Collections.Generic;
using Regexator.Text;

namespace Regexator.Xml.Serialization.Projects
{
    public class Pattern
    {
        public static Pattern ToSerializable(Regexator.Pattern pattern)
        {
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            return new Pattern()
            {
                RegexOptions = pattern.RegexOptions.ToString(),
                PatternOptions = SerializeOptions(pattern),
                CurrentLine = pattern.CurrentLine,
                Text = string.Concat("\n", XmlUtility.EncodeCDataEnd(pattern.Text), "\n")
            };
        }

        private static string SerializeOptions(Regexator.Pattern item)
        {
            if (item.UnknownPatternOptions.Count > 0)
            {
                if (item.PatternOptions == Regexator.PatternOptions.None)
                    return string.Join(", ", item.UnknownPatternOptions);

                return item.PatternOptions.ToString() + ", " + string.Join(", ", item.UnknownPatternOptions);
            }

            return item.PatternOptions.ToString();
        }

        public static Regexator.Pattern FromSerializable(Pattern item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var pattern = new Regexator.Pattern()
            {
                RegexOptions = item.ParseRegexOptions(),
                CurrentLine = Math.Max(item.CurrentLine, 0),
                Text = (item.Text != null) ? RegexLibrary.FirstLastEmptyLine
                    .Replace(XmlUtility.DecodeCDataEnd(item.Text), "")
                    .EnsureCarriageReturnLinefeed() : null
            };
            ParsePatternOptions(item, pattern);
            return pattern;
        }

        private static void ParsePatternOptions(Pattern item, Regexator.Pattern pattern)
        {
            if (Enum.TryParse(item.PatternOptions, out PatternOptions options))
            {
                pattern.PatternOptions = options;
            }
            else
            {
                var result = new EnumParseResult<PatternOptions>();
                pattern.PatternOptions = result.ParseValues(item.PatternOptions).GetValue();
                pattern.UnknownPatternOptions.AddItems(result.UnknownValues);
            }
        }

        public RegexOptions ParseRegexOptions()
        {
            if (Enum.TryParse(RegexOptions, out RegexOptions value))
                return value;

            return System.Text.RegularExpressions.RegexOptions.None;
        }

        public string RegexOptions { get; set; }

        [XmlElement("Options")]
        public string PatternOptions { get; set; }

        public int CurrentLine { get; set; }

        [XmlIgnore]
        public string Text { get; set; }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1059:MembersShouldNotExposeCertainConcreteTypes",
            MessageId = "System.Xml.XmlNode")]
        [XmlElement("Text")]
        public XmlNode TextCData
        {
            get { return (Text == null) ? null : new XmlDocument().CreateCDataSection(Text); }
            set { Text = (value == null) ? "" : value.Value; }
        }
    }
}
