// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Regexator.Text;

namespace Regexator.Output
{
    public class CaptureInfoSelections : InfoSelections
    {
        internal CaptureInfoSelections(
            TextSpan match,
            TextSpan group,
            TextSpan capture,
            TextSpan index,
            TextSpan length)
            : base(match, index, length)
        {
            Group = group;
            Capture = capture;
        }

        public override IEnumerable<TextSpan> EnumerateItems()
        {
            yield return Match;
            yield return Group;
            yield return Capture;
            yield return Index;
            yield return Length;
        }

        public TextSpan Group { get; }
        public TextSpan Capture { get; }
    }
}
