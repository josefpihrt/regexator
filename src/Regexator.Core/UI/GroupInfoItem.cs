// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel;
using System.Diagnostics;
using Regexator.Text.RegularExpressions;

namespace Regexator.UI
{
    [DebuggerDisplay("Index: {Index}, Name: {Name}")]
    public class GroupInfoItem : GroupInfo, INotifyPropertyChanged
    {
        public GroupInfoItem(GroupInfo info)
            : base(info)
        {
            Enabled = true;
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
                    NotifyPropertyChanged("Enabled");
                }
            }
        }

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private bool _enabled;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
