// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator.FileSystem
{
    public class SaveManager
    {
        internal SaveManager(FileSystemManager manager)
        {
            _manager = manager;
        }

        public bool Save()
        {
            return Save(SaveMode.All);
        }

        public bool SaveInput()
        {
            return Save(SaveMode.Input);
        }

        public bool Save(SaveMode mode)
        {
            return Save(mode, true);
        }

        public bool Save(bool confirm)
        {
            return Save(SaveMode.All, confirm);
        }

        public bool Save(SaveMode mode, bool confirm)
        {
            var e = new SaveEventArgs(mode, confirm);
            OnSaveRequested(e);
            return SaveExecutor.Save(_manager, e);
        }

        protected virtual void OnSaveRequested(SaveEventArgs e)
        {
            SaveRequested?.Invoke(this, e);
        }

        private readonly FileSystemManager _manager;

        public event EventHandler<SaveEventArgs> SaveRequested;
    }
}