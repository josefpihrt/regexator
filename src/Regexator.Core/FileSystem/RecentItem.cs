// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Windows.Forms;

namespace Regexator.FileSystem
{
    public abstract class RecentItem
    {
        protected RecentItem(FileSystemInfo info)
        {
            Info = info ?? throw new ArgumentNullException(nameof(info));
        }

        public abstract ItemKind Kind { get; }

        public override string ToString()
        {
            return FullName;
        }

        public ToolStripMenuItem CreateToolStripMenuItem(Action<RecentItem> onClick)
        {
            string text = CompactText;

            return new ToolStripMenuItem(text, null, (object sender, EventArgs e) => onClick(this))
            {
                ToolTipText = (text.Length != Text.Length) ? Text : null
            };
        }

        public string FullName
        {
            get { return Info.FullName; }
        }

        public virtual string Text
        {
            get { return Info.FullName; }
        }

        public string CompactText
        {
            get { return FileSystemUtility.GetCompactPath(Text); }
        }

        public FileSystemInfo Info { get; }
    }
}