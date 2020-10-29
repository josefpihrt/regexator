// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator.Snippets
{
    [Flags]
    public enum SnippetKinds
    {
        None = 0,
        Regular = 1,
        Surround = 2
    }
}
