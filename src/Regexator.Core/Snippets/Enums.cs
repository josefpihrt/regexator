// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Regexator.Snippets
{
    public static class EnumExtensions
    {
        public static string GetDescription(this SnippetCodeKind value)
        {
            switch (value)
            {
                case SnippetCodeKind.None:
                    return Resources.Default;
                case SnippetCodeKind.Format:
                    return Resources.Format;
                case SnippetCodeKind.Negative:
                    return Resources.Negative;
                default:
                    return value.ToString();
            }
        }
    }
}