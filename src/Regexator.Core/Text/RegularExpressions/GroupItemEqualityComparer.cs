// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Regexator.Text.RegularExpressions
{
    public class GroupItemEqualityComparer : EqualityComparer<GroupItem>
    {
        private static readonly CaptureItemEqualityComparer _comparer = new CaptureItemEqualityComparer();

        public override bool Equals(GroupItem x, GroupItem y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            return x.CaptureItems.SequenceEqual(y.CaptureItems, _comparer);
        }

        public override int GetHashCode(GroupItem obj)
        {
            if (obj != null)
            {
                int hashCode = 0;
                foreach (CaptureItem captureItem in obj.CaptureItems)
                    hashCode ^= _comparer.GetHashCode(captureItem);

                return hashCode;
            }

            return 0;
        }
    }
}
