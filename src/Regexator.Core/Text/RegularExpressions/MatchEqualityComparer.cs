// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Regexator.Text.RegularExpressions
{
    internal class MatchEqualityComparer : EqualityComparer<Match>
    {
        public override bool Equals(Match x, Match y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            return x.Success == y.Success
                && x.Index == y.Index
                && x.Length == y.Length
                && x.Value == y.Value;
        }

        public override int GetHashCode(Match obj)
        {
            if (obj != null)
            {
                return obj.Success.GetHashCode()
                    ^ obj.Index.GetHashCode()
                    ^ obj.Length.GetHashCode()
                    ^ obj.Value.GetHashCode();
            }

            return 0;
        }
    }
}
