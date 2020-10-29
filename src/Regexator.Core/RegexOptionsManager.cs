// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Regexator.Text;

namespace Regexator
{
    public sealed class RegexOptionsManager
    {
        public RegexOptionsManager()
        {
            History = new CommandHistory();
            Items = new RegexOptionsCollection();
            _lst = new BindingList<RegexOptionsItem>(Items);
            _lst.ListChanged += (f, f2) => Items_ListChanged(f, f2);
        }

        private void Items_ListChanged(object sender, ListChangedEventArgs e)
        {
            Debug.Assert(e.ListChangedType == ListChangedType.ItemChanged);
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                switch (e.PropertyDescriptor.Name)
                {
                    case "Enabled":
                        {
                            OnValueChanged(EventArgs.Empty);
                            break;
                        }
                    case "Visible":
                        {
                            OnItemVisibleChanged(EventArgs.Empty);
                            break;
                        }
                }
            }
        }

        public bool HasOptions(RegexOptions value)
        {
            return Value.Contains(value);
        }

        public void AddOptions(RegexOptions options)
        {
            Value = Value.Union(options);
        }

        public void RemoveOptions(RegexOptions options)
        {
            Value = Value.Except(options);
        }

        public void SetNone()
        {
            Value = RegexOptions.None;
        }

        public void ClearHistory()
        {
            History.Clear();
        }

        private void OnValueChanged(EventArgs e)
        {
            if (!History.IsExecuting)
            {
                RegexOptions value = GetOptions();
                History.AddCommand(new Command(() => Value = value, RegexOptionsUtility.Format(value)));
            }

            ValueChanged?.Invoke(this, e);
        }

        private void OnItemVisibleChanged(EventArgs e)
        {
            ItemVisibleChanged?.Invoke(this, e);
        }

        public void SetAllVisible()
        {
            SetVisible(RegexOptionsUtility.Value);
        }

        public void SetVisible(RegexOptions value)
        {
            RegexOptions options = VisibleOptions;
            _lst.RaiseListChangedEvents = false;
            Items.ForEach((f) => f.Visible = value.Contains(f.Value));
            _lst.RaiseListChangedEvents = true;

            if (options != VisibleOptions)
                OnItemVisibleChanged(EventArgs.Empty);
        }

        private RegexOptions GetOptions(Func<RegexOptionsItem, bool> predicate)
        {
            return Items.Where(predicate).Select(f => f.Value).GetValue();
        }

        public RegexOptions GetOptions()
        {
            return GetOptions(f => f.Enabled);
        }

        public RegexOptions VisibleOptions
        {
            get { return GetOptions(f => f.Visible); }
        }

        public RegexOptions Value
        {
            get { return GetOptions(); }
            set
            {
                RegexOptions options = GetOptions();
                _lst.RaiseListChangedEvents = false;
                Items.ForEach((f) => f.Enabled = value.Contains(f.Value));
                _lst.RaiseListChangedEvents = true;

                if (options != GetOptions())
                    OnValueChanged(EventArgs.Empty);
            }
        }

        public RegexOptionsCollection Items { get; }

        public CommandHistory History { get; }

        private readonly BindingList<RegexOptionsItem> _lst;

        public event EventHandler ValueChanged;
        public event EventHandler ItemVisibleChanged;

        public static readonly RegexOptions DefaultVisibleOptions = RegexOptions.ExplicitCapture
            | RegexOptions.IgnoreCase
            | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Multiline
            | RegexOptions.Singleline;
    }
}
