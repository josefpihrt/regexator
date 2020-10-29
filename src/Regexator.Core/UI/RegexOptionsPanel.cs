// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Regexator.UI;
using Regexator.Text;
using Regexator.Windows.Forms;

namespace Regexator
{
    public class RegexOptionsPanel : IDisposable
    {
        public RegexOptionsPanel(RegexOptionsManager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            LoadItems();
            DataGridView.ContextMenuStrip = CreateContextMenuStrip();
            DataGridView.DataSource = _items;
            DataGridView.CellContextMenuStripNeeded += (f, f2) => DataGridView_CellContextMenuStripNeeded(f, f2);
            DataGridView.KeyDown += (f, f2) => DataGridView_KeyDown(f, f2);
            DataGridView.CellMouseDoubleClick += (f, f2) => DataGridView_CellMouseDoubleClick(f, f2);
            _manager.ItemVisibleChanged += delegate { LoadItems(); };
            _manager.ValueChanged += delegate
            {
                if (_manager.Value.Except(GetValue()).Any() || _items.Any(f => !f.Enabled && !f.Visible))
                    LoadItems();
            };
        }

        public void LoadItems()
        {
            RegexOptions[] selected = DataGridView.EnumerateSelectedItems().Select(f => f.Value).ToArray();
            _items.Clear();

            foreach (RegexOptionsItem item in _manager.Items.Where(f => f.Enabled || f.Visible))
                _items.Add(item);

            DataGridView.ClearSelection();

            foreach (DataGridViewRow row in DataGridView.Rows.Cast<DataGridViewRow>()
                .Join(selected, f => ((RegexOptionsItem)f.DataBoundItem).Value, g => g, (f, _) => f))
            {
                row.Selected = true;
            }
        }

        private RegexOptions GetValue()
        {
            return _items.Select(f => f.Value).GetValue();
        }

        private void SetAll(object sender, EventArgs e)
        {
            SetAll();
        }

        public void SetAll()
        {
            _manager.Value = GetValue();
        }

        private void SetNone(object sender, EventArgs e)
        {
            _manager.SetNone();
        }

        public void SetAllOrNone()
        {
            if (_items.All(f => f.Enabled))
            {
                _manager.SetNone();
            }
            else
            {
                _manager.Value = GetValue();
            }
        }

        private void SetSelectedAllOrNone()
        {
            RegexOptionsItem[] selected = DataGridView.EnumerateSelectedItems().ToArray();
            RegexOptions value = selected.Select(f => f.Value).GetValue();

            if (selected.All(f => f.Enabled))
            {
                _manager.RemoveOptions(value);
            }
            else
            {
                _manager.AddOptions(value);
            }
        }

        public void ToggleItemEnabled(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (index < DataGridView.Rows.Count)
                ((RegexOptionsItem)DataGridView.Rows[index].DataBoundItem).ToggleEnabled();
        }

        private ContextMenuStrip CreateContextMenuStrip()
        {
            var cms = new ContextMenuStrip();
            cms.Items.AddRange(CreateContextMenuStripItems(cms).ToArray());
            return cms;
        }

        private IEnumerable<ToolStripItem> CreateContextMenuStripItems(ContextMenuStrip cms)
        {
            yield return new ToolStripMenuItem(Resources.SetAll, Resources.IcoCheckAll.ToBitmap(), (f, f2) => SetAll(f, f2));
            yield return new ToolStripMenuItem(
                Resources.SetNone,
                Resources.IcoUncheckAll.ToBitmap(),
                (f, f2) => SetNone(f, f2));
            yield return new ToolStripSeparator();

            foreach (ToolStripItem item in CreateVisibilityItems())
                yield return item;

            yield return new ToolStripSeparator();

            foreach (ToolStripItem item in CreateColumnVisibilityItems(cms))
                yield return item;
        }

        public IEnumerable<ToolStripItem> CreateToolStripItems(ToolStripMenuItem parent)
        {
            var allItem = new ToolStripMenuItem(Resources.SetAll, null, (f2, f3) => SetAll(f2, f3));
            var noneItem = new ToolStripMenuItem(Resources.SetNone, null, (f2, f3) => SetNone(f2, f3));

            if (_items.All(f => f.Enabled))
            {
                noneItem.ShortcutKeyDisplayString = Resources.CtrlShiftTilde;
            }
            else
            {
                allItem.ShortcutKeyDisplayString = Resources.CtrlShiftTilde;
            }

            yield return allItem;
            yield return noneItem;
            yield return new ToolStripSeparator();

            foreach (ToolStripItem item in CreateVisibilityItems())
                yield return item;

            yield return new ToolStripSeparator();

            foreach (ToolStripItem item in CreateColumnVisibilityItems(parent.DropDown))
                yield return item;

            var tseToggle = new ToolStripSeparator();
            var tsiToggle = new ToolStripMenuItem(Resources.ToggleOption, null, new ToolStripSeparator());
            tsiToggle.DropDownOpening += (object sender, EventArgs e) => tsiToggle.DropDownItems
                .LoadItems(CreateToggleToolStripItems());
            parent.DropDownOpening += delegate
            {
                bool flg = _items.Count > 0;
                tseToggle.Visible = flg;
                tsiToggle.Visible = flg;

                if (flg && tsiToggle.DropDownItems.Count == 0)
                    tsiToggle.DropDownItems.Add(new ToolStripSeparator());
            };

            yield return tseToggle;
            yield return tsiToggle;
        }

