// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;

namespace Regexator.Snippets
{
    public class SenseTextBox : TextBox
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.Y || e.KeyCode == Keys.Z)
                    e.Handled = true;
            }

            base.OnKeyDown(e);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if ((int)keyData == 9 || (int)keyData == 65545) // tab || shift+tab
                return true;

            return base.IsInputKey(keyData);
        }
    }
}
