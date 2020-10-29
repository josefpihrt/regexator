// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator.FileSystem
{
    public class SaveEventArgs : EventArgs
    {
        public SaveEventArgs(SaveMode mode, bool confirm)
        {
            Mode = mode;
            Confirm = confirm;
        }

        public ProjectContainer Container { get; set; }
        public Input Input { get; set; }

        public SaveMode Mode { get; }

        public bool Confirm { get; }
    }
}
