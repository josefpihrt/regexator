// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Regexator.Collections.Generic;
using Regexator.Output;

namespace Regexator.Text
{
    public class TextProcessor : IDisposable
    {
        public TextProcessor(string input, OutputSettings settings)
        {
            Initialize(input, settings, "");
        }

        public TextProcessor(string input, OutputSettings settings, int indentLength)
        {
            if (indentLength < 0)
                throw new ArgumentOutOfRangeException(nameof(indentLength));

            Initialize(input, settings, new string(' ', indentLength));
        }

        public TextProcessor(string input, OutputSettings settings, string indent)
        {
            Initialize(input, settings, indent);
        }

        public void Initialize(string input, OutputSettings settings, string indent)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Indent = indent ?? throw new ArgumentNullException(nameof(indent));
            HasIndent = !string.IsNullOrEmpty(indent);
            _lines = new List<string>();
            _textSpans = new List<TextSpan>();
            TextSpans = _textSpans.AsReadOnly();
            _symbols = new List<TextSpan>();
            SymbolSpans = _symbols.AsReadOnly();
            _sb = new StringBuilder(input.Length);
            _sr = new StringReader(input);
        }

        public void ReadToEnd()
        {
            while (Read())
            {
            }
        }

        public void ReadTo(int position)
        {
            while (_position < position && Read())
            {
            }
        }

        private bool Read()
        {
            int ch = _sr.Read();
            switch (ch)
            {
                case 9:
                    {
                        if (_settings.UseTabSymbol)
                        {
                            AddSymbol(_sb.Length + _offset, _settings.Symbols.Tab.Length);
                            _sb.Append(_settings.Symbols.Tab);
                        }
                        else
                        {
                            _sb.Append('\t');
                        }

                        TabCount++;
                        break;
                    }
                case 10:
                    {
                        if (_settings.UseLinefeedSymbol)
                        {
                            AddSymbol(_sb.Length + _offset, _settings.Symbols.Linefeed.Length);
                            _sb.Append(_settings.Symbols.Linefeed);
                        }

                        if (HasIndent)
                        {
                            AddLine();
                        }
                        else
                        {
                            _sb.Append('\n');
                        }

                        LinefeedCount++;
                        break;
                    }
                case 13:
                    {
                        if (_settings.UseCarriageReturnSymbol)
                        {
                            AddSymbol(_sb.Length + _offset, _settings.Symbols.CarriageReturn.Length);
                            _sb.Append(_settings.Symbols.CarriageReturn);
                        }

                        CarriageReturnCount++;
                        break;
                    }
                case 32:
                    {
                        if (_settings.UseSpaceSymbol)
                        {
                            AddSymbol(_sb.Length + _offset, _settings.Symbols.Space.Length);
                            _sb.Append(_settings.Symbols.Space);
                        }
                        else
                        {
                            _sb.Append(' ');
                        }

                        break;
                    }
                case -1:
                    {
                        if (RemoveEndingLinefeed
                            && _settings.UseLinefeedSymbol
                            && _sb.Length > 0
                            && _sb[_sb.Length - 1] == '\n')
                        {
                            _sb.Remove(_sb.Length - 1, 1);
                        }

                        if (HasIndent)
                        {
                            if (_lines.Count == 0 || !_settings.UseLinefeedSymbol || _sb.Length > 0)
                                AddLine();
                        }
                        else
                        {
                            _textSpans.Add(new TextSpan(0, _sb.Length));
                        }

                        return false;
                    }
                default:
                    {
                        _sb.Append((char)ch);
                        break;
                    }
            }

            _position++;
            return true;
        }

        private void AddSymbol(int index, int length)
        {
            _symbols.Add(new TextSpan(index, length));
        }

        private void AddLine()
        {
            _lines.Add(_sb.ToString());
            _textSpans.Add(new TextSpan(_offset, _sb.Length));
            _offset += _sb.Length + Indent.Length + 1;
            _sb.Clear();
        }

        public override string ToString()
        {
            return (HasIndent) ? string.Join("\n", _lines) : _sb.ToString();
        }

        public string ToString(bool indent)
        {
            return (indent && HasIndent) ? string.Join("\n" + Indent, _lines) : _sb.ToString();
        }

        public IEnumerable<TextSpan> EnumerateSymbolsFrom(int collectionIndex)
        {
            return EnumerateSpansFrom(_symbols, collectionIndex);
        }

        private static IEnumerable<TextSpan> EnumerateSpansFrom(IList<TextSpan> collection, int collectionIndex)
        {
            TextSpan prev = null;
            foreach (TextSpan item in collection.EnumerateFrom(collectionIndex))
            {
                if (prev == null)
                {
                    prev = item;
                }
                else if (prev.EndIndex == item.Index)
                {
                    prev = prev.Extend(item.Length);
                }
                else
                {
                    yield return prev;
                    prev = null;
                    yield return item;
                }
            }

            if (prev != null)
                yield return prev;
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
                    _sr.Dispose();
                    _sr = null;
                }

                _disposed = true;
            }
        }

        public static string ProcessSymbols(string value, OutputSettings settings)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.UseLinefeedSymbol)
            {
                value = value.Replace("\n", settings.Symbols.Linefeed + "\n");

                if (value.Length > 0 && value[value.Length - 1] == '\n')
                    value = value.Remove(value.Length - 1);
            }

            if (settings.UseTabSymbol)
                value = value.Replace("\t", settings.Symbols.Tab);

            return value.Replace("\r", (settings.UseCarriageReturnSymbol) ? settings.Symbols.CarriageReturn : "");
        }

        public int Length
        {
            get { return _sb.Length; }
        }

        public bool Multiline
        {
            get { return LinefeedCount > 0; }
        }

        public int LineCount
        {
            get { return LinefeedCount + 1; }
        }

        public bool HasIndent { get; private set; }

        public bool RemoveEndingLinefeed { get; set; }
        public int LinefeedCount { get; private set; }
        public int CarriageReturnCount { get; private set; }
        public int TabCount { get; private set; }
        public string Input { get; private set; }
        public ReadOnlyCollection<TextSpan> TextSpans { get; private set; }
        public ReadOnlyCollection<TextSpan> SymbolSpans { get; private set; }
        public string Indent { get; private set; }

        private StringReader _sr;
        private StringBuilder _sb;
        private OutputSettings _settings;
        private List<TextSpan> _symbols;
        private List<TextSpan> _textSpans;
        private bool _disposed;
        private List<string> _lines;
        private int _offset;
        private int _position;
    }
}
