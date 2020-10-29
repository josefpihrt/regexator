// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator
{
    [Flags]
    public enum OutputOptions
    {
        None = 0,
        Highlight = 1,
        Info = 2,
        CarriageReturnSymbol = 4,
        LinefeedSymbol = 8,
        TabSymbol = 16,
        NoCaptureSymbol = 32,
        WrapText = 64,
        SpaceSymbol = 128
    }
}
