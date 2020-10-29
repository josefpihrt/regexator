// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public static class ControlExtensions
    {
        public static Control FindFocusedControl(this Control control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            if (control.Focused)
                return control;

            if (control.ContainsFocus)
            {
                foreach (Control child in control.Controls)
                {
                    if (child.Focused)
                    {
                        return child;
                    }
                    else if (child.ContainsFocus)
                    {
                        return FindFocusedControl(child);
                    }
                }
            }

            return null;
        }

        public static TControl FindParent<TControl>(this Control control) where TControl : Control
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            Control parent = control.Parent;

            while (parent != null)
            {
                if (parent is TControl result)
                    return result;

                parent = parent.Parent;
            }

            return null;
        }

        public static IEnumerable<TControl> EnumerateAllControls<TControl>(this Control control) where TControl : Control
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            return EnumerateAllControls<TControl>(control, false);
        }

        private static IEnumerable<TControl> EnumerateAllControls<TControl>(
            this Control control,
            bool include) where TControl : Control
        {
            if (include && control is TControl c)
            {
                yield return c;
            }

            foreach (Control item in control.Controls)
            {
                foreach (TControl item2 in EnumerateAllControls<TControl>(item, true))
                    yield return item2;
            }
        }

        public static void BeginUpdate(this Control control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            NativeMethods.SendMessage(control.Handle, NativeMethods.WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        }

        public static void EndUpdate(this Control control)
        {
            EndUpdate(control, true);
        }

        public static void EndUpdate(this Control control, bool refresh)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            NativeMethods.SendMessage(control.Handle, NativeMethods.WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);

            if (refresh)
                control.Refresh();
        }
    }
}
