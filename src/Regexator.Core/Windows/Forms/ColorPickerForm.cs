// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public sealed class ColorPickerForm : Form
    {
        public ColorPickerForm()
        {
            StartPosition = FormStartPosition.CenterParent;
            Text = "Color Picker";
            KeyPreview = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MinimizeBox = false;
            MaximizeBox = false;
            AutoSize = true;
            ShowInTaskbar = false;

            CloseOnEscapeKey = true;
        }

        protected override void OnShown(EventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (InitialButton != null)
            {
                Point pt = PointToScreen(InitialButton.Location);
                pt.Offset(InitialButton.Width / 2, InitialButton.Height / 2);
                Cursor.Position = pt;
            }

            base.OnShown(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData & Keys.KeyCode) == Keys.Tab)
                return true;

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            switch (e.Modifiers)
            {
                case Keys.None:
                    {
                        {
                            switch (e.KeyCode)
                            {
                                case Keys.Escape:
                                    {
                                        {
                                            if (CloseOnEscapeKey)
                                                Close();
                                        }

                                        break;
                                    }
                            }
                        }

                        break;
                    }
            }

            base.OnKeyDown(e);
        }

        public bool CloseOnEscapeKey { get; set; }
        public Button InitialButton { get; set; }
    }
}
