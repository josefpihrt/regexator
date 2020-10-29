// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;
using Regexator.Text;

namespace Regexator.Xml.Serialization.Projects.VersionLessThan1
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
                Options = item.Options.ToString(),
                NewLine = item.NewLine.ToString(),
                CurrentLine = item.CurrentLine
            };
        }

        public static Regexator.Replacement FromSerializable(Replacement item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new Regexator.Replacement()
            {
                Options = item.ParseOptions(),
                NewLine = item.ParseNewLineMode(),
                CurrentLine = Math.Max(item.CurrentLine, 0),
                Text = (item.Text != null)
                    ? PatternLibrary.FirstLastEmptyLine.Replace(XmlUtility.DecodeCDataEnd(item.Text), "")
                    : null
            };
        }

        public NewLineMode ParseNewLineMode()
        {
            if (Enum.TryParse(NewLine, out NewLineMode value))
                return value;

            return NewLineMode.Lf;
        }

        public ReplacementOptions ParseOptions()
        {
            if (Enum.TryParse(Options, out ReplacementOptions value))
                return value;

            return EnumHelper.ParseReplacementOptions(Options);
        }

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

        public string NewLine { get; set; }
        public string Options { get; set; }
        public int CurrentLine { get; set; }
    }
}
