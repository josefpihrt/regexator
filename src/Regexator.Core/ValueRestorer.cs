// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator
{
    public sealed class ValueRestorer<T> : IDisposable where T : struct
    {
        private bool _disposed;
        private readonly T _value;
        private readonly Action<T> _setter;

        public ValueRestorer(T value, Action<T> setter)
        {
            _value = value;
            _setter = setter ?? throw new ArgumentNullException(nameof(setter));
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
                    _setter(_value);

                _disposed = true;
            }
        }
    }
}