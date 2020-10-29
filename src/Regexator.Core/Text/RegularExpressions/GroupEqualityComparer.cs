// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Regexator.Text.RegularExpressions
{
    public class GroupEqualityComparer : EqualityComparer<Group>
    {
        private static readonly CaptureEqualityComparer _comparer = new CaptureEqualityComparer();

        public override bool Equals(Group x, Group y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            IEnumerator e1 = x.Captures.GetEnumerator();
            IEnumerator e2 = y.Captures.GetEnumerator();

            while (e1.MoveNext())
            {
                if (!(e2.MoveNext() && _comparer.Equals((Capture)e1.Current, (Capture)e2.Current)))
                    return false;
            }

            return !e2.MoveNext();
        }

        public override int GetHashCode(Group obj)
        {
            if (obj != null)
            {
                int hashCode = 0;
                foreach (Capture capture in obj.Captures)
                    hashCode ^= _comparer.GetHashCode(capture);

                return hashCode;
            }

            return 0;
        }
    }
}
