// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Regexator.Text;

namespace Regexator
{
    [DebuggerDisplay("{Value}, Enabled: {Enabled}")]
    public class RegexOptionsItem : INotifyPropertyChanged
    {
        internal RegexOptionsItem(RegexOptions value)
            : this(value, "", true)
        {
        }

        internal RegexOptionsItem(RegexOptions value, string description, bool defaultVisible)
        {
            Value = value;
            Text = TextUtility.SplitCamelCase(value);
            Description = description;
            Visible = defaultVisible;
        }

        public void SetValuesFrom(RegexOptionsItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            Enabled = item.Enabled;
            Visible = item.Visible;
            Description = item.Description;
        }

        public void ToggleEnabled()
        {
            Enabled = !Enabled;
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (value != _enabled)
                {
                    _enabled = value;
                    NotifyPropertyChanged(nameof(Enabled));
                }
            }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (value != _visible)
                {
                    _visible = value;
                    NotifyPropertyChanged(nameof(Visible));
                }
            }
        }

        public string Text { get; }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value != _description)
                {
                    _description = value ?? "";
                    NotifyPropertyChanged(nameof(Description));
                }
            }
        }

        public RegexOptions Value { get; }

        private bool _enabled;
        private bool _visible;
        private string _description;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
