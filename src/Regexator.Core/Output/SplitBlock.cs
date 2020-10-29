// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Regexator.Collections.Generic;
using Regexator.Text;
using Regexator.Text.RegularExpressions;

namespace Regexator.Output
{
    public sealed class SplitBlock : RegexBlock
    {
        internal SplitBlock(SplitItem item, int startIndex, SplitTextBuilder builder)
        {
            SplitItem = item ?? throw new ArgumentNullException(nameof(item));
            StartIndex = startIndex;
            _builder = builder;
            _infoText = builder.GetInfo(item);
            using (TextProcessor processor = builder.GetText(item, InfoLength))
            {
                FormattedValue = processor.ToString();
                Text = _infoText + processor.ToString(true);
                int offset = StartIndex + InfoLength;
                TextSpans = processor.TextSpans.Offset(offset).ToReadOnly();
                _symbolSpans = processor.SymbolSpans.Offset(offset).ToReadOnly();
            }
        }

        public override ReadOnlyCollection<TextSpan> TextSpans { get; }

        public override ReadOnlyCollection<InfoSelections> InfoSelections
        {
            get
            {
                return _infoSelections
                    ?? (_infoSelections = (new InfoSelections[] {
                        _builder.GetInfoSelections(StartIndex) })
                        .ToReadOnly());
            }
        }

        public int InfoLength
        {
            get { return _infoText.Length; }
        }

        public override int TextLength
        {
            get { return Text.Length; }
        }

        public override int StartIndex { get; }

        public override string Key
        {
            get { return SplitItem.Key; }
        }

        public override int MatchItemIndex
        {
            get { return SplitItem.ItemIndex; }
        }

        public override string GroupName
        {
            get { return (SplitItem.Kind == SplitItemKind.Group) ? SplitItem.GroupInfo.Name : null; }
        }

        public override int ValueIndex
        {
            get { return SplitItem.Index; }
        }

        public override int ValueLength
        {
            get { return SplitItem.Length; }
        }

        public override string Value
        {
            get { return SplitItem.Value; }
        }

        public override IEnumerable<TextSpan> SymbolTextSpans
        {
            get { return _symbolSpans; }
        }

        public override RegexBlockKind Kind
        {
            get { return (SplitItem.Kind == SplitItemKind.Split) ? RegexBlockKind.Split : RegexBlockKind.SplitGroup; }
        }

        public string FormattedValue { get; }

        public SplitItem SplitItem { get; }

        public string Text { get; }

        private readonly string _infoText;
        private readonly SplitTextBuilder _builder;
        private readonly ReadOnlyCollection<TextSpan> _symbolSpans;
        private ReadOnlyCollection<InfoSelections> _infoSelections;
    }
}
