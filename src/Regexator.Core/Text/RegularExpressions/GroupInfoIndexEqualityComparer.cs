// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Regexator.Text.RegularExpressions
{
    public class GroupInfoIndexEqualityComparer : EqualityComparer<GroupInfo>
    {
        public override bool Equals(GroupInfo x, GroupInfo y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            return x.Index == y.Index;
        }

        public override int GetHashCode(GroupInfo obj)
        {
            if (obj != null)
                return obj.Index.GetHashCode();

            return 0;
        }
    }
}
