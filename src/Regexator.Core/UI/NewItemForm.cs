// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Windows.Forms;
using Regexator.Windows.Forms;

namespace Regexator.UI
{
    public partial class NewItemForm : Form
    {
        internal NewItemForm(NewItemCommand[] commands)
        {
            _commands = commands ?? throw new ArgumentNullException(nameof(commands));
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            ShowInTaskbar = false;
            Icon = Resources.RegexatorIcon;
            Text = Resources.NewItem;
            KeyPreview = true;
            AcceptButton = _btnOk;
            CancelButton = _btnCancel;
            _lsv.View = View.Details;
            _lsv.Columns.Add("", -1);
            _lsv.HeaderStyle = ColumnHeaderStyle.None;
            _lsv.MultiSelect = false;
            _lsv.ShowItemToolTips = true;
            _lsv.KeyDown += (f, f2) => ListView_KeyDown(f, f2);
            _lsv.MouseDoubleClick += (f, f2) => ListView_MouseDoubleClick(f, f2);
            _btnOk.Text = Resources.OK;
            _btnCancel.Text = Resources.Cancel;
            _btnOk.Click += (object sender, EventArgs e) => CloseOk();
            _btnCancel.Click += (object sender, EventArgs e) => Close();
        }

        private void ListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = _lsv.GetItemAt(e.X, e.Y);
            if (item != null)
                CloseOk();
        }

        private void ListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None)
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        {
                            DialogResult = DialogResult.OK;
                            break;
                        }
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadItems();
            _lsv.Select();
            _lsv.TrySelectFirstItem();
            _lsv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            base.OnLoad(e);
        }

        private void CloseOk()
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void LoadItems()
        {
            var iml = new ImageList();
            int i = 0;
            foreach (NewItemCommand command in _commands)
            {
                var item = new ListViewItem(command.Name) { Tag = command, ImageIndex = i };
                _lsv.Items.Add(item);
                iml.Images.Add(command.Icon);
                i++;
            }

            _lsv.SmallImageList = iml;
        }

        public NewItemCommand SelectedCommand
        {
            get
            {
                return _lsv.SelectedItems.Cast<ListViewItem>().Select(f => f.Tag).Cast<NewItemCommand>()
                    .FirstOrDefault();
            }
        }

        private readonly NewItemCommand[] _commands;
    }
}
