// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Regexator.Collections.Generic;
using Regexator.Text;

namespace Regexator.Xml.Serialization.Projects
{
    public class Input
    {
        public static IEnumerable<Input> ToSerializable(IEnumerable<Regexator.Input> inputs, string projectPath)
        {
            return ToSerializable(inputs, projectPath, InputKind.File);
        }

        public static IEnumerable<Input> ToSerializable(
            IEnumerable<Regexator.Input> inputs,
            string projectPath,
            InputKind kind)
        {
            return inputs
                .OrderBy(f => f.Name)
                .Select(f => ToSerializable(f, projectPath, kind));
        }

        public static Input ToSerializable(Regexator.Input item, string projectPath, InputKind kind)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var input = new Input()
            {
                Options = SerializeOptions(item),
                NewLine = item.NewLine.ToString(),
                CurrentLine = item.CurrentLine
            };
            switch (kind)
            {
                case InputKind.File:
                    {
                        input.Encoding = (!item.HasDefaultEncoding) ? item.Encoding.WebName : null;

                        if (FileSystem.FileSystemUtility
                            .TryCreateRelativePath(projectPath, item.Name ?? "", out string relPath))
                        {
                            input.Name = relPath;
                        }
                        else
                        {
                            input.Name = item.Name;
                        }

                        break;
                    }
                case InputKind.Included:
                    {
                        input.Name = item.Name;
                        input.Text = item.Text;
                        break;
                    }
            }

            if (item.Attributes != ItemAttributes.None)
                input.Attributes = item.Attributes.ToString();

            return input;
        }

        private static string SerializeOptions(Regexator.Input input)
        {
            if (input.UnknownOptions.Count > 0)
            {
                if (input.Options == InputOptions.None)
                    return string.Join(", ", input.UnknownOptions);

                return input.Options.ToString() + ", " + string.Join(", ", input.UnknownOptions);
            }

            return input.Options.ToString();
        }

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

        public static Regexator.Input FromSerializable(Input item, string dirPath, InputKind kind)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var input = new Regexator.Input()
            {
                Kind = kind,
                NewLine = item.ParseNewLineMode(),
                CurrentLine = Math.Max(item.CurrentLine, 0),
                Encoding = item.ParseEncoding(),
                Attributes = item.ParseAttributes()
            };
            ParseOptions(item, input);
            switch (kind)
            {
                case InputKind.File:
                    {
                        if (FileSystem.FileSystemUtility.TryCreateAbsolutePath(dirPath, item.Name, out string result))
                        {
                            input.Name = result;
                        }
                        else
                        {
                            input.Name = item.Name;
                            Debug.Fail("invalid file input name: " + item.Name);
                        }

                        break;
                    }
                case InputKind.Included:
                    if (IO.FileSystemUtility.IsValidFileName(item.Name))
                    {
                        input.Name = item.Name;
                        input.Text = item.Text.EnsureCarriageReturnLinefeed();
                        break;
                    }
                    else
                    {
                        Debug.Fail("invalid input name: " + item.Name);
                        return null;
                    }
            }

            return input;
        }

        private static void ParseOptions(Input item, Regexator.Input input)
        {
            if (Enum.TryParse(item.Options, out InputOptions options))
            {
                input.Options = options;
            }
            else
            {
                var result = new EnumParseResult<InputOptions>();
                input.Options = result.ParseValues(item.Options).GetValue();
                input.UnknownOptions.AddItems(result.UnknownValues);
            }
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

        [XmlAttribute]
        public string Name { get; set; }

        public string Options { get; set; }
        public string NewLine { get; set; }
        public int CurrentLine { get; set; }
        public string Attributes { get; set; }
        public string Encoding { get; set; }

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
