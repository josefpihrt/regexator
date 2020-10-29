// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator.FileSystem
{
    [Flags]
    public enum Exceptions
    {
        None = 0,
        Argument = 1,
        IO = 2,
        UnauthorizedAccess = 4,
        Security = 8,
        NotSupported = 16,
        InvalidOperation = 32,
        XmlException = 64
    }
}
