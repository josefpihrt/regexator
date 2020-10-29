// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Regexator.Text;
using Regexator.Text.RegularExpressions;

namespace Regexator.Output
{
    internal class SplitTextBuilder
    {
        private const string Space = " ";

        private readonly OutputSettings _settings;
        private int _splitPadding = -1;
        private int _indexPadding = -1;
        private int _lengthPadding = -1;

        public SplitTextBuilder(SplitItemCollection items)
            : this(items, new OutputSettings())
        {
        }

        public SplitTextBuilder(SplitItemCollection items, OutputSettings settings)
        {
            SplitItems = items ?? throw new ArgumentNullException(nameof(items));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public string GetInfo(SplitItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_settings.AddInfo)
            {
                return ((item.Kind == SplitItemKind.Group) ? Captions.GroupChar : Captions.SplitChar)
                    + PrefixSeparator
                    + item.Name.PadRight(SplitPadding)
                    + Space +
                    Captions.IndexChar
                    + PrefixSeparator
                    + OutputFormatter.PadNumber(item.Index, IndexPadding, _settings.NumberAlignment)
                    + Space +
                    Captions.LengthChar
                    + PrefixSeparator
                    + OutputFormatter.PadNumber(item.Length, LengthPadding, _settings.NumberAlignment)
                    + Space;
            }

            return "";
        }

        public SplitBlock[] GetBlocks()
        {
            return new SplitBlockIterator(this).ToArray();
        }

        public TextProcessor GetText(SplitItem item, int indent)
        {
            var text = new TextProcessor(item.Value, _settings, indent);
            text.ReadToEnd();
            return text;
        }

        public InfoSelections GetInfoSelections(int startIndex)
        {
            TextSpan[] arr = ((_settings.AddInfo) ? EnumerateInfoSpans(startIndex) : Enumerable.Repeat(TextSpan.Empty, 3))
                .ToArray();
            return new InfoSelections(arr[0], arr[1], arr[2]);
        }

        private IEnumerable<TextSpan> EnumerateInfoSpans(int startIndex)
        {
            int index = startIndex;
            foreach (int length in EnumerateLengths())
            {
                var ts = new TextSpan(index, length + PrefixSeparator.Length);
                index = ts.EndIndex + 1;
                yield return ts;
            }
        }

        private IEnumerable<int> EnumerateLengths()
        {
            yield return SplitPadding + Captions.SplitCharLength;
            yield return IndexPadding + Captions.IndexCharLength;
            yield return LengthPadding + Captions.LengthCharLength;
        }

        public int SplitPadding
        {
            get
            {
                if (_splitPadding == -1)
                {
                    _splitPadding = Math.Max(
                        Math.Max(SplitItems.Count(f => f.Kind == SplitItemKind.Split) - 1, 0).GetDigitCount(),
                        SplitItems.SuccessGroups
                            .Select(f => f.Name.Length)
                            .DefaultIfEmpty()
                            .Max());
                }

                return _splitPadding;
            }
        }

        public int IndexPadding
        {
            get
            {
                if (_indexPadding == -1)
                {
                    _indexPadding = SplitItems
                        .Select(f => f.Index)
                        .DefaultIfEmpty()
                        .Max()
                        .GetDigitCount();
                }

                return _indexPadding;
            }
        }

        public int LengthPadding
        {
            get
            {
                if (_lengthPadding == -1)
                {
                    _lengthPadding = SplitItems
                        .Select(f => f.Length)
                        .DefaultIfEmpty()
                        .Max()
                        .GetDigitCount();
                }

                return _lengthPadding;
            }
        }

        private Captions Captions
        {
            get { return _settings.Captions; }
        }

        private string PrefixSeparator
        {
            get { return _settings.PrefixSeparator; }
        }

        public SplitItemCollection SplitItems { get; }

        public int InfoLength
        {
            get
            {
                return ((PrefixSeparator.Length + Space.Length + 1) * 3)
                    + SplitPadding
                    + IndexPadding
                    + LengthPadding;
            }
        }
    }
}
