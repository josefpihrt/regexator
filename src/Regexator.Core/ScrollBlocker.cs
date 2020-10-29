// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;
using Regexator.Text;
using Regexator.Windows.Forms;

namespace Regexator
{
    public class ScrollBlocker : IDisposable
    {
        private readonly bool _focused;
        private readonly RichTextBox _rtb;
        private bool _disposed;

        public ScrollBlocker(RichTextBox rtb)
            : this(rtb, false)
        {
        }

        public ScrollBlocker(RichTextBox rtb, bool autoRestore)
        {
            _rtb = rtb ?? throw new ArgumentNullException(nameof(rtb));
            AutoRestore = autoRestore;
            TextSpan = new TextBoxTextSpan(_rtb);
            _focused = rtb.Focused;
            if (_focused)
            {
                Form frm = _rtb.FindForm();
                if (frm != null)
                    frm.ActiveControl = null;
            }

            NativeMethods.SendMessage(rtb.Handle, NativeMethods.EM_HIDESELECTION, (IntPtr)1, IntPtr.Zero);
            _rtb.BeginUpdate();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (AutoRestore)
                        Restore();

                    _rtb.EndUpdate();

                    if (_focused)
                        _rtb.Focus();
                }

                _disposed = true;
            }
        }

        public void Restore()
        {
            Restore(TextSpan);
        }

        public void Restore(TextSpan textSpan)
        {
            if (textSpan == null)
                throw new ArgumentNullException(nameof(textSpan));

            NativeMethods.SendMessage(_rtb.Handle, NativeMethods.EM_HIDESELECTION, IntPtr.Zero, IntPtr.Zero);
            _rtb.Select(textSpan);
        }

        public TextSpan TextSpan { get; }

        public bool AutoRestore { get; set; }
    }
}
