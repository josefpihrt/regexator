// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace Regexator.Text.RegularExpressions
{
    public sealed class CaptureItem
    {
        internal CaptureItem(Capture capture, GroupItem groupItem, int itemIndex)
        {
            Capture = capture;
            GroupItem = groupItem;
            ItemIndex = itemIndex;
            Key = groupItem.Key + itemIndex;
        }

        public override string ToString() => Value;

        public string Value => Capture.Value;

        public int Index => Capture.Index;

        public int Length => Capture.Length;

        public MatchItem MatchItem => GroupItem.MatchItem;

        public bool IsDefaultCapture => GroupItem.IsDefaultGroup;

        public Capture Capture { get; }

        public GroupItem GroupItem { get; }

        public GroupInfo GroupInfo => GroupItem.GroupInfo;

        public int ItemIndex { get; }

        public string Key { get; }
    }
}