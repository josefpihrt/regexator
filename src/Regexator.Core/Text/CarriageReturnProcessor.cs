// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Regexator.Output;

namespace Regexator.Text
{
    public class CarriageReturnProcessor : IDisposable
    {
        private StringReader _reader;
        private readonly RegexBuilder _builder;
        private int _pos;
        private int _countCr;
        private bool _disposed;

        public CarriageReturnProcessor(RegexBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
            _reader = new StringReader(_builder.Input);
        }

        public static void ProcessBlocks(RegexBuilder builder)
        {
            using (var processor = new CarriageReturnProcessor(builder))
            {
                var matchBuilder = builder as MatchBuilder;
                if (matchBuilder?.GroupSettings.IsZeroIgnored == false)
                {
                    foreach (IGrouping<int, CaptureBlock> blocks in matchBuilder.CaptureBlocks
                        .GroupBy(f => f.MatchItemIndex))
                    {
                        CaptureBlock block = blocks.FirstOrDefault(f => f.IsDefaultCapture);
                        Debug.Assert(block != null);
                        if (block != null)
                        {
                            int index = processor.OffsetIndex(block.ValueIndex);
                            processor.Process(blocks.Where(f => !f.IsDefaultCapture));
                            int endIndex = processor.OffsetIndex(block.ValueIndex + block.ValueLength);
                            block.InputSpan = new TextSpan(index, endIndex - index);
                        }
                        else
                        {
                            processor.Process(blocks);
                        }
                    }
                }
                else
                {
                    processor.Process(builder.Blocks);
                }
            }
        }

        private void Process(IEnumerable<RegexBlock> blocks)
        {
            foreach (RegexBlock block in blocks.OrderBy(f => f.ValueIndex).ThenBy(f => f.ValueLength))
            {
                int index = OffsetIndex(block.ValueIndex);
                int endIndex = OffsetIndex(block.ValueIndex + block.ValueLength);
                block.InputSpan = new TextSpan(index, endIndex - index);
            }
        }

        private int OffsetIndex(int index)
        {
            ReadToIndex(index);
            index -= _countCr;
            return index;
        }

        private void ReadToIndex(int index)
        {
            while (_pos < index)
            {
                switch (_reader.Read())
                {
                    case 13:
                        {
                            _countCr++;
                            break;
                        }
                    case -1:
                        return;
                }

                _pos++;
            }
        }

        public void Close()
        {
            Dispose();
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
                    _reader.Dispose();
                    _reader = null;
                }

                _disposed = true;
            }
        }
    }
}
