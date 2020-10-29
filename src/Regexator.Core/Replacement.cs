// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Regexator.Text;

namespace Regexator
{
    [Serializable]
    public class Replacement : ICloneable
    {
        public Replacement()
        {
            UnknownOptions = new Collection<string>();
            Text = "";
        }

        public Replacement(string text, ReplacementOptions options, NewLineMode newLine, int currentLine)
        {
            UnknownOptions = new Collection<string>();
            Text = text;
            Options = options;
            NewLine = newLine;
            CurrentLine = currentLine;
        }

        [DebuggerStepThrough]
        public static ContainerProps GetChangedProps(Replacement first, Replacement second, ContainerProps props)
        {
            if (first == null || second == null || ReferenceEquals(first, second))
                return ContainerProps.None;

            var value = ContainerProps.None;

            if (props.Contains(ContainerProps.ReplacementCurrentLine) && first.CurrentLine != second.CurrentLine)
                value |= ContainerProps.ReplacementCurrentLine;

            if (props.Contains(ContainerProps.ReplacementText) && !string.Equals(first.Text, second.Text))
                value |= ContainerProps.ReplacementText;

            if (props.Contains(ContainerProps.ReplacementNewLine) && first.NewLine != second.NewLine)
                value |= ContainerProps.ReplacementNewLine;

            if (props.Contains(ContainerProps.ReplacementOptions) && first.Options != second.Options)
                value |= ContainerProps.ReplacementOptions;

            return value;
        }

        public bool HasOptions(ReplacementOptions options)
        {
            return (Options & options) == options;
        }

        public object Clone()
        {
            return new Replacement(Text, Options, NewLine, CurrentLine);
        }

        public string Text
        {
            get { return _text; }
            set { _text = value ?? ""; }
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

        public Collection<string> UnknownOptions { get; }
        public ReplacementOptions Options { get; set; }
        public NewLineMode NewLine { get; set; }

        public static readonly ContainerProps AllProps = ContainerProps.ReplacementCurrentLine
            | ContainerProps.ReplacementNewLine
            | ContainerProps.ReplacementOptions
            | ContainerProps.ReplacementText;

        private string _text;
        private int _currentLine;
    }
}
