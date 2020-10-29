// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Regexator.Text;

namespace Regexator.Xml.Serialization.Projects.VersionLessThan1
{
    public class Input
    {
        public static IEnumerable<Regexator.Input> FromSerializable(IEnumerable<Input> inputs, string dirPath)
        {
            return FromSerializable(inputs, dirPath, InputKind.File);
        }

        public static IEnumerable<Regexator.Input> FromSerializable(
            IEnumerable<Input> inputs,
            string dirPath,
            InputKind kind)
        {
            foreach (Regexator.Input input in inputs.Select(f => FromSerializable(f, dirPath, kind)).Where(f => f != null))
                yield return input;
        }

        public static Regexator.Input FromSerializable(Input input, string dirPath, InputKind kind)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var item = new Regexator.Input()
            {
                Kind = kind,
                Options = input.ParseOptions(),
                NewLine = input.ParseNewLineMode(),
                CurrentLine = Math.Max(input.CurrentLine, 0),
                Encoding = input.ParseEncoding(),
                Attributes = input.ParseAttributes()
            };
            switch (kind)
            {
                case InputKind.File:
                    {
                        if (FileSystem.FileSystemUtility.TryCreateAbsolutePath(dirPath, input.FilePath, out string result))
                        {
                            item.Name = result;
                        }
                        else
                        {
                            item.Name = input.FilePath;
                            Debug.Fail("invalid file input name: " + input.FilePath);
                        }

                        break;
                    }
                case InputKind.Included:
                    if (IO.FileSystemUtility.IsValidFileName(input.FilePath))
                    {
                        item.Name = input.FilePath;
                        item.Text = input.Text.EnsureCarriageReturnLinefeed();
                        break;
                    }
                    else
                    {
                        Debug.Fail("invalid input name: " + input.FilePath);
                        return null;
                    }
            }

            return item;
        }

        public InputOptions ParseOptions()
        {
            if (Enum.TryParse(Options, out InputOptions value))
                return value;

            return EnumHelper.ParseInputOptions(Options);
        }

        public ItemAttributes ParseAttributes()
        {
            if (Enum.TryParse(Attributes, out ItemAttributes value))
                return value;

            return EnumHelper.ParseAttributes(Attributes);
        }

        public NewLineMode ParseNewLineMode()
        {
            if (Enum.TryParse(NewLine, out NewLineMode value))
                return value;

            return Regexator.Input.DefaultNewLine;
        }

        public Encoding ParseEncoding()
        {
            if (!string.IsNullOrEmpty(Encoding))
            {
                try
                {
                    return System.Text.Encoding.GetEncoding(Encoding);
                }
                catch (ArgumentException)
                {
                }
            }

            return Regexator.Input.DefaultEncoding;
        }

        [XmlElement("Header")]
        public ProjectInfo Properties { get; set; }

        public string FilePath { get; set; }
        public string Options { get; set; }
        public string Encoding { get; set; }
        public string NewLine { get; set; }
        public int CurrentLine { get; set; }
        public string Attributes { get; set; }

        [XmlIgnore]
        public string Text
        {
            get
            {
                return (_text != null)
                    ? PatternLibrary.FirstLastEmptyLine.Replace(XmlUtility.DecodeCDataEnd(_text), "")
                    : null;
            }
            set
            {
                _text = (value != null)
                    ? XmlUtility.EncodeCDataEnd(value).Enclose("\n")
                    : null;
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1059:MembersShouldNotExposeCertainConcreteTypes",
            MessageId = "System.Xml.XmlNode")]
        [XmlElement("Text")]
        public XmlNode TextCData
        {
            get { return (_text == null) ? null : new XmlDocument().CreateCDataSection(_text); }
            set { _text = (value == null) ? "" : value.Value; }
        }

        private string _text;
    }
}
