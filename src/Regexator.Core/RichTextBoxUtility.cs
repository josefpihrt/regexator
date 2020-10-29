// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Regexator.Windows.Forms;

namespace Regexator
{
    public static class RichTextBoxUtility
    {
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void ResetZoom(RichTextBox rtb)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            NativeMethods.SendMessage(rtb.Handle, NativeMethods.EM_SETZOOM, IntPtr.Zero, IntPtr.Zero);
        }

        public static bool EscapeSelection(TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            string escaped = string.Join("\n", box.SelectedText.Split(new[] { '\n' }).Select(f => Regex.Escape(f)));
            if (escaped != box.SelectedText)
            {
                box.SelectedText = escaped;
                return true;
            }

            return false;
        }

        public static bool UnescapeSelection(TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            string unescaped = Regex.Unescape(box.SelectedText);
            if (unescaped != box.SelectedText)
            {
                box.SelectedText = unescaped;
                return true;
            }

            return false;
        }
    }
}
