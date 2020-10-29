// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Regexator.Collections.Generic;
using Regexator.IO;

namespace Regexator.FileSystem
{
    public class RecentManager
    {
        public RecentManager(FileSystemManager manager)
        {
            _items = new List<RecentItem>();
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public void AddRange(IEnumerable<RecentItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (RecentItem item in items)
                Add(item);
        }

        public void Add(RecentItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            Remove(item.FullName);
            _items.Add(item);
        }

        public bool Remove(string path)
        {
            int index = _items.FindIndex(f => FileSystemUtility.PathEquals(f.FullName, path));
            if (index != -1)
            {
                _items.RemoveAt(index);
                return true;
            }

            return false;
        }

        public void RemoveInvalidItems()
        {
            RecentItem[] items = EnumerateInvalidItems().ToArray();
            if (items.Length > 0)
            {
                if (MessageDialog.Confirm(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.RemoveInvalidRecentItemsMsg,
                    items.Count(f => f.Kind == ItemKind.Directory),
                    items.Count(f => f.Kind == ItemKind.Project))))
                {
                    int directoryCount = 0;
                    int projectCount = 0;
                    foreach (RecentItem item in items)
                    {
                        if (_items.Remove(item))
                        {
                            if (item.Kind == ItemKind.Directory)
                            {
                                directoryCount++;
                            }
                            else if (item.Kind == ItemKind.Project)
                            {
                                projectCount++;
                            }
                        }
                    }

                    MessageDialog.Info(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.InvalidRecentItemsRemovedMsg,
                        directoryCount,
                        projectCount));
                }
            }
            else
            {
                MessageDialog.Info(Resources.NoInvalidRecentItemsFoundMsg);
            }
        }

        public void Clear(ItemKind kind)
        {
            _items.RemoveAll(f => f.Kind == kind);
        }

        public void Clear()
        {
            _items.Clear();
        }

        private IEnumerable<RecentItem> EnumerateInvalidItems()
        {
            return _items.Where(f => !f.Info.ExistsNow());
        }

        public bool Any(ItemKind kind)
        {
            return Reversed().Any(f => f.Kind == kind);
        }

        public IEnumerable<RecentItem> EnumerateItems()
        {
            return EnumerateItems(_ => true);
        }

        public IEnumerable<RecentItem> EnumerateItems(ItemKind kind)
        {
            return EnumerateItems(f => f.Kind == kind);
        }

        private IEnumerable<RecentItem> EnumerateItems(Func<RecentItem, bool> predicate)
        {
            return Reversed().Where(predicate);
        }

        private IEnumerable<RecentItem> Reversed()
        {
            return _items.Reversed();
        }

        public IEnumerable<ToolStripMenuItem> CreateToolStripMenuItems(ItemKind kind)
        {
            return CreateToolStripMenuItems(kind, false);
        }

        public IEnumerable<ToolStripMenuItem> CreateToolStripMenuItems(ItemKind kind, bool addNumbers)
        {
            return (addNumbers)
                ? CreateToolStripMenuItemsWithNumbers(kind)
                : CreateToolStripMenuItemsInternal(kind);
        }

        private IEnumerable<ToolStripMenuItem> CreateToolStripMenuItemsWithNumbers(ItemKind kind)
        {
            int i = 1;
            foreach (ToolStripMenuItem item in CreateToolStripMenuItemsInternal(kind))
            {
                item.Text = i.ToString(CultureInfo.CurrentCulture) + " " + item.Text;
                yield return item;
                i++;
            }
        }

        private IEnumerable<ToolStripMenuItem> CreateToolStripMenuItemsInternal(ItemKind kind)
        {
            return EnumerateItems(kind).Select(f => f.CreateToolStripMenuItem(f2 => Item_Click(f2)));
        }

        private void Item_Click(RecentItem item)
        {
            switch (item.Kind)
            {
                case ItemKind.Directory:
                    {
                        LoadDirectory(item);
                        break;
                    }
                case ItemKind.Project:
                    {
                        LoadProject(item);
                        break;
                    }
            }
        }

        private void LoadDirectory(RecentItem item)
        {
            if (!item.Info.ExistsNow())
            {
                if (MessageDialog.Question(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.FolderNotFoundRemoveItFromRecentMsg,
                        item.FullName)) == DialogResult.Yes)
                {
                    _items.Remove(item);
                }
            }
            else
            {
                _manager.TreeView.Select();
                _manager.LoadDirectory((DirectoryInfo)item.Info);
            }
        }

        private void LoadProject(RecentItem item)
        {
            if (!item.Info.ExistsNow())
            {
                if (MessageDialog.Question(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.FileNotFoundRemoveItFromRecentMsg,
                        item.FullName)) == DialogResult.Yes)
                {
                    _items.Remove(item);
                }
            }
            else
            {
                _manager.TreeView.Select();
                var info = (FileInfo)item.Info;

                if (!_manager.IsCurrentProjectNode(info))
                    _manager.LoadProject(info);
            }
        }

        private readonly List<RecentItem> _items;
        private readonly FileSystemManager _manager;
    }
}
