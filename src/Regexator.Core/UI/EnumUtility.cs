// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Regexator.UI
{
    public static class EnumUtility
    {
        public static string GetDescription(UIElement value)
        {
            switch (value)
            {
                case UIElement.Pattern:
                    return Resources.Pattern;
                case UIElement.Replacement:
                    return Resources.Replacement;
                case UIElement.Input:
                    return Resources.Input;
                case UIElement.OutputText:
                    return Resources.OutputText;
                case UIElement.OutputTable:
                    return Resources.OutputTable;
                case UIElement.OutputSummary:
                    return Resources.OutputSummary;
                case UIElement.RegexOptions:
                    return Resources.RegexOptions;
                case UIElement.Groups:
                    return Resources.Groups;
                case UIElement.ProjectInfo:
                    return Resources.ProjectInfo;
                case UIElement.FindResults:
                    return Resources.FindResults;
                case UIElement.ProjectExplorer:
                    return Resources.ProjectExplorer;
                case UIElement.FileSystemSearchResults:
                    return Resources.FileSystemSearch;
                default:
                    {
                        Debug.Fail(string.Format("Missing case for {0}", value));
                        return value.ToString();
                    }
            }
        }
    }
}
