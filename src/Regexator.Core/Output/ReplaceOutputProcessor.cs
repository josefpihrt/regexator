// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Regexator.Text;
using Regexator.Text.RegularExpressions;

namespace Regexator.Output
{
    internal sealed class ReplaceOutputProcessor : IDisposable
    {
        public ReplaceBlock[] CreateBlocks(ReplaceBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            using (_processor = new TextProcessor(builder.Data.Output, builder.Settings) { RemoveEndingLinefeed = true })
            {
                ReplaceBlock[] blocks = builder.ReplaceItems.Select(f => CreateBlock(f)).ToArray();
                _processor.ReadToEnd();
                return blocks;
            }
        }

        private ReplaceBlock CreateBlock(ReplaceItem item)
        {
            _processor.ReadTo(item.Result.Index);
            int index = _processor.Length;
            int symbolCount = _processor.SymbolSpans.Count;
            _processor.ReadTo(item.Result.EndIndex);
            return new ReplaceBlock(
                item,
                new TextSpan(index, _processor.Length - index),
                _processor.EnumerateSymbolsFrom(symbolCount).ToArray());
        }

        public override string ToString()
        {
            return _processor.ToString();
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
                if (disposing && _processor != null)
                {
                    _processor.Dispose();
                    _processor = null;
                }

                _disposed = true;
            }
        }

        public ReadOnlyCollection<TextSpan> SymbolSpans
        {
            get { return _processor.SymbolSpans; }
        }

        private TextProcessor _processor;
        private bool _disposed;
    }
}
