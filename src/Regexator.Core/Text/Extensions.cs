// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Regexator.Text
{
    public static class Extensions
    {
        public static string GetString(this NewLineMode mode)
        {
            switch (mode)
            {
                case NewLineMode.CrLf:
                    return "\r\n";
                case NewLineMode.Lf:
                    return "\n";
                default:
                    return "";
            }
        }
    }
}
