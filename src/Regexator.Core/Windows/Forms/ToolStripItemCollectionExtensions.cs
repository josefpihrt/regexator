// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public static class ToolStripItemCollectionExtensions
    {
        public static void AddSeparator(this ToolStripItemCollection items)
        {
            AddSeparatorIf(items, true);
        }

        public static void AddSeparatorIfAny(this ToolStripItemCollection items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            AddSeparatorIf(items, items.Count > 0);
        }

        public static void AddSeparatorIf(this ToolStripItemCollection items, bool condition)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (condition)
                items.Add(new ToolStripSeparator());
        }

        public static void LoadItems(this ToolStripItemCollection collection, IEnumerable<ToolStripItem> items)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (items == null)
                throw new ArgumentNullException(nameof(items));

            collection.Clear();
            collection.AddRange(items.ToArray());
        }
    }
}
