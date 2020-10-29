// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Globalization;

namespace Regexator.Output
{
    internal static class OutputFormatter
    {
        public static string PadNumber(int value, int totalWidth, TextAlignment alignment)
        {
            return (alignment == TextAlignment.Left)
                ? value.ToString(CultureInfo.CurrentCulture).PadRight(totalWidth)
                : value.ToString(CultureInfo.CurrentCulture).PadLeft(totalWidth);
        }
    }
}
