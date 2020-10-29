// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;

namespace Regexator
{
    [Flags]
    public enum InputOptions
    {
        None = 0,
        CurrentLineIncludesNewLine = 1,
        CurrentLineOnly = 2,
        Highlight = 4,
#if !DEBUG
        [Browsable(false)]
#endif
        ReplacementSync = 8,

        [Browsable(false)]
        TextContainsCDataEnd = 16,

        [Browsable(false)]
        WordWrap = 32
    }
}
