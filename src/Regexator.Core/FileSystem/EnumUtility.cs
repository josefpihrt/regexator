// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Regexator.FileSystem
{
    public static class EnumUtility
    {
        public static string GetDescription(View value)
        {
            switch (value)
            {
                case View.Solution:
                    return Resources.Solution;
                case View.FileSystem:
                    return Resources.FileSystem;
                default:
                    return "";
            }
        }
    }
}
