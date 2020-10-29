// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator
{
    public class RegexCategoryInfo
    {
        public RegexCategoryInfo(RegexCategory category, Uri url)
        {
            Category = category;
            Url = url;
        }

        public RegexCategory Category { get; }
        public Uri Url { get; }
    }
}
