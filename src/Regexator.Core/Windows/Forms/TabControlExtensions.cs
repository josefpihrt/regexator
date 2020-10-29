// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public static class TabControlExtensions
    {
        public static TabPage GetTabAt(this TabControl tbc, Point point)
        {
            if (tbc == null)
                throw new ArgumentNullException(nameof(tbc));

            for (int i = 0; i < tbc.TabCount; i++)
            {
                if (tbc.GetTabRect(i).Contains(point))
                    return tbc.TabPages[i];
            }

            return null;
        }

        public static void TrySelectFirst(this TabControl tbc)
        {
            if (tbc == null)
                throw new ArgumentNullException(nameof(tbc));

            if (tbc.TabCount > 0)
                tbc.SelectTab(0);
        }

        public static void TrySelectLast(this TabControl tbc)
        {
            if (tbc == null)
                throw new ArgumentNullException(nameof(tbc));

            if (tbc.TabCount > 0)
                tbc.SelectTab(tbc.TabCount - 1);
        }

        public static void SelectNextTab(this TabControl tbc)
        {
            if (tbc == null)
                throw new ArgumentNullException(nameof(tbc));

            if (tbc.TabCount > 0)
            {
                int index = tbc.SelectedIndex;
                tbc.SelectTab((index == -1 || index == tbc.TabCount - 1)
                    ? 0
                    : index + 1);
            }
        }

        public static void SelectPreviousTab(this TabControl tbc)
        {
            if (tbc == null)
                throw new ArgumentNullException(nameof(tbc));

            if (tbc.TabCount > 0)
            {
                int index = tbc.SelectedIndex;
                tbc.SelectTab((index == -1 || index == 0)
                    ? tbc.TabCount - 1
                    : index - 1);
            }
        }
    }
}
