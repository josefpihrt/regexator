// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Regexator.UI
{
    public partial class ProjectDeleteForm : Form
    {
        public ProjectDeleteForm()
        {
            InitializeComponent();
            Text = Resources.Regexator;
            Icon = Resources.RegexatorIcon;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            CancelButton = _btnCancel;
            _btnOk.Text = Resources.OK;
            _btnCancel.Text = Resources.Cancel;
            _btnOk.Click += (f, f2) => OkButton_Click(f, f2);
            _btnCancel.Click += (f, f2) => CancelButton_Click(f, f2);
            _pbxIcon.Image = SystemIcons.Warning.ToBitmap();
            _cbxDeleteAllFileInputs.Text = Resources.DeleteAllFileInputs;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ActiveControl = _btnOk;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        public string Message
        {
            get { return _lblMessage.Text; }
            set { _lblMessage.Text = value; }
        }

        public bool DeleteAllFileInputs
        {
            get { return _cbxDeleteAllFileInputs.Checked; }
            set { _cbxDeleteAllFileInputs.Checked = value; }
        }

        public bool DeleteAllFileInputsEnabled
        {
            get { return _cbxDeleteAllFileInputs.Visible; }
            set { _cbxDeleteAllFileInputs.Visible = value; }
        }
    }
}
