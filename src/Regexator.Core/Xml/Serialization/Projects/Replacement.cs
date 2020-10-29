// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;
using Regexator.Collections.Generic;
using Regexator.Text;

namespace Regexator.Xml.Serialization.Projects
{
    public class Replacement
    {
        public static Replacement ToSerializable(Regexator.Replacement item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new Replacement()
            {
                Text = string.Concat("\n", XmlUtility.EncodeCDataEnd(item.Text), "\n"),
                Options = SerializeOptions(item),
                NewLine = item.NewLine.ToString(),
                CurrentLine = item.CurrentLine
            };
        }

        private static string SerializeOptions(Regexator.Replacement input)
        {
            if (input.UnknownOptions.Count > 0)
            {
                if (input.Options == ReplacementOptions.None)
                    return string.Join(", ", input.UnknownOptions);

                return input.Options.ToString() + ", " + string.Join(", ", input.UnknownOptions);
            }

            return input.Options.ToString();
        }

        public static Regexator.Replacement FromSerializable(Replacement item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var replacement = new Regexator.Replacement()
            {
                NewLine = item.ParseNewLineMode(),
                CurrentLine = Math.Max(item.CurrentLine, 0),
                Text = (item.Text != null)
                    ? PatternLibrary.FirstLastEmptyLine.Replace(XmlUtility.DecodeCDataEnd(item.Text), "")
                    : null
            };
            ParseOptions(item, replacement);
            return replacement;
        }

        private static void ParseOptions(Replacement item, Regexator.Replacement replacement)
        {
            if (Enum.TryParse(item.Options, out ReplacementOptions options))
            {
                replacement.Options = options;
            }
            else
            {
                var result = new EnumParseResult<ReplacementOptions>();
                replacement.Options = result.ParseValues(item.Options).GetValue();
                replacement.UnknownOptions.AddItems(result.UnknownValues);
            }
        }

        public NewLineMode ParseNewLineMode()
        {
            if (Enum.TryParse(NewLine, out NewLineMode value))
                return value;

            return NewLineMode.Lf;
        }

        public string Options { get; set; }
        public string NewLine { get; set; }
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
