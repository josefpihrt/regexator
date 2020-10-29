// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public class ExtendedToolStrip : ToolStrip
    {
        public ExtendedToolStrip()
        {
            ToolStripItems = new Collection<ToolStripItem>();

            AutoSize = false;
            Height = 25;
            LeftMargin = 3;
            FirstItemHasLeftMargin = true;
            UseCustomMargin = true;
            Padding = new Padding(0, 0, 2, 0);
            ItemMargin = new Padding(LeftMargin, 0, 0, 0);
            GripStyle = ToolStripGripStyle.Hidden;
            BackColor = SystemColors.Control;
            TabStop = false;
            DoubleBuffered = true;
            ItemHeight = 23;
            ButtonWidth = 23;
            SplitButtonWidth = 35;
            DropDownButtonWidth = 30;
        }

        public virtual void Load()
        {
            Load(ToolStripItems);
        }

        public void Load(bool clearItems)
        {
            Load(ToolStripItems, clearItems);
        }

        public void Load(ToolStripItem item)
        {
            Load(new ToolStripItem[] { item });
        }

        public void Load(IEnumerable<ToolStripItem> items)
        {
            Load(items, true);
        }

        public void Load(IEnumerable<ToolStripItem> items, bool clearItems)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (clearItems)
                Items.Clear();

            foreach (ToolStripItem item in items)
                Items.Add(item);
        }

        protected override void OnItemAdded(ToolStripItemEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (UseCustomMargin)
            {
                if (Items.Count == 1 && !FirstItemHasLeftMargin)
                {
                    e.Item.Margin = new Padding(0, ItemMargin.Top, ItemMargin.Right, ItemMargin.Bottom);
                }
                else
                {
                    e.Item.Margin = new Padding(2, ItemMargin.Top, ItemMargin.Right, ItemMargin.Bottom);
                }
            }

            if (e.Item is ToolStripButton || e.Item is ToolStripSplitButton || e.Item is ToolStripDropDownButton)
            {
                if (!ButtonBackColor.IsEmpty)
                    e.Item.BackColor = ButtonBackColor;

                if (!ButtonForeColor.IsEmpty)
                    e.Item.ForeColor = ButtonForeColor;
            }

            if (e.Item is ToolStripButton
                || e.Item is ToolStripSplitButton
                || e.Item is ToolStripDropDownButton
                || ((e.Item is ToolStripControlHost host)
                    && (host.Control is CheckBox
                        || host.Control is RadioButton)))
            {
                if (!ItemAutoSize)
                {
                    e.Item.AutoSize = false;
                    if (e.Item is ToolStripButton)
                    {
                        e.Item.Width = ButtonWidth;
                    }
                    else if (e.Item is ToolStripSplitButton)
                    {
                        e.Item.Width = SplitButtonWidth;
                    }
                    else if (e.Item is ToolStripDropDownButton)
                    {
                        e.Item.Width = DropDownButtonWidth;
                    }
                    else if (e.Item is ToolStripControlHost)
                    {
                        e.Item.Width = ButtonWidth;
                    }

                    e.Item.Height = ItemHeight;
                }
            }

            base.OnItemAdded(e);
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            Keys modifiers = keyData & Keys.Modifiers;

            if (modifiers == Keys.Control
                && (keyData & Keys.KeyCode) == Keys.Tab
                && ProcessCtrlTab())
            {
                return true;
            }

            if (modifiers == (Keys.Control | Keys.Shift)
                && (keyData & Keys.KeyCode) == Keys.Tab
                && ProcessCtrlShiftTab())
            {
                return true;
            }

            return base.ProcessCmdKey(ref m, keyData);
        }

        protected virtual bool ProcessCtrlTab()
        {
            return true;
        }

        protected virtual bool ProcessCtrlShiftTab()
        {
            return true;
        }

        public bool ItemAutoSize { get; set; }
        public int LeftMargin { get; set; }
        public Padding ItemMargin { get; set; }
        public bool FirstItemHasLeftMargin { get; set; }
        public bool UseCustomMargin { get; set; }
        public Color ButtonBackColor { get; set; }
        public Color ButtonForeColor { get; set; }
        public int ItemHeight { get; set; }
        public int ButtonWidth { get; set; }
        public int SplitButtonWidth { get; set; }
        public int DropDownButtonWidth { get; set; }

        public Collection<ToolStripItem> ToolStripItems { get; }
    }
}
