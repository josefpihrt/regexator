// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Regexator.Text;
using Regexator.Text.RegularExpressions;

namespace Regexator.Output
{
    internal class MatchTextBuilder
    {
        private readonly HashSet<int> _ignoredGroupIndexes;
        private readonly bool _noIgnoredGroups;
        private int _matchPadding = -1;
        private int _groupPadding = -1;
        private int _capturePadding = -1;
        private int _indexPadding = -1;
        private int _lengthPadding = -1;
        private string _matchSpaces;
        private string _groupSpaces;

        private const string ItemSeparator = " ";

        public MatchTextBuilder(MatchItemCollection items)
            : this(items, new OutputSettings())
        {
        }

        public MatchTextBuilder(MatchItemCollection items, OutputSettings settings)
            : this(items, settings, new GroupSettings())
        {
        }

        public MatchTextBuilder(MatchItemCollection items, GroupSettings groupSettings)
            : this(items, new OutputSettings(), groupSettings)
        {
        }

        public MatchTextBuilder(MatchItemCollection items, OutputSettings settings, GroupSettings groupSettings)
        {
            MatchItems = items ?? throw new ArgumentNullException(nameof(items));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            GroupSettings = groupSettings ?? throw new ArgumentNullException(nameof(groupSettings));
            HasDefaultGroupSettings = GroupSettings.HasDefaultValues(GroupSettings);
            _noIgnoredGroups = groupSettings.IgnoredGroups.Count == 0;
            _ignoredGroupIndexes = new HashSet<int>(items.GroupInfos
                .Where(f => groupSettings.IsIgnored(f))
                .Select(f => f.Index));
        }

        public string GetInfo(CaptureItem item, bool isMatchInfoRepeated, bool isGroupInfoRepeated)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (Settings.AddInfo)
            {
                return ((Settings.OmitRepeatedInfo && isMatchInfoRepeated) ? MatchSpaces : GetMatchInfo(item)) +
                    ((Settings.OmitRepeatedInfo && isGroupInfoRepeated) ? GroupSpaces : GetGroupInfo(item)) +
                    GetCaptureInfo(item) +
                    GetIndexInfo(item) +
                    GetLengthInfo(item);
            }

            return "";
        }

        private string GetMatchInfo(CaptureItem item)
        {
            return GetInfo(Captions.MatchChar, () => PadNumber(item.MatchItem.ItemIndex, MatchPadding));
        }

        private string GetGroupInfo(CaptureItem item)
        {
            return GetInfo(Captions.GroupChar, () => item.GroupInfo.Name.PadRight(GroupPadding));
        }

        private string GetCaptureInfo(CaptureItem item)
        {
            return GetInfo(Captions.CaptureChar, () => PadNumber(item.ItemIndex, CapturePadding));
        }

        private string GetIndexInfo(CaptureItem item)
        {
            return GetInfo(Captions.IndexChar, () => PadNumber(item.Index, IndexPadding));
        }

        private string GetLengthInfo(CaptureItem item)
        {
            return GetInfo(Captions.LengthChar, () => PadNumber(item.Length, LengthPadding));
        }

        private string GetInfo(char character, Func<string> valueFactory)
        {
            return character + PrefixSeparator + valueFactory() + ItemSeparator;
        }

        private string PadNumber(int value, int totalWidth)
        {
            return OutputFormatter.PadNumber(value, totalWidth, Settings.NumberAlignment);
        }

        public int MatchPadding
        {
            get
            {
                if (_matchPadding == -1)
                    _matchPadding = Math.Max(MatchItems.Count - 1, 0).GetDigitCount();

                return _matchPadding;
            }
        }

        public int GroupPadding
        {
            get
            {
                if (_groupPadding == -1)
                {
                    _groupPadding = MatchItems.SuccessGroups
                        .Select(f => f.Name)
                        .Except(GroupSettings.IgnoredGroups)
                        .Select(f => f.Length)
                        .DefaultIfEmpty()
                        .Max();
                }

                return _groupPadding;
            }
        }

        public int CapturePadding
        {
            get
            {
                if (_capturePadding == -1)
                {
                    _capturePadding = MatchItems.ToSuccessGroupItems()
                        .Where(f => _noIgnoredGroups || !GroupSettings.IsIgnored(f.GroupInfo))
                        .Select(f => f.CaptureCount)
                        .DefaultIfEmpty()
                        .Max()
                        .GetDigitCount();
                }

                return _capturePadding;
            }
        }

        public int IndexPadding
        {
            get
            {
                if (_indexPadding == -1)
                {
                    _indexPadding = MatchItems.ToCaptureItems()
                        .Where(f => _noIgnoredGroups || !_ignoredGroupIndexes.Contains(f.GroupInfo.Index))
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
                    _lengthPadding = MatchItems.ToCaptureItems()
                        .Where(f => _noIgnoredGroups || !_ignoredGroupIndexes.Contains(f.GroupInfo.Index))
                        .Select(f => f.Length)
                        .DefaultIfEmpty()
                        .Max()
                        .GetDigitCount();
                }

                return _lengthPadding;
            }
        }

        public string MatchSpaces
        {
            get
            {
                return _matchSpaces
                    ?? (_matchSpaces = new string(
                        ' ',
                        MatchPadding
                            + Captions.MatchCharLength
                            + PrefixSeparator.Length
                            + ItemSeparator.Length));
            }
        }

        public string GroupSpaces
        {
            get
            {
                return _groupSpaces
                    ?? (_groupSpaces = new string(
                        ' ',
                        GroupPadding
                            + Captions.GroupCharLength
                            + PrefixSeparator.Length
                            + ItemSeparator.Length));
            }
        }

        public CaptureInfoSelections GetInfoSelections(int startIndex)
        {
            TextSpan[] arr = ((Settings.AddInfo) ? EnumerateInfoSpans(startIndex) : Enumerable.Repeat(TextSpan.Empty, 5))
                .ToArray();
            return new CaptureInfoSelections(arr[0], arr[1], arr[2], arr[3], arr[4]);
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
            yield return MatchPadding + Captions.MatchCharLength;
            yield return GroupPadding + Captions.GroupCharLength;
            yield return CapturePadding + Captions.CaptureCharLength;
            yield return IndexPadding + Captions.IndexCharLength;
            yield return LengthPadding + Captions.LengthCharLength;
        }

        private Captions Captions
        {
            get { return Settings.Captions; }
        }

        private string PrefixSeparator
        {
            get { return Settings.PrefixSeparator; }
        }

        public OutputSettings Settings { get; }

        public GroupSettings GroupSettings { get; }

        public MatchItemCollection MatchItems { get; }

        public bool HasDefaultGroupSettings { get; }

        public int InfoLength
        {
            get
            {
                return MatchPadding
                    + GroupPadding
                    + CapturePadding
                    + IndexPadding
                    + LengthPadding
                    + ((PrefixSeparator.Length + ItemSeparator.Length + 1) * 5);
            }
        }
    }
}
