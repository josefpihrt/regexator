// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator.Text
{
    public sealed class Captions : ICloneable
    {
        private string _match;
        private string _group;
        private string _capture;
        private string _captures;
        private string _index;
        private string _length;
        private string _value;
        private string _result;
        private string _split;

        public const string DefaultMatch = "Match";
        public const string DefaultGroup = "Group";
        public const string DefaultCapture = "Capture";
        public const string DefaultCaptures = "Captures";
        public const string DefaultIndex = "Index";
        public const string DefaultLength = "Length";
        public const string DefaultValue = "Value";
        public const string DefaultResult = "Result";
        public const string DefaultSplit = "Split";

        public const char DefaultMatchChar = 'M';
        public const char DefaultGroupChar = 'G';
        public const char DefaultCaptureChar = 'C';
        public const char DefaultIndexChar = 'I';
        public const char DefaultLengthChar = 'L';
        public const char DefaultValueChar = 'V';
        public const char DefaultResultChar = 'R';
        public const char DefaultSplitChar = 'S';

        public Captions()
        {
            Match = DefaultMatch;
            Group = DefaultGroup;
            Capture = DefaultCapture;
            Captures = DefaultCaptures;
            Index = DefaultIndex;
            Length = DefaultLength;
            Value = DefaultValue;
            Result = DefaultResult;
            Split = DefaultSplit;

            CaptureChar = DefaultCaptureChar;
            GroupChar = DefaultGroupChar;
            IndexChar = DefaultIndexChar;
            LengthChar = DefaultLengthChar;
            MatchChar = DefaultMatchChar;
            ResultChar = DefaultResultChar;
            SplitChar = DefaultSplitChar;
            ValueChar = DefaultValueChar;
        }

        public object Clone()
        {
            return new Captions()
            {
                Match = Match,
                Group = Group,
                Capture = Capture,
                Captures = Captures,
                Index = Index,
                Length = Length,
                Value = Value,
                Result = Result,
                Split = Split,

                MatchChar = MatchChar,
                GroupChar = GroupChar,
                CaptureChar = CaptureChar,
                IndexChar = IndexChar,
                LengthChar = LengthChar,
                ValueChar = ValueChar,
                ResultChar = ResultChar,
                SplitChar = SplitChar
            };
        }

        public string Match
        {
            get { return _match; }
            set { _match = value ?? DefaultMatch; }
        }

        public string Group
        {
            get { return _group; }
            set { _group = value ?? DefaultGroup; }
        }

        public string Capture
        {
            get { return _capture; }
            set { _capture = value ?? DefaultCapture; }
        }

        public string Captures
        {
            get { return _captures; }
            set { _captures = value ?? DefaultCaptures; }
        }

        public string Index
        {
            get { return _index; }
            set { _index = value ?? DefaultIndex; }
        }

        public string Length
        {
            get { return _length; }
            set { _length = value ?? DefaultLength; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value ?? DefaultValue; }
        }

        public string Result
        {
            get { return _result; }
            set { _result = value ?? DefaultResult; }
        }

        public string Split
        {
            get { return _split; }
            set { _split = value ?? DefaultSplit; }
        }

        public char MatchChar { get; set; }

        public char GroupChar { get; set; }

        public char CaptureChar { get; set; }

        public char IndexChar { get; set; }

        public char LengthChar { get; set; }

        public char ValueChar { get; set; }

        public char ResultChar { get; set; }

        public char SplitChar { get; set; }

        internal const int IndexCharLength = 1;
        internal const int LengthCharLength = 1;
        internal const int MatchCharLength = 1;
        internal const int GroupCharLength = 1;
        internal const int CaptureCharLength = 1;
        internal const int SplitCharLength = 1;
    }
}
