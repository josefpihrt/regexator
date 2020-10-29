// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public sealed class DataGridViewColumnInfo
    {
        public DataGridViewColumnInfo()
        {
        }

        public DataGridViewColumnInfo(
            string name,
            string headerText,
            int displayIndex,
            int width,
            bool visible,
            DataGridViewContentAlignment contentAlignment,
            bool isSystemColumn)
        {
            Name = name;
            HeaderText = headerText;
            DisplayIndex = displayIndex;
            ContentAlignment = contentAlignment;
            Visible = visible;
            IsSystemColumn = isSystemColumn;
            Width = width;
        }

        public string Name { get; set; }
        public string HeaderText { get; set; }
        public int DisplayIndex { get; set; }
        public bool Visible { get; set; }
        public bool IsSystemColumn { get; set; }
        public DataGridViewContentAlignment ContentAlignment { get; set; }
        public int Width { get; set; }
        public string Format { get; set; }
    }
}
