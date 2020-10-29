// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public class SelectionMemento : IDisposable
    {
        private bool _disposed;
        private TextBoxTextSpan _span;

        public SelectionMemento(TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            _span = new TextBoxTextSpan(box);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _span.Select();
                    _span = null;
                }

                _disposed = true;
            }
        }
    }
}
