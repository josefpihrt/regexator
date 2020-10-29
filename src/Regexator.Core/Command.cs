// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator
{
    public class Command : ICommand
    {
        public Command(Action action)
            : this(action, "")
        {
        }

        public Command(Action action, string name)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            Name = name ?? "";
        }

        public void Execute()
        {
            _action();
        }

        public override string ToString()
        {
            return Name;
        }

        public string Name { get; }

        private readonly Action _action;
    }
}
