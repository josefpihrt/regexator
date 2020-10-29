// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Regexator.Output
{
    [DebuggerDisplay("Index: {Index}")]
    public class IndexSearcherItem :
        IComparer<IndexSearcherItem>,
        IComparable<IndexSearcherItem>,
        IEquatable<IndexSearcherItem>
    {
        public IndexSearcherItem(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            Index = index;
        }

        public IndexSearcherItem(int index, RegexBlock block)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            Index = index;
            Block = block ?? throw new ArgumentNullException(nameof(block));
        }

        public bool Equals(IndexSearcherItem other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            var other = obj as IndexSearcherItem;
            return CompareTo(other) == 0;
        }

        public int CompareTo(IndexSearcherItem other)
        {
            if (other is null)
                return 1;

            return Index.CompareTo(other.Index);
        }

        public int Compare(IndexSearcherItem x, IndexSearcherItem y)
        {
            return CompareItems(x, y);
        }

        private static int CompareItems(IndexSearcherItem left, IndexSearcherItem right)
        {
            if (ReferenceEquals(left, right))
                return 0;

            if (left is null)
                return -1;

            return left.CompareTo(right);
        }

        public override int GetHashCode()
        {
            return Index.GetHashCode();
        }

        public static bool operator ==(IndexSearcherItem left, IndexSearcherItem right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(IndexSearcherItem left, IndexSearcherItem right)
        {
            return !(left == right);
        }

        public static bool operator <(IndexSearcherItem left, IndexSearcherItem right)
        {
            return CompareItems(left, right) < 0;
        }

        public static bool operator >(IndexSearcherItem left, IndexSearcherItem right)
        {
            return CompareItems(left, right) > 0;
        }

        public int SelectionIndex
        {
            get
            {
                if (Block?.TextSpans.Count > 0)
                    return Block.TextSpans.Min(f => f.Index);

                return -1;
            }
        }

        public RegexBlock Block { get; }

        public int Index { get; }
    }
}
