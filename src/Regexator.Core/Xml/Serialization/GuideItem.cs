// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;

namespace Regexator.Xml.Serialization
{
    public class GuideItem
    {
        public static GuideItem ToSerializable(Regexator.GuideItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new GuideItem()
            {
                Text = item.Text,
                Category = item.Category.ToString(),
                Description = item.Description
            };
        }

        public static Regexator.GuideItem FromSerializable(GuideItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new Regexator.GuideItem(item.Text, item.ParseCategory(), item.Description);
        }

        public RegexCategory ParseCategory()
        {
            if (Enum.TryParse(Category, out RegexCategory value))
                return value;

            return RegexCategory.None;
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1059:MembersShouldNotExposeCertainConcreteTypes",
            MessageId = "System.Xml.XmlNode")]
        [XmlElement("Text")]
        public XmlNode TextCData
        {
            get { return (Text == null) ? null : new XmlDocument().CreateCDataSection(Text); }
            set { Text = (value == null) ? "" : value.Value; }
        }

        [XmlIgnore]
        public string Text { get; set; }

        public string Category { get; set; }
        public string Description { get; set; }
    }
}
