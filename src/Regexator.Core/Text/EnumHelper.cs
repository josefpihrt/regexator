// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Regexator.Text
{
    public static class EnumHelper
    {
        public static string GetDescription(ExportMode value)
        {
            switch (value)
            {
                case ExportMode.CSharp:
                    return Resources.CSharp;
                case ExportMode.CSharpVerbatim:
                    return Resources.CSharpVerbatim;
                case ExportMode.VisualBasic:
                    return Resources.VisualBasic;
                default:
                    return value.ToString();
            }
        }

        public static string GetDescription(NewLineLiteral value)
        {
            switch (value)
            {
                case NewLineLiteral.Lf:
                    return Resources.Linefeed;
                case NewLineLiteral.CrLf:
                    return Resources.CarriageReturnPlusLinefeed;
                case NewLineLiteral.Environment:
                    return Resources.Environment;
                default:
                    return value.ToString();
            }
        }

        public static string GetDescription(ConcatOperatorPosition value)
        {
            switch (value)
            {
                case ConcatOperatorPosition.End:
                    return Resources.End;
                case ConcatOperatorPosition.Start:
                    return Resources.Start;
                default:
                    return value.ToString();
            }
        }

        public static string GetAbbreviation(ExportMode value)
        {
            switch (value)
            {
                case ExportMode.CSharp:
                    return Resources.CSharp;
                case ExportMode.CSharpVerbatim:
                    return Resources.CSharpVerbatim;
                case ExportMode.VisualBasic:
                    return Resources.VisualBasicAbbr;
                default:
                    return value.ToString();
            }
        }
    }
}
