// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator
{
    public class GuideSearchManager : SearchManagerBase<GuideItem>
    {
        public GuideSearchManager(GuideItem[] items)
            : base(items)
        {
        }

        public override bool Predicate(GuideItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return IsFilterValueEmpty
                || item.Text.IndexOf(FilterValue, Comparison) != -1
                || (SearchInDescription && item.Description.IndexOf(FilterValue, Comparison) != -1);
        }

        public bool SearchInDescription { get; set; }
    }
}
