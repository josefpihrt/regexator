// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Regexator.Text.RegularExpressions
{
    public enum CharEscapeMode
    {
        None = 0,
        AsciiHexadecimal = 1,
        Backslash = 2,
        Bell = 3,
        CarriageReturn = 4,
        Escape = 5,
        FormFeed = 6,
        Linefeed = 7,
        Tab = 8,
        VerticalTab = 9,
    }
}
