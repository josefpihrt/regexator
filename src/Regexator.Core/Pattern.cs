// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Regexator
{
    [Serializable]
    public class Pattern : ICloneable
    {
        public Pattern()
        {
            UnknownPatternOptions = new Collection<string>();
            Text = "";
        }

        [DebuggerStepThrough]
        public static ContainerProps GetChangedProps(Pattern first, Pattern second, ContainerProps props)
        {
            if (first == null || second == null || ReferenceEquals(first, second))
                return ContainerProps.None;

            var value = ContainerProps.None;

            if (props.Contains(ContainerProps.PatternCurrentLine) && first.CurrentLine != second.CurrentLine)
                value |= ContainerProps.PatternCurrentLine;

            if (props.Contains(ContainerProps.RegexOptions) && first.RegexOptions != second.RegexOptions)
                value |= ContainerProps.RegexOptions;

            if (props.Contains(ContainerProps.PatternOptions) && first.PatternOptions != second.PatternOptions)
                value |= ContainerProps.PatternOptions;

            if (props.Contains(ContainerProps.PatternText) && !string.Equals(first.Text, second.Text))
                value |= ContainerProps.PatternText;

            return value;
        }

        public bool HasOptions(PatternOptions options)
        {
            return (PatternOptions & options) == options;
        }

        public object Clone()
        {
            return new Pattern()
            {
                CurrentLine = CurrentLine,
                PatternOptions = PatternOptions,
                RegexOptions = RegexOptions,
                Text = Text
            };
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

        public Collection<string> UnknownPatternOptions { get; }
        public RegexOptions RegexOptions { get; set; }
        public PatternOptions PatternOptions { get; set; }

        public static readonly ContainerProps AllProps = ContainerProps.PatternCurrentLine
            | ContainerProps.PatternOptions
            | ContainerProps.PatternText
            | ContainerProps.RegexOptions;

        private string _text;
        private int _currentLine;
    }
}