        private IEnumerable<ToolStripMenuItem> CreateToggleToolStripItems()
        {
            int i = 1;
            foreach (RegexOptionsItem item in _items)
            {
                yield return new ToolStripMenuItem(item.Text, null, delegate { item.ToggleEnabled(); })
                {
                    Checked = item.Enabled,
                    ShortcutKeyDisplayString = GetShorcutKeyDisplayString(i)
                };
                i++;
            }
        }

        private static string GetShorcutKeyDisplayString(int index)
        {
            switch (index)
            {
                case 1:
                    return Resources.CtrlShift1;
                case 2:
                    return Resources.CtrlShift2;
                case 3:
                    return Resources.CtrlShift3;
                case 4:
                    return Resources.CtrlShift4;
                case 5:
                    return Resources.CtrlShift5;
                case 6:
                    return Resources.CtrlShift6;
                case 7:
                    return Resources.CtrlShift7;
                case 8:
                    return Resources.CtrlShift8;
                case 9:
                    return Resources.CtrlShift9;
                default:
                    return "";
            }
        }

        public IEnumerable<ToolStripItem> CreateVisibilityItems()
        {
            var item = new ToolStripMenuItem(Resources.SetVisibility, null, new ToolStripMenuItem());
            item.DropDownOpening += delegate
            {
                item.DropDownItems.Clear();
                item.DropDownItems.AddRange(_manager.Items
                    .Select(f => new ToolStripMenuItem(f.Text, null, delegate { f.Visible = !f.Visible; })
                        {
                            Checked = f.Visible,
                            CheckOnClick = true
                        })
                    .ToArray());
            };
            yield return item;
            yield return new ToolStripMenuItem(Resources.SetAllVisible, null, delegate { _manager.SetAllVisible(); });
            yield return new ToolStripMenuItem(
                Resources.SetDefaultVisibility,
                null,
                delegate { _manager.SetVisible(RegexOptionsManager.DefaultVisibleOptions); });
        }

        private void DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Modifiers)
            {
                case Keys.None:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.Enter:
                            case Keys.Space:
                                {
                                    SetSelectedAllOrNone();
                                    break;
                                }
                        }

                        break;
                    }
                case Keys.Control:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.Z:
                                {
                                    History.UndoIfCan();
                                    break;
                                }
                            case Keys.Y:
                                {
                                    History.RedoIfCan();
                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        private void DataGridView_CellContextMenuStripNeeded(
            object sender,
            DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            var cms = new ContextMenuStrip();

            if (e.RowIndex != -1)
            {
                cms.Items.AddRange(
                    CreateCellToolStripItems((RegexOptionsItem)DataGridView.Rows[e.RowIndex].DataBoundItem, cms).ToArray());
            }

            e.ContextMenuStrip = cms;
        }

        private IEnumerable<ToolStripItem> CreateCellToolStripItems(RegexOptionsItem item, ContextMenuStrip cms)
        {
            yield return new ToolStripMenuItem(Resources.Visible, null, delegate { item.Visible = true; })
            {
                Checked = item.Visible
            };
            yield return new ToolStripMenuItem(Resources.Hidden, null, delegate { item.Visible = false; })
            {
                Checked = !item.Visible
            };
            yield return new ToolStripSeparator();
            yield return new ToolStripMenuItem(Resources.SetAll, Resources.IcoCheckAll.ToBitmap(), (f, f2) => SetAll(f, f2));
            yield return new ToolStripMenuItem(
                Resources.SetNone,
                Resources.IcoUncheckAll.ToBitmap(),
                (f, f2) => SetNone(f, f2));
            yield return new ToolStripSeparator();

            foreach (ToolStripItem tsi in CreateVisibilityItems())
                yield return tsi;

            yield return new ToolStripSeparator();

            foreach (ToolStripItem tsi in CreateColumnVisibilityItems(cms))
                yield return tsi;
        }

        private IEnumerable<ToolStripItem> CreateColumnVisibilityItems(ToolStripDropDown dropDown)
        {
            var tsiShowHotkeyNumber = new ToolStripMenuItem(
                Resources.ShowHotkeyNumber,
                null,
                (object sender, EventArgs e)
                    => DataGridView.HotkeyNumberColumnVisible = !DataGridView.HotkeyNumberColumnVisible)
            {
                Checked = DataGridView.HotkeyNumberColumnVisible
            };

            var tsiShowDescription = new ToolStripMenuItem(
                Resources.ShowDescription,
                null,
                (object sender, EventArgs e)
                    => DataGridView.DescriptionColumnVisible = !DataGridView.DescriptionColumnVisible)
            {
                Checked = DataGridView.DescriptionColumnVisible
            };

            dropDown.Opening += (object sender, CancelEventArgs e) =>
            {
                tsiShowHotkeyNumber.Checked = DataGridView.HotkeyNumberColumnVisible;
                tsiShowDescription.Checked = DataGridView.DescriptionColumnVisible;
            };

            yield return tsiShowHotkeyNumber;
            yield return tsiShowDescription;
        }

        private void DataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1
                && e.ColumnIndex != -1
                && !(DataGridView.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn))
            {
                ((RegexOptionsItem)DataGridView.Rows[e.RowIndex].DataBoundItem).ToggleEnabled();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DataGridView.Dispose();
                    DataGridView = null;
                }

                _disposed = true;
            }
        }

        public RegexOptionsDataGridView DataGridView { get; private set; } = new RegexOptionsDataGridView();

        public CommandHistory History
        {
            get { return _manager.History; }
        }

        private readonly RegexOptionsManager _manager;
        private readonly BindingList<RegexOptionsItem> _items = new BindingList<RegexOptionsItem>();
        private bool _disposed;
    }
}
