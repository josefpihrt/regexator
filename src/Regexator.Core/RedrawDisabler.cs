// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;
using Regexator.Windows.Forms;

namespace Regexator
{
    public sealed class RedrawDisabler : IDisposable
    {
        private bool _disposed;
        private readonly Control _control;

        public RedrawDisabler(Control control)
        {
            _control = control ?? throw new ArgumentNullException(nameof(control));
            _control.BeginUpdate();
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
                    _control.EndUpdate();

                _disposed = true;
            }
        }
    }
}