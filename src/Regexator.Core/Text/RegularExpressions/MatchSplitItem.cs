// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Globalization;

namespace Regexator.Text.RegularExpressions
{
    public sealed class MatchSplitItem : SplitItem
    {
        internal MatchSplitItem(string value)
        {
            Value = value;
            Name = "0";
            Key = "0";
        }

        internal MatchSplitItem(string value, int index, int itemIndex, int splitIndex)
        {
            Value = value;
            Index = index;
            ItemIndex = itemIndex;
            Key = ItemIndex.ToString(CultureInfo.CurrentCulture);
            Name = splitIndex.ToString(CultureInfo.CurrentCulture);
        }

        public override string Value { get; }

        public override int Index { get; }

        public override int Length => Value.Length;

        public override int ItemIndex { get; }

        public override string Name { get; }

        public override SplitItemKind Kind => SplitItemKind.Split;

        public override string Key { get; }
    }
}
