// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace Regexator.FileSystem
{
    internal class ClipboardManager : IDisposable
    {
        public ClipboardManager(FileSystemManager manager)
        {
            _manager = manager;
            _trv = manager.TreeView;
            _trv.KeyDown += (f, f2) => TreeView_KeyDown(f, f2);
            Enabled = true;
        }

        private void TreeView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Modifiers)
            {
                case Keys.Control:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.C:
                                {
                                    SetSelectedNode(ClipboardMode.Copy);
                                    break;
                                }
                            case Keys.V:
                                {
                                    Paste();
                                    break;
                                }
                            case Keys.X:
                                {
                                    SetSelectedNode(ClipboardMode.Cut);
                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        public void SetSelectedNode(ClipboardMode mode)
        {
            ClipboardItem item = SetNode(_trv.SelectedNode, mode);

            if (item != null)
                CurrentItem = item;
        }

        private ClipboardItem SetNode(TreeNode node, ClipboardMode mode)
        {
            if (Enabled && node is IClipboardNode n)
            {
                ClipboardItem item = ClipboardItem.Create(n, mode);
                var data = new DataObject();
                data.SetData(typeof(Guid), item.Guid);

                if (Execute(() => Clipboard.SetDataObject(data)))
                    return item;
            }

            return null;
        }

        public bool CanBePasted(TreeNode node)
        {
            ClipboardItem item = GetItem();
            return item?.CanBePasted(node) ?? false;
        }

        private ClipboardItem GetItem()
        {
            if (CurrentItem != null)
            {
                IDataObject data = null;
                Execute(() => data = Clipboard.GetDataObject());

                if (data?.GetDataPresent(typeof(Guid)) == true
                    && (Guid)data.GetData(typeof(Guid)) == CurrentItem.Guid)
                {
                    return CurrentItem;
                }
            }

            return null;
        }

        public void Paste()
        {
            if (Enabled)
            {
                ClipboardItem item = GetItem();

                if (item != null)
                {
                    Executor.Execute(() => item.Paste(_manager), Executor.BasicExceptions | Exceptions.InvalidOperation);
                }
                //todo FileDropList do CanPaste
                else if (_trv.SelectedNode != null)
                {
                    ProjectNode projectNode = (_trv.SelectedNode as ProjectNode)
                        ?? (_trv.SelectedNode.Parent as ProjectNode);
                    if (projectNode != null)
                    {
                        string[] paths = GetFileDropList();
                        if (paths != null)
                            _manager.LoadFileInputsInternal(projectNode, paths);
                    }
                }
            }
        }

        public static string[] GetFileDropList()
        {
            string[] paths = null;
            Execute(() =>
            {
                if (Clipboard.ContainsFileDropList())
                {
                    StringCollection values = Clipboard.GetFileDropList();
                    if (values != null)
                        paths = values.Cast<string>().ToArray();
                }
            });
            return paths;
        }

        public static void CopyPathToClipboard(FileSystemNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (!string.IsNullOrEmpty(node.FullName))
                Execute(() => Clipboard.SetText(node.FullName));
        }

        private static bool Execute(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
                Debug.Fail("");
            }
            catch (System.Threading.ThreadStateException)
            {
                Debug.Fail("");
            }

            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    _trv.KeyDown -= (f, f2) => TreeView_KeyDown(f, f2);

                _disposed = true;
            }
        }

        private ClipboardItem CurrentItem { get; set; }
        public bool Enabled { get; set; }

        private readonly FileSystemManager _manager;
        private readonly FileSystemTreeView _trv;
        private bool _disposed;
    }
}