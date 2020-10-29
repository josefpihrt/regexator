// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Regexator.Text;

namespace Regexator.Output
{
    public class InfoSelections
    {
        internal InfoSelections(
            TextSpan match,
            TextSpan index,
            TextSpan length)
        {
            Match = match;
            Index = index;
            Length = length;
        }

        public virtual IEnumerable<TextSpan> EnumerateItems()
        {
            yield return Match;
            yield return Index;
            yield return Length;
        }

        public TextSpan Match { get; }
        public TextSpan Index { get; }
        public TextSpan Length { get; }
    }
}
