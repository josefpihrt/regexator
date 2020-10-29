// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Regexator.Collections.Generic;
using Regexator.Text.RegularExpressions;

namespace Regexator.Text
{
    public class TextSearcher
    {
        private readonly ITextContainer _container;
        private string _searchPhrase;
        private string _replacementPhrase;
        private bool _matchWholeWord;
        private bool _matchCase;
        private bool _replaceEnabled;
        private Regex _regex;
        private string _text;
        private int _firstMatchIndex;
        private Match _match;
        private SearcherState _state;
        private int _count;
        private int _offset;
        private readonly Dictionary<string, DateTime> _searchHistory;
        private readonly Dictionary<string, DateTime> _replaceHistory;

        public event EventHandler SearchPhraseChanged;
        public event EventHandler ReplacementPhraseChanged;
        public event EventHandler MatchWholeWordChanged;
        public event EventHandler MatchCaseChanged;
        public event EventHandler Reset;
        public event EventHandler ReplaceEnabledChanged;
        public event EventHandler ReplacingAll;
        public event EventHandler ReplacedAll;

        public TextSearcher(ITextContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _searchPhrase = "";
            _replacementPhrase = "";
            _searchHistory = new Dictionary<string, DateTime>();
            _replaceHistory = new Dictionary<string, DateTime>();
            _replaceEnabled = true;
        }

        public FindResult FindNext()
        {
            return FindNext(false);
        }

        public FindResult ReplaceNext()
        {
            return FindNext(true);
        }

        private FindResult FindNext(bool replace)
        {
            if (State == SearcherState.Finished)
                OnReset(EventArgs.Empty);

            if (State == SearcherState.None)
            {
                _text = _container.Text;
                State = SearcherState.ToEnd;
            }

            if (IsValidSearchPhrase)
            {
                UpdateHistory(replace);

                if (_match == null)
                {
                    FindFirst(replace);
                }
                else
                {
                    if (replace)
                        ReplaceSelectedText();

                    _match = _match.NextMatch();

                    if (!_match.Success && State != SearcherState.FromBeginning)
                    {
                        State = SearcherState.FromBeginning;
                        _match = Find();
                    }
                }

                if (_match.Success
                    && (State != SearcherState.FromBeginning || _firstMatchIndex == -1 || _match.Index < _firstMatchIndex))
                {
                    Debug.WriteLine(string.Format(
                        CultureInfo.CurrentCulture,
                        "match found: {0}, I:{1}, L:{2}",
                        _match.Value,
                        _match.Index,
                        _match.Length));
                    DoWithoutReset(() => _container.SelectText(_match.Index + _offset, _match.Length));
                    _count++;
                    return FindResult.Success;
                }

                State = SearcherState.Finished;
                return (_count > 0) ? FindResult.Finished : FindResult.NoSuccess;
            }

            return FindResult.NoSearchPhrase;
        }

        private void FindFirst(bool replace)
        {
            if (replace)
            {
                _match = Find(_container.SelectionStart, _container.SelectionLength);
                if (_match.Success
                    && _match.Index == _container.SelectionStart
                    && _match.Length == _container.SelectionLength)
                {
                    ReplaceSelectedText();
                    _match = Find(_container.SelectionStart);
                }
                else
                {
                    _match = null;
                }
            }

            if (_match?.Success != true)
                _match = Find(SelectionEnd);

            if (_match.Success)
            {
                _firstMatchIndex = _match.Index;
            }
            else
            {
                State = SearcherState.FromBeginning;
                _match = Find();
            }
        }

        public int ReplaceAll()
        {
            if (ReplaceEnabled && IsValidSearchPhrase)
            {
                ResetEnabled = false;
                UpdateHistory();
                ReplaceItemCollection items = Regex.ReplaceItems(_container.Text, ReplacementPhrase);
                if (items.Count > 0)
                {
                    OnReplacingAll(EventArgs.Empty);
                    ReplaceAll(items, SelectionEnd);
                    OnReplacedAll(EventArgs.Empty);
                }

                ResetEnabled = true;
                OnReset(EventArgs.Empty);
                return items.Count;
            }

            return 0;
        }

        protected virtual void ReplaceAll(ReplaceItemCollection items, int selectionEnd)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (ReplaceItem item in items)
            {
                _container.SelectText(item.Result.Index, item.Match.Length);
                ReplaceSelectedText();
            }

            ReplaceItem item2 = items.FirstOrDefault(f => (f.MatchEndIndex) >= selectionEnd)
                ?? items.Reversed().FirstOrDefault(f => (f.MatchEndIndex) < selectionEnd);

            if (item2 != null)
                _container.SelectText(item2.Result.EndIndex, 0);
        }

        private void ReplaceSelectedText()
        {
            if (ReplaceEnabled)
            {
                Debug.WriteLine(string.Format(
                    CultureInfo.CurrentCulture,
                    "'{0}' replaced for '{1}' at {2}",
                    _container.SelectedText,
                    ReplacementPhrase,
                    _container.SelectionStart));
                DoWithoutReset(() => _container.SelectedText = ReplacementPhrase);

                if (_match != null)
                    _offset += ReplacementPhrase.Length - _match.Length;
            }
        }

        private Match Find()
        {
            return this.Regex.Match(_text);
        }

        private Match Find(int beginning)
        {
            return this.Regex.Match(_text, beginning);
        }

        private Match Find(int beginning, int length)
        {
            return this.Regex.Match(_text, beginning, length);
        }

