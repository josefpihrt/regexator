// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Regexator.Collections.Generic;
using Regexator.Output;

namespace Regexator.Text
{
    public class InputTextProcessor
    {
        public InputTextProcessor(string input, OutputSettings settings, IEnumerable<TextSpan> textSpans)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (textSpans == null)
                throw new ArgumentNullException(nameof(textSpans));

            using (_processor = new TextProcessor(input, settings) { RemoveEndingLinefeed = true })
            {
                _symbols = new List<TextSpan>();
                TextSpans = textSpans
                    .OrderBy(f => f.Index)
                    .Select(f => GetTextSpan(f))
                    .ToReadOnly();
                SymbolSpans = _symbols.AsReadOnly();
                _processor.ReadToEnd();
                Text = _processor.ToString();
            }
        }

        private TextSpan GetTextSpan(TextSpan textSpan)
        {
            _processor.ReadTo(textSpan.Index);
            int index = _processor.Length;
            int symbolCount = _processor.SymbolSpans.Count;
            _processor.ReadTo(textSpan.EndIndex);
            _symbols.AddRange(_processor.EnumerateSymbolsFrom(symbolCount));
            return new TextSpan(index, _processor.Length - index);
        }

        public string Text { get; }

        public ReadOnlyCollection<TextSpan> TextSpans { get; }

        public ReadOnlyCollection<TextSpan> SymbolSpans { get; }

        private readonly TextProcessor _processor;
        private readonly List<TextSpan> _symbols;
    }
}
