// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator.Text
{
    [Flags]
    public enum SummaryElements
    {
        None = 0,
        Author = 1,
        Description = 2,
        Title = 4,
        Mode = 8,
        RegexOptions = 16,
        Pattern = 32,
        Replacement = 64,
        Input = 128,
        Output = 256,
        Groups = 512,
        ReplacementMode = 1024
    }
}
