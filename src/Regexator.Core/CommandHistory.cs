// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Regexator
{
    public class CommandHistory
    {
        private readonly List<ICommand> _items;
        private bool _canUndo;
        private bool _canRedo;

        public event EventHandler CanUndoChanged;
        public event EventHandler CanRedoChanged;
        public event EventHandler<UndoEventArgs> Undoing;
        public event EventHandler<UndoEventArgs> Redoing;

        public CommandHistory()
        {
            _items = new List<ICommand>(32);
        }

        public void AddCommand(ICommand command)
        {
            _items.RemoveRange(_items.Count - RedoCount, RedoCount);
            _items.Add(command);
            RedoCount = 0;
            SetCanUndoCanRedo();
        }

        public void Undo()
        {
            Undo(1);
        }

        public void Undo(int count)
        {
            int index = _items.Count - 1 - RedoCount - count;
            ICommand item = _items[index];
            var e = new UndoEventArgs(item);
            OnUndoing(e);
            try
            {
                if (!e.Cancel)
                {
                    IsExecuting = true;
                    try
                    {
                        item.Execute();
                    }
                    finally
                    {
                        IsExecuting = false;
                        RedoCount += count;
                    }
                }
                else if (e.Remove)
                {
                    _items.RemoveAt(index);
                }
            }
            finally
            {
                SetCanUndoCanRedo();
            }
        }

        public void UndoIfCan()
        {
            if (CanUndo)
                Undo();
        }

        public void Redo()
        {
            Redo(1);
        }

        public void Redo(int count)
        {
            int index = _items.Count - 1 - RedoCount + count;
            ICommand item = _items[index];
            var e = new UndoEventArgs(item);
            OnRedoing(e);
            try
            {
                if (!e.Cancel)
                {
                    IsExecuting = true;
                    try
                    {
                        item.Execute();
                    }
                    finally
                    {
                        IsExecuting = false;
                        RedoCount -= count;
                    }
                }
                else if (e.Remove)
                {
                    RemoveAt(index);
                }
            }
            finally
            {
                SetCanUndoCanRedo();
            }
        }

        public void RedoIfCan()
        {
            if (CanRedo)
                Redo();
        }

        public void Clear()
        {
            _items.Clear();
            RedoCount = 0;
            SetCanUndoCanRedo();
        }

        public bool Remove(ICommand item)
        {
            int index = _items.IndexOf(item);
            if (index != -1)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        private void RemoveAt(int index)
        {
            bool flg = (index >= _items.Count - RedoCount);
            _items.RemoveAt(index);

            if (flg)
                RedoCount--;

            SetCanUndoCanRedo();
        }

        private void SetCanUndoCanRedo()
        {
            CanUndo = (_items.Count - 1) > RedoCount;
            CanRedo = RedoCount > 0;
        }

        protected virtual void OnCanUndoChanged(EventArgs e)
        {
            CanUndoChanged?.Invoke(this, e);
        }

        protected virtual void OnCanRedoChanged(EventArgs e)
        {
            CanRedoChanged?.Invoke(this, e);
        }

        protected virtual void OnUndoing(UndoEventArgs e)
        {
            Undoing?.Invoke(this, e);
        }

        protected virtual void OnRedoing(UndoEventArgs e)
        {
            Redoing?.Invoke(this, e);
        }

        public IEnumerable<ICommand> UndoItems
        {
            get
            {
                for (int i = _items.Count - 2 - RedoCount; i >= 0; i--)
                    yield return _items[i];
            }
        }

        public IEnumerable<ICommand> RedoItems
        {
            get
            {
                for (int i = _items.Count - RedoCount; i < _items.Count; i++)
                    yield return _items[i];
            }
        }

        public string UndoName
        {
            get
            {
                ICommand item = FirstUndo;
                return (item != null) ? item.Name : "";
            }
        }

        public string RedoName
        {
            get
            {
                ICommand item = FirstRedo;
                return (item != null) ? item.Name : "";
            }
        }

        public bool CanUndo
        {
            get { return _canUndo; }
            set
            {
                if (_canUndo != value)
                {
                    _canUndo = value;
                    OnCanUndoChanged(EventArgs.Empty);
                }
            }
        }

        public bool CanRedo
        {
            get { return _canRedo; }
            set
            {
                if (_canRedo != value)
                {
                    _canRedo = value;
                    OnCanRedoChanged(EventArgs.Empty);
                }
            }
        }

        public ICommand Current
        {
            get
            {
                if (_items.Count > 0)
                    return _items[_items.Count - 1 - RedoCount];

                return null;
            }
        }

        public ICommand FirstUndo
        {
            get { return UndoItems.FirstOrDefault(); }
        }

        public ICommand FirstRedo
        {
            get { return RedoItems.FirstOrDefault(); }
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public int UndoCount
        {
            get { return (_items.Count > 0) ? _items.Count - RedoCount : 0; }
        }

        public int RedoCount { get; private set; }

        public bool IsExecuting { get; private set; }
    }
}
