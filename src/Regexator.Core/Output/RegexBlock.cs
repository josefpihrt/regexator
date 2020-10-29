// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Regexator.Text;

namespace Regexator.Output
{
    public abstract class RegexBlock
    {
        private TextSpan _span;
        private TextSpan _inputSpan;
        private TextSpan _valueSpan;
        private static readonly ReadOnlyCollection<TextSpan> _emptyTextSpans = Array.AsReadOnly(new TextSpan[] { });
        private static readonly ReadOnlyCollection<InfoSelections> _emptyInfos = Array.AsReadOnly(new InfoSelections[] { });

        public abstract int StartIndex { get; }
        public abstract int TextLength { get; }
        public abstract int MatchItemIndex { get; }
        public abstract int ValueIndex { get; }
        public abstract int ValueLength { get; }
        public abstract string Value { get; }
        public abstract RegexBlockKind Kind { get; }

        public virtual RegexBlock ValueBlock
        {
            get { return this; }
        }

        public TextSpan ValueSpan
        {
            get
            {
                if (_valueSpan == null)
                {
                    Debug.Assert(ValueIndex != -1);
                    if (ValueIndex != -1)
                        _valueSpan = new TextSpan(ValueIndex, ValueLength);
                }

                return _valueSpan;
            }
        }

        public TextSpan InputSpan
        {
            get { return _inputSpan ?? (_inputSpan = new TextSpan(ValueIndex, ValueLength)); }
            set { _inputSpan = value; }
        }

        public virtual ReadOnlyCollection<TextSpan> TextSpans
        {
            get { return _emptyTextSpans; }
        }

        public TextSpan TextSpan
        {
            get { return _span ?? (_span = new TextSpan(StartIndex, TextLength)); }
        }

        public virtual ReadOnlyCollection<InfoSelections> InfoSelections
        {
            get { return _emptyInfos; }
        }

        public virtual IEnumerable<TextSpan> SymbolTextSpans
        {
            get { yield break; }
        }

        public virtual IEnumerable<CaptureBlock> CaptureBlocks
        {
            get { yield break; }
        }

        public virtual string GroupName
        {
            get { return null; }
        }

        public virtual int CaptureItemIndex
        {
            get { return -1; }
        }

        public int EndIndex
        {
            get { return StartIndex + TextLength; }
        }

        public virtual string Key
        {
            get { return null; }
        }
    }
}