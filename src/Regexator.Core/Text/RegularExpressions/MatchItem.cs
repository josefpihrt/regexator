// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Regexator.Text.RegularExpressions
{
    public sealed class MatchItem
    {
        private GroupInfoCollection _groupInfos;

        internal MatchItem(Match match, GroupInfoCollection groups)
        {
            Match = match;
            Key = ItemIndex.ToString(CultureInfo.CurrentCulture);
            GroupItems = CreateGroupItems(groups);
        }

        private MatchItem(MatchItem previousItem, GroupInfoCollection groups)
        {
            Match = previousItem.Match.NextMatch();
            ItemIndex = previousItem.ItemIndex + 1;
            Key = ItemIndex.ToString(CultureInfo.CurrentCulture);
            GroupItems = CreateGroupItems(groups);
        }

        public GroupItemCollection CreateGroupItems(GroupInfoCollection groups)
        {
            _groupInfos = groups;

            var list = new List<GroupItem>(_groupInfos.Count);

            for (int i = 0; i < _groupInfos.Count; i++)
                list.Add(new GroupItem(Match.Groups[groups[i].Index], groups[i], i, this));

            return new GroupItemCollection(list);
        }

        public MatchItem NextItem() => new MatchItem(this, _groupInfos);

        public override string ToString() => Value;

        public IEnumerable<GroupItem> EnumerateGroupItems(GroupSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            return GroupItems
                .Where(f => !settings.IsIgnored(f.Name))
                .OrderBy(f => f.GroupInfo, settings.Sorter);
        }

        public IEnumerable<CaptureItem> EnumerateCaptureItems => GroupItems.ToCaptureItems();

        public string Value => Match.Value;

        public int Index => Match.Index;

        public int Length => Match.Length;

        public bool Success => Match.Success;

        public Match Match { get; }

        public int ItemIndex { get; }

        public string Key { get; }

        public GroupItemCollection GroupItems { get; }
    }
}
