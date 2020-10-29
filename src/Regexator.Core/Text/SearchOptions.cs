// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator.Text
{
    [Flags]
    public enum SearchOptions
    {
        None = 0,
        MatchCase = 1,
        MatchWholeWord = 2,
        CultureInvariant = 4
    }
}
