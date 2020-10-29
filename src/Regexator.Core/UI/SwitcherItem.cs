// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Regexator.Windows.Forms;

namespace Regexator.UI
{
    public class SwitcherItem
    {
        public SwitcherItem(UIElement element, Control control)
            : this(element, control, null)
        {
        }

        public SwitcherItem(UIElement element, Control control, Control parentControl)
        {
            Element = element;
            Control = control ?? throw new ArgumentNullException(nameof(control));
            ParentControl = parentControl;
            Enabled = true;
        }

        public void SelectControl()
        {
            var stack = new Stack<TabPage>();
            Control control = Control;

            while (control != null)
            {
                TabPage tbp = control.FindParent<TabPage>();
                if (tbp != null)
                    stack.Push(tbp);

                control = tbp;
            }

            while (stack.Count > 0)
                stack.Pop().SelectTab();

            Control.Select();
        }

        public ToolStripMenuItem ToolStripMenuItem
        {
            get { return _tsi ?? (_tsi = new ToolStripMenuItem(LocalizedText) { Tag = this, Enabled = Enabled }); }
        }

        public bool Enabled { get; set; }

        public string LocalizedText
        {
            get { return EnumUtility.GetDescription(Element); }
        }

        public UIElement Element { get; }

        public Control Control { get; }

        public Control ParentControl { get; }

        private ToolStripMenuItem _tsi;
    }
}
