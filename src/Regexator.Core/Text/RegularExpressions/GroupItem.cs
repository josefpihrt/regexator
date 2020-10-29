// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Text.RegularExpressions;

namespace Regexator.Text.RegularExpressions
{
    public sealed class GroupItem
    {
        internal GroupItem(Group group, GroupInfo groupInfo, int itemIndex, MatchItem matchItem)
        {
            Group = group;
            GroupInfo = groupInfo;
            ItemIndex = itemIndex;
            MatchItem = matchItem;
            Key = matchItem.Key + groupInfo.Name;

            CaptureItems = new CaptureItemCollection(Captures
                .Cast<Capture>()
                .Select((c, i) => new CaptureItem(c, this, i))
                .ToArray());
        }

        public override string ToString() => Group.ToString();

        public CaptureCollection Captures => Group.Captures;

        public int CaptureCount => Captures.Count;

        public string Value => Group.Value;

        public int Index => Group.Index;

        public int Length => Group.Length;

        public bool Success => Group.Success;

        public bool IsDefaultGroup => GroupInfo.Index == 0;

        public Group Group { get; }

        public GroupInfo GroupInfo { get; }

        public string Name => GroupInfo.Name;

        public int ItemIndex { get; }

        public string Key { get; }

        public CaptureItemCollection CaptureItems { get; }

        public MatchItem MatchItem { get; }
    }
}
