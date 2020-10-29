// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Regexator.Text;

namespace Regexator
{
    [Serializable]
    [DebuggerDisplay("Kind={Kind}, Name={Name}")]
    public class Input : ICloneable
    {
        [DebuggerStepThrough]
        public Input()
        {
            UnknownOptions = new Collection<string>();
            Text = "";
            Options = InputOptions.None;
            NewLine = DefaultNewLine;
            Kind = DefaultKind;
        }

        public static InputProps GetChangedProps(Input first, Input second, InputProps props)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));

            if (second == null)
                throw new ArgumentNullException(nameof(second));

            var value = InputProps.None;
            if (props.Contains(InputProps.CurrentLine) && first.CurrentLine != second.CurrentLine)
                value |= InputProps.CurrentLine;

            if (props.Contains(InputProps.Options) && first.Options != second.Options)
                value |= InputProps.Options;

            if (props.Contains(InputProps.Attributes) && first.Attributes != second.Attributes)
                value |= InputProps.Attributes;

            if (props.Contains(InputProps.NewLine) && first.NewLine != second.NewLine)
                value |= InputProps.NewLine;

            if (props.Contains(InputProps.Encoding) && !Equals(first.Encoding, second.Encoding))
                value |= InputProps.Encoding;

            if (props.Contains(InputProps.Name) && !FileSystem.FileSystemUtility.PathEquals(first.Name, second.Name))
                value |= InputProps.Name;

            if (props.Contains(InputProps.Text) && !string.Equals(first.Text, second.Text))
                value |= InputProps.Text;

            return value;
        }

        public void LoadText()
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (var sr = new StreamReader(stream, Encoding, true))
                {
                    stream = null;
                    Text = sr.ReadToEnd();
                }
            }
            finally
            {
                stream?.Dispose();
            }
        }

        public void SaveText()
        {
            SaveText(Text);
        }

        public void SaveText(string text)
        {
            using (var sw = new StreamWriter(Name, false, Encoding))
            {
                sw.Write(text);
                Text = text;
            }
        }

        public object Clone()
        {
            return new Input()
            {
                Kind = Kind,
                Options = Options,
                NewLine = NewLine,
                CurrentLine = CurrentLine,
                Text = Text,
                Name = Name,
                Encoding = Encoding,
                Attributes = Attributes
            };
        }

        public bool HasOptions(InputOptions options)
        {
            return (Options & options) == options;
        }

        public bool HasAttributes(ItemAttributes attributes)
        {
            return Attributes.Contains(attributes);
        }

        public void AddAttributes(ItemAttributes attributes)
        {
            Attributes = Attributes.Union(attributes);
        }

        public void RemoveAttributes(ItemAttributes attributes)
        {
            Attributes = Attributes.Except(attributes);
        }

        public static bool NameEquals(string name1, string name2)
        {
            return string.Equals(name1 ?? "", name2 ?? "", StringComparison.CurrentCulture);
        }

        private static InputProps CreateAllProps()
        {
            var value = InputProps.None;

            foreach (InputProps item in Enum.GetValues(typeof(InputProps)).Cast<InputProps>())
                value |= item;

            return value;
        }

        public static InputProps AllProps
        {
            get
            {
                if (_allProps == InputProps.None)
                    _allProps = CreateAllProps();

                return _allProps;
            }
        }

        public string Text
        {
            get { return _text ?? ""; }
            set { _text = value; }
        }

        public int CurrentLine
        {
            get { return _currentLine; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                _currentLine = value;
            }
        }

        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value ?? throw new ArgumentNullException("value"); }
        }

        public bool HasDefaultEncoding
        {
            get { return Equals(Encoding, DefaultEncoding); }
        }

        public static Encoding DefaultEncoding
        {
            get { return Encoding.UTF8; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value ?? ""; }
        }

        public Collection<string> UnknownOptions { get; }
        public InputOptions Options { get; set; }
        public ItemAttributes Attributes { get; set; }
        public NewLineMode NewLine { get; set; }
        public InputKind Kind { get; set; }

        private string _text;
        private static InputProps _allProps;
        private int _currentLine;
        private Encoding _encoding = DefaultEncoding;
        private string _name = "";

        public static readonly InputProps AutoSaveProps = InputProps.CurrentLine;
        public static readonly NewLineMode DefaultNewLine;
        public static readonly InputKind DefaultKind = InputKind.Included;
    }
}
