// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public sealed class ColorPickerTableLayoutPanel : TableLayoutPanel
    {
        public ColorPickerTableLayoutPanel()
        {
            DoubleBuffered = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                    {
                        return MoveUp();
                    }
                case Keys.Right:
                    {
                        bool result = MoveRight();
                        if (result)
                            return true;

                        break;
                    }
                case Keys.Down:
                    {
                        return MoveDown();
                    }
                case Keys.Left:
                    {
                        bool result = MoveLeft();
                        if (result)
                            return true;

                        break;
                    }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private bool MoveUp()
        {
            Control control = GetControl();
            if (control != null)
            {
                TableLayoutPanelCellPosition position = GetCellPosition(control);
                if (position.Row > 0)
                {
                    Control btn = GetControlFromPosition(position.Column, position.Row - 1);
                    btn?.Select();
                }
            }

            return true;
        }

        private bool MoveRight()
        {
            Control control = GetControl();
            if (control != null)
            {
                TableLayoutPanelCellPosition position = GetCellPosition(control);

                if (position.Column == (ColumnCount - 1))
                {
                    return true;
                }
                else if (GetControlFromPosition(position.Column + 1, position.Row) == null)
                {
                    return true;
                }
            }

            return false;
        }

        private bool MoveDown()
        {
            Control control = GetControl();
            if (control != null)
            {
                TableLayoutPanelCellPosition position = GetCellPosition(control);
                if (position.Row < (RowCount - 1))
                {
                    Control btn = GetControlFromPosition(position.Column, position.Row + 1);
                    btn?.Select();
                }
            }

            return true;
        }

        private bool MoveLeft()
        {
            Control control = GetControl();
            if (control != null)
            {
                TableLayoutPanelCellPosition position = GetCellPosition(control);
                if (position.Column == 0)
                    return true;
            }

            return false;
        }

        private Control GetControl()
        {
            Form frm = FindForm();
            if (frm != null)
            {
                Control control = frm.ActiveControl;
                if (control != null && control is Button)
                    return control;
            }

            return null;
        }
    }
}
