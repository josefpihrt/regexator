// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public static class FormExtensions
    {
        public static void SetLocationAndSize(this Form form, FormWindowState state, Point location, Size size)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            form.Size = size;
            var flg = false;

            foreach (Screen s in Screen.AllScreens)
            {
                if (s.WorkingArea.Contains(location))
                {
                    form.Location = location;
                    flg = true;
                    break;
                }
            }

            if (!flg)
            {
                foreach (Screen s in Screen.AllScreens)
                {
                    if (s.WorkingArea.Contains(form.Location))
                    {
                        flg = true;
                        break;
                    }
                }
            }

            if (!flg)
                form.SetDesktopLocation(Screen.PrimaryScreen.WorkingArea.Left, Screen.PrimaryScreen.WorkingArea.Top);

            form.WindowState = state;
        }
    }
}