        public IEnumerable<string> EnumerateSearchPhrases()
        {
            foreach (KeyValuePair<string, DateTime> item in _searchHistory.OrderByDescending(f => f.Value))
                yield return item.Key;
        }

        public IEnumerable<string> EnumerateReplacePhrases()
        {
            foreach (KeyValuePair<string, DateTime> item in _replaceHistory.OrderByDescending(f => f.Value))
                yield return item.Key;
        }

        private void UpdateHistory()
        {
            UpdateHistory(true);
        }

        private void UpdateHistory(bool replace)
        {
            UpdateSearchHistory();

            if (replace)
                UpdateReplaceHistory();
        }

        private void UpdateSearchHistory()
        {
            _searchHistory[SearchPhrase] = DateTime.UtcNow;
        }

        private void UpdateReplaceHistory()
        {
            if (!string.IsNullOrEmpty(ReplacementPhrase))
                _replaceHistory[ReplacementPhrase] = DateTime.UtcNow;
        }

        private void DoWithoutReset(Action action)
        {
            ResetEnabled = false;
            action();
            ResetEnabled = true;
        }

        public void ToggleMatchCase()
        {
            MatchCase = !MatchCase;
        }

        public void ToggleMatchWholeWord()
        {
            MatchWholeWord = !MatchWholeWord;
        }

        public string SearchPhrase
        {
            get { return _searchPhrase; }
            set
            {
                string s = value ?? "";
                Debug.Assert(!s.IsMultiline());
                if (_searchPhrase != s)
                {
                    _searchPhrase = s;
                    OnSearchPhraseChanged(EventArgs.Empty);
                }
            }
        }

        public bool IsValidSearchPhrase
        {
            get { return !string.IsNullOrEmpty(SearchPhrase); }
        }

        public string ReplacementPhrase
        {
            get { return _replacementPhrase; }
            set
            {
                string s = value ?? "";
                Debug.Assert(!s.IsMultiline());
                if (_replacementPhrase != s)
                {
                    _replacementPhrase = s;
                    OnReplacementPhraseChanged(EventArgs.Empty);
                }
            }
        }

        public bool MatchWholeWord
        {
            get { return _matchWholeWord; }
            set
            {
                if (_matchWholeWord != value)
                {
                    _matchWholeWord = value;
                    OnMatchWholeWordChanged(EventArgs.Empty);
                }
            }
        }

        public bool MatchCase
        {
            get { return _matchCase; }
            set
            {
                if (_matchCase != value)
                {
                    _matchCase = value;
                    OnMatchCaseChanged(EventArgs.Empty);
                }
            }
        }

        public bool ReplaceEnabled
        {
            get { return _replaceEnabled; }
            set
            {
                if (_replaceEnabled != value)
                {
                    _replaceEnabled = value;
                    OnReplaceEnabledChanged(EventArgs.Empty);
                }
            }
        }

        protected bool ResetEnabled { get; private set; }

        protected virtual void OnSearchPhraseChanged(EventArgs e)
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "search phrase changed: {0}", SearchPhrase));
            _regex = null;

            SearchPhraseChanged?.Invoke(this, e);

            OnReset(EventArgs.Empty);
        }

        protected virtual void OnReplacementPhraseChanged(EventArgs e)
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "replacement phrase changed: {0}", ReplacementPhrase));

            ReplacementPhraseChanged?.Invoke(this, e);
        }

        protected virtual void OnMatchWholeWordChanged(EventArgs e)
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "match whole word changed: {0}", MatchWholeWord));
            _regex = null;

            MatchWholeWordChanged?.Invoke(this, e);

            OnReset(EventArgs.Empty);
        }

        protected virtual void OnMatchCaseChanged(EventArgs e)
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "match case changed: {0}", MatchCase));
            _regex = null;

            MatchCaseChanged?.Invoke(this, e);

            OnReset(EventArgs.Empty);
        }

        protected virtual void OnReplaceEnabledChanged(EventArgs e)
        {
            ReplaceEnabledChanged?.Invoke(this, e);
        }

        protected virtual void OnReplacingAll(EventArgs e)
        {
            Debug.WriteLine("replacing all");

            ReplacingAll?.Invoke(this, e);
        }

        protected virtual void OnReplacedAll(EventArgs e)
        {
            Debug.WriteLine("replaced all");

            ReplacedAll?.Invoke(this, e);
        }

        protected virtual void OnReset(EventArgs e)
        {
            _text = null;
            _match = null;
            _firstMatchIndex = -1;
            State = SearcherState.None;
            _count = 0;
            _offset = 0;
            Debug.WriteLine("reset");

            Reset?.Invoke(this, e);
        }

        private Regex Regex
        {
            get
            {
                if (_regex == null)
                {
                    string pattern = Regex.Escape(SearchPhrase);

                    if (MatchWholeWord)
                        pattern = pattern.Enclose(@"\b");

                    _regex = new Regex(pattern, Options);
                    Debug.WriteLine(
                        string.Format(CultureInfo.CurrentCulture, "regex created: '{0}', options: {1}", _regex, Options));
                }

                return _regex;
            }
        }

        private RegexOptions Options
        {
            get
            {
                var value = RegexOptions.None;

                if (!MatchCase)
                    value |= RegexOptions.IgnoreCase;

                return value;
            }
        }

        private int SelectionEnd
        {
            get { return _container.SelectionStart + _container.SelectionLength; }
        }

        private SearcherState State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;

                    if (_state == SearcherState.FromBeginning)
                        _offset = 0;

                    Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "state changed: {0}", State));
                }
            }
        }
    }
}
