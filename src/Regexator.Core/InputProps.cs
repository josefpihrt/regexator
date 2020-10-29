// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator
{
    [Flags]
    public enum InputProps
    {
        None = 0,
        Name = 1,
        Options = 2,
        Text = 4,
        NewLine = 8,
        CurrentLine = 16,
        Encoding = 32,
        Attributes = 64
    }
}
