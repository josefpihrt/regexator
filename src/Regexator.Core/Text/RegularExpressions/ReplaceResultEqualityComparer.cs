// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Regexator.Text.RegularExpressions
{
    public class ReplaceResultEqualityComparer : EqualityComparer<ReplaceResult>
    {
        public override bool Equals(ReplaceResult x, ReplaceResult y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            return x.Index == y.Index
                && x.Length == y.Length
                && x.Value == y.Value;
        }

        public override int GetHashCode(ReplaceResult obj)
        {
            if (obj != null)
            {
                return obj.Index.GetHashCode()
                    ^ obj.Length.GetHashCode()
                    ^ obj.Value.GetHashCode();
            }

            return 0;
        }
    }
}
