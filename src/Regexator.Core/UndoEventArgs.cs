// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel;

namespace Regexator
{
    public class UndoEventArgs : CancelEventArgs
    {
        public UndoEventArgs(ICommand command)
        {
            Command = command;
        }

        public bool Remove { get; set; }

        public ICommand Command { get; }
    }
}
