// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public class ExtendedTextBox : TextBox
    {
        public ExtendedTextBox()
        {
            BorderStyle = BorderStyle.FixedSingle;
            ContextMenuStrip = new ContextMenuStrip();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            switch (e.Modifiers)
            {
                case Keys.None:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.Escape:
                                {
                                    if (!ReadOnly)
                                        Clear();

                                    break;
                                }
                        }

                        break;
                    }
                case Keys.Control:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.A:
                                {
                                    SelectAll();
                                    break;
                                }
                        }

                        break;
                    }
            }

            base.OnKeyDown(e);
        }
    }
}