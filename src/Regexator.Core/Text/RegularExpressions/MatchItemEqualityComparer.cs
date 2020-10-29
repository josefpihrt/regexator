// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Regexator.Text.RegularExpressions
{
    public class MatchItemEqualityComparer : EqualityComparer<MatchItem>
    {
        private static readonly GroupItemEqualityComparer _comparer = new GroupItemEqualityComparer();

        public override bool Equals(MatchItem x, MatchItem y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            return x.GroupItems.SequenceEqual(y.GroupItems, _comparer);
        }

        public override int GetHashCode(MatchItem obj)
        {
            if (obj != null)
            {
                int hashCode = 0;
                foreach (GroupItem groupItem in obj.GroupItems)
                    hashCode ^= _comparer.GetHashCode(groupItem);

                return hashCode;
            }

            return 0;
        }
    }
}
