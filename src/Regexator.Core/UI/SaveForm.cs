// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;
using Regexator.FileSystem;

namespace Regexator.UI
{
    public partial class SaveForm : Form
    {
        internal SaveForm(SaveExecutor executor)
        {
            if (executor == null)
                throw new ArgumentNullException(nameof(executor));

            InitializeComponent();
            Text = Resources.Regexator;
            Icon = Resources.RegexatorIcon;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            AcceptButton = _btnYes;
            CancelButton = _btnCancel;
            _btnYes.Text = Resources.Yes;
            _btnNo.Text = Resources.No;
            _btnCancel.Text = Resources.Cancel;
            _lblQuestion.Text = Resources.SaveChangesToFollowingItemsMsg;
            _btnYes.Click += (f, f2) => YesButton_Click(f, f2);
            _btnNo.Click += (f, f2) => NoButton_Click(f, f2);
            _btnCancel.Click += (f, f2) => CancelButton_Click(f, f2);
            _btnYes.Select();
            _trv.Load(executor);
        }

        protected override void OnLoad(EventArgs e)
        {
            ActiveControl = _btnYes;
            base.OnLoad(e);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void NoButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        private void YesButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }
    }
}
