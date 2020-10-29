// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;

namespace Regexator
{
    [Flags]
    public enum PatternOptions
    {
        None = 0,
        CurrentLineOnly = 1,
#if !DEBUG
        [Browsable(false)]
#endif
        InputSync = 2,
#if !DEBUG
        [Browsable(false)]
#endif
        ReplacementSync = 4,

        [Browsable(false)]
        TextContainsCDataEnd = 8,

        [Browsable(false)]
        WordWrap = 16
    }
}
