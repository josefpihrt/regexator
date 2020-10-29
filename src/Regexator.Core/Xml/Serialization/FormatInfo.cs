// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Drawing;

namespace Regexator.Xml.Serialization
{
    public class FormatInfo
    {
        public static FormatInfo ToSerializable(Regexator.FormatInfo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new FormatInfo()
            {
                Group = item.Group.Name,
                BackColor = item.BackColor.Name,
                ForeColor = item.ForeColor.Name,
                Bold = item.Bold,
                BackColorEnabled = item.BackColorEnabled,
                ForeColorEnabled = item.ForeColorEnabled,
                BoldEnabled = item.BoldEnabled,
                Name = item.Name,
                Text = item.Text
            };
        }

        public static Regexator.FormatInfo FromSerializable(FormatInfo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new Regexator.FormatInfo(Color.FromName(item.BackColor), Color.FromName(item.ForeColor), item.Bold)
            {
                Bold = item.Bold,
                Name = item.Name,
                Text = item.Text,
                BackColorEnabled = item.BackColorEnabled,
                ForeColorEnabled = item.ForeColorEnabled,
                BoldEnabled = item.BoldEnabled
            };
        }

        public string Name { get; set; }
        public string Text { get; set; }
        public string Group { get; set; }
        public string BackColor { get; set; }
        public bool BackColorEnabled { get; set; }
        public string ForeColor { get; set; }
        public bool ForeColorEnabled { get; set; }
        public bool Bold { get; set; }
        public bool BoldEnabled { get; set; }
    }
}
