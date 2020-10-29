// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;
using Regexator.Collections.Generic;

namespace Regexator.UI
{
    public partial class ErrorInfoForm : Form
    {
        public static void ShowError(ErrorInfo item, string message)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            ShowErrors(new ErrorInfo[] { item }, message);
        }

        public static void ShowErrors(ErrorInfo[] items, string message)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (items.Length > 0)
            {
                using (var frm = new ErrorInfoForm(items))
                {
                    frm.Message = message;
                    frm.ShowDialog();
                }
            }
        }

        public ErrorInfoForm(ErrorInfo[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            InitializeComponent();
            Text = Resources.Regexator;
            Icon = Resources.RegexatorIcon;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            AcceptButton = _btnOK;
            KeyPreview = true;
            _btnOK.Text = Resources.OK;
            _btnOK.Click += (f, f2) => YesButton_Click(f, f2);

            _dgv = new ErrorInfoDataGridView();
            _pnl.Controls.Add(_dgv);
            _dgv.DataSource = new SortableBindingList<ErrorInfo>(items);
        }

        private void YesButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ActiveControl = _btnOK;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Modifiers == Keys.None
                && e.KeyCode == Keys.Escape)
            {
                Close();
            }

            base.OnKeyDown(e);
        }

        public string Message
        {
            get { return _lblMessage.Text; }
            set { _lblMessage.Text = value; }
        }

        private readonly ErrorInfoDataGridView _dgv;
    }
}
