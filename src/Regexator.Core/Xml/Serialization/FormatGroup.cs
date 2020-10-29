// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Drawing;

namespace Regexator.Xml.Serialization
{
    public class FormatGroup
    {
        public static FormatGroup ToSerializable(Regexator.FormatGroup item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new FormatGroup()
            {
                FontFamily = item.FontFamily.Name,
                FontSize = item.FontSize,
                Name = item.Name,
                Text = item.Text
            };
        }

        public static Regexator.FormatGroup FromSerializable(FormatGroup item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new Regexator.FormatGroup(new FontFamily(item.FontFamily), item.FontSize)
            {
                Name = item.Name,
                Text = item.Text
            };
        }

        public string Name { get; set; }
        public string Text { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
    }
}
