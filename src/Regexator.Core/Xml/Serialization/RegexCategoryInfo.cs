// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Regexator.Xml.Serialization
{
    public class RegexCategoryInfo
    {
        public static RegexCategoryInfo ToSerializable(Regexator.RegexCategoryInfo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new RegexCategoryInfo()
            {
                Category = item.Category.ToString(),
                Url = item.Url?.ToString()
            };
        }

        public static Regexator.RegexCategoryInfo FromSerializable(RegexCategoryInfo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new Regexator.RegexCategoryInfo(item.ParseCategory(), (item.Url != null) ? new Uri(item.Url) : null);
        }

        public RegexCategory ParseCategory()
        {
            if (Enum.TryParse(Category, out RegexCategory value))
                return value;

            return RegexCategory.None;
        }

        public string Category { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string Url { get; set; }
    }
}
