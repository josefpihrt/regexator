// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Regexator.Collections.Generic;
using Regexator.Text;
using Regexator.Text.RegularExpressions;

namespace Regexator.Output
{
    public sealed class CaptureBlock : RegexBlock
    {
        internal CaptureBlock(CaptureBlockIterator iterator)
        {
            CaptureItem = iterator.CaptureItem;
            CaptureId = iterator.CaptureId;
            StartIndex = iterator.StartIndex;
            InfoText = iterator.GetInfoText();
            using (var processor = new TextProcessor(CaptureItem.Value, iterator.Settings, InfoText.Length))
            {
                processor.ReadToEnd();
                FormattedValue = processor.ToString();
                Text = InfoText + processor.ToString(true);
                TextSpans = processor.TextSpans.Offset(StartIndex + InfoLength).ToReadOnly();
                _symbolTextSpans = processor.SymbolSpans.Offset(StartIndex + InfoLength).ToReadOnly();
            }

            InfoSelections = new ReadOnlyCollection<InfoSelections>(new CaptureInfoSelections[] { iterator.InfoSelections });
        }

        public override IEnumerable<CaptureBlock> CaptureBlocks
        {
            get { yield return this; }
        }

        public override ReadOnlyCollection<TextSpan> TextSpans { get; }

        public override IEnumerable<TextSpan> SymbolTextSpans
        {
            get { return _symbolTextSpans; }
        }

        public override ReadOnlyCollection<InfoSelections> InfoSelections { get; }

        public int InfoLength
        {
            get { return InfoText.Length; }
        }

        public string Text { get; }

        public override int TextLength
        {
            get { return Text.Length; }
        }

        public override int StartIndex { get; }

        public override string Key
        {
            get { return CaptureItem.Key; }
        }

        public override int MatchItemIndex
        {
            get { return CaptureItem.MatchItem.ItemIndex; }
        }

        public override string GroupName
        {
            get { return GroupItem.Name; }
        }

        public override int CaptureItemIndex
        {
            get { return CaptureItem.ItemIndex; }
        }

        public override int ValueIndex
        {
            get { return CaptureItem.Index; }
        }

        public override int ValueLength
        {
            get { return CaptureItem.Length; }
        }

        public override string Value
        {
            get { return CaptureItem.Value; }
        }

        public MatchItem MatchItem
        {
            get { return CaptureItem.MatchItem; }
        }

        public GroupItem GroupItem
        {
            get { return CaptureItem.GroupItem; }
        }

        public bool IsDefaultCapture
        {
            get { return CaptureItem.IsDefaultCapture; }
        }

        public override RegexBlockKind Kind
        {
            get { return RegexBlockKind.Capture; }
        }

        public CaptureItem CaptureItem { get; }

        public int CaptureId { get; }

        public string InfoText { get; }

        public string FormattedValue { get; }

        private readonly ReadOnlyCollection<TextSpan> _symbolTextSpans;
    }
}
