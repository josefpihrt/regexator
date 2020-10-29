// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Globalization;
using System.Text.RegularExpressions;

namespace Regexator.Text.RegularExpressions
{
    public sealed class GroupSplitItem : SplitItem
    {
        private readonly Group _group;

        internal GroupSplitItem(Group group, GroupInfo groupInfo, int itemIndex)
        {
            _group = group;
            GroupInfo = groupInfo;
            ItemIndex = itemIndex;
            Key = itemIndex.ToString(CultureInfo.CurrentCulture);
        }

        public override string Value => _group.Value;

        public override int Index => _group.Index;

        public override int Length => _group.Length;

        public override int ItemIndex { get; }

        public override string Name => GroupInfo.Name;

        public override GroupInfo GroupInfo { get; }

        public override SplitItemKind Kind => SplitItemKind.Group;

        public override string Key { get; }
    }
}
