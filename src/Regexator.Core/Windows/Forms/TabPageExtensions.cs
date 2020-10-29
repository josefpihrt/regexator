// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public static class TabPageExtensions
    {
        public static void SelectTab(this TabPage tabPage)
        {
            if (tabPage == null)
                throw new ArgumentNullException(nameof(tabPage));

            if (tabPage.Parent is TabControl tbc)
                tbc.SelectedTab = tabPage;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static TabControl GetTabControl(this TabPage tabPage)
        {
            if (tabPage == null)
                throw new ArgumentNullException(nameof(tabPage));

            return tabPage.Parent as TabControl;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static bool IsSelected(this TabPage tabPage)
        {
            if (tabPage == null)
                throw new ArgumentNullException(nameof(tabPage));

            if (tabPage.Parent is TabControl tbc)
                return ReferenceEquals(tbc.SelectedTab, tabPage);

            return false;
        }
    }
}
