// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Regexator.Text;
using Regexator.Text.RegularExpressions;

namespace Regexator.Output
{
    public sealed class ReplaceBlock : RegexBlock
    {
        internal ReplaceBlock(ReplaceItem item, TextSpan textSpan, IList<TextSpan> symbols)
        {
            ReplaceItem = item;
            _span = textSpan;
            TextSpans = Array.AsReadOnly(new TextSpan[] { textSpan });
            _symbolSpans = new ReadOnlyCollection<TextSpan>(symbols);
        }

        public override int StartIndex
        {
            get { return _span.Index; }
        }

        public override int TextLength
        {
            get { return _span.Length; }
        }

        public override string Key
        {
            get { return ReplaceItem.Key; }
        }

        public Match Match
        {
            get { return ReplaceItem.Match; }
        }

        public override ReadOnlyCollection<TextSpan> TextSpans { get; }

        public override int MatchItemIndex
        {
            get { return ReplaceItem.ItemIndex; }
        }

        public override int ValueIndex
        {
            get { return ReplaceItem.Match.Index; }
        }

        public override int ValueLength
        {
            get { return ReplaceItem.Match.Length; }
        }

        public override string Value
        {
            get { return ReplaceItem.Match.Value; }
        }

        public ReplaceResult Result
        {
            get { return ReplaceItem.Result; }
        }

        public override IEnumerable<TextSpan> SymbolTextSpans
        {
            get { return _symbolSpans; }
        }

        public override RegexBlockKind Kind
        {
            get { return RegexBlockKind.Replace; }
        }

        public ReplaceItem ReplaceItem { get; }

        private readonly TextSpan _span;
        private readonly ReadOnlyCollection<TextSpan> _symbolSpans;
    }
}
