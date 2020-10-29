// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;

namespace Regexator
{
    [Flags]
    public enum ReplacementOptions
    {
        None = 0,
        CurrentLineIncludesNewLine = 1,
        CurrentLineOnly = 2,

        [Browsable(false)]
        TextContainsCDataEnd = 4,

        [Browsable(false)]
        WordWrap = 8,
#if !DEBUG
        [Browsable(false)]
#endif
        ToUpper = 16,
#if !DEBUG
        [Browsable(false)]
#endif
        ToLower = 32
    }
}
