// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Drawing;

namespace Regexator
{
    public class NewItemCommand : ICommand
    {
        public NewItemCommand(string name, Action action)
            : this(name, action, null)
        {
        }

        public NewItemCommand(string name, Action action, Icon icon)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _action = action ?? throw new ArgumentNullException(nameof(action));
            Icon = icon;
        }

        public void Execute()
        {
            _action();
        }

        public string Name { get; }

        public Icon Icon { get; }

        private readonly Action _action;
    }
}
