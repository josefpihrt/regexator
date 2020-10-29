// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Regexator.Text.RegularExpressions
{
    public class ReplaceItemEqualityComparer : EqualityComparer<ReplaceItem>
    {
        private static readonly MatchEqualityComparer _matchEqualityComparer = new MatchEqualityComparer();
        private static readonly ReplaceResultEqualityComparer _resultEqualityComparer = new ReplaceResultEqualityComparer();

        public override bool Equals(ReplaceItem x, ReplaceItem y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            return _matchEqualityComparer.Equals(x.Match, y.Match)
                && _resultEqualityComparer.Equals(x.Result, y.Result);
        }

        public override int GetHashCode(ReplaceItem obj)
        {
            if (obj != null)
            {
                return _matchEqualityComparer.GetHashCode(obj.Match)
                    ^ _resultEqualityComparer.GetHashCode(obj.Result);
            }

            return 0;
        }
    }
}
