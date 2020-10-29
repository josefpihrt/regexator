// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Regexator.Windows.Forms;

namespace Regexator.FileSystem
{
    internal class ContextMenuStripFactory
    {
        public ContextMenuStripFactory(FileSystemTreeView trv, FileSystemManager manager)
        {
            _manager = manager;
            _trv = trv;
        }

        public ContextMenuStrip GetContextMenuStrip(TreeNode node)
        {
            if (node is RootNode)
            {
                if (_cmsRoot == null)
                    _cmsRoot = CreateRootMenu();

                return _cmsRoot;
            }
            else if (node is ProjectNode)
            {
                if (_cmsProject == null)
                    _cmsProject = CreateProjectMenu();

                return _cmsProject;
            }
            else if (node is FileInputNode)
            {
                if (_cmsFileInput == null)
                    _cmsFileInput = CreateFileInputMenu();

                return _cmsFileInput;
            }
            else if (node is InputNode)
            {
                if (_cmsInput == null)
                    _cmsInput = CreateInputMenu();

                return _cmsInput;
            }
            else if (node is DirectoryNode)
            {
                if (_cmsDirectory == null)
                    _cmsDirectory = CreateDirectoryMenu();

                return _cmsDirectory;
            }
            else if (node is InvalidNode)
            {
                if (_cmsInvalid == null)
                    _cmsInvalid = CreateInvalidMenu();

                return _cmsInvalid;
            }

            return null;
        }

        public ContextMenuStrip CreateRootMenu()
        {
            return CreateMenu(f => CreateRootItems(f));
        }

        public ContextMenuStrip CreateDirectoryMenu()
        {
            return CreateMenu(f => CreateDirectoryItems(f));
        }

        public ContextMenuStrip CreateProjectMenu()
        {
            return CreateMenu(f => CreateProjectItems(f));
        }

        public ContextMenuStrip CreateFileInputMenu()
        {
            return CreateMenu(f => CreateFileInputItems(f));
        }

        public ContextMenuStrip CreateInputMenu()
        {
            return CreateMenu(f => CreateInputItems(f));
        }

        public ContextMenuStrip CreateDefaultMenu()
        {
            return CreateMenu(f => CreateDefaultItems(f));
        }

        public ContextMenuStrip CreateInvalidMenu()
        {
            return CreateMenu(() => CreateInvalidItems());
        }

        private ContextMenuStrip CreateMenu(Func<IEnumerable<ToolStripItem>> factory)
        {
            var cms = new ContextMenuStrip();
            cms.Items.AddRange(factory().ToArray());
            cms.Closed += (f, f2) => ContextMenuStrip_Closed(f, f2);
            return cms;
        }

        private ContextMenuStrip CreateMenu(Func<ContextMenuStrip, IEnumerable<ToolStripItem>> factory)
        {
            var cms = new ContextMenuStrip();
            cms.Items.AddRange(factory(cms).ToArray());
            cms.Closed += (f, f2) => ContextMenuStrip_Closed(f, f2);
            return cms;
        }

        private IEnumerable<ToolStripItem> CreateRootItems(ContextMenuStrip cms)
        {
            var tsiAddNewSubdirectory = new ToolStripMenuItem(
                Resources.NewFolder,
                Resources.IcoNewFolder.ToBitmap(),
                (object sender, EventArgs e) => _manager.AddNewSubdirectory())
            {
                ShortcutKeyDisplayString = Resources.F7
            };

            yield return new ToolStripMenuItem(
                Resources.NewItem + Resources.EllipsisStr,
                Resources.IcoNewItem.ToBitmap(),
                (object sender, EventArgs e) => _manager.AddNewProjectOrFolder())
            {
                ShortcutKeyDisplayString = Resources.CtrlShiftA
            };

            yield return new ToolStripSeparator();

            yield return new ToolStripMenuItem(
                Resources.NewProject + Resources.EllipsisStr,
                Resources.IcoCodeNew.ToBitmap(),
                (f, f2) => LoadNewProject(f, f2));

            yield return new ToolStripMenuItem(
                Resources.NewProjectWithInput + Resources.EllipsisStr,
                Resources.IcoNewProjectWithInput.ToBitmap(),
                (f, f2) => LoadNewProjectWithInput(f, f2));

            yield return new ToolStripMenuItem(
                Resources.NewProjectWithFileInput + Resources.EllipsisStr,
                Resources.IcoNewProjectWithFileInput.ToBitmap(),
                (f, f2) => LoadNewProjectWithFileInput(f, f2));

            yield return tsiAddNewSubdirectory;
            yield return new ToolStripSeparator();

            foreach (ToolStripItem item in CreateCutCopyPasteItems(cms))
                yield return item;

            yield return new ToolStripMenuItem(
                Resources.Refresh,
                Resources.IcoReload.ToBitmap(),
                (f, f2) => ReloadDirectory(f, f2));

            yield return new ToolStripSeparator();
            yield return CreateOpenInExplorerItem();
            yield return CreateCopyPathToClipboardItem();
            yield return new ToolStripSeparator();

            yield return new ToolStripMenuItem(
                Resources.CollapseAll,
                Resources.IcoCollapseAll.ToBitmap(),
                (object sender, EventArgs e) => _trv.SelectedNodeCollapseAll(true))
            {
                ShortcutKeyDisplayString = Resources.CtrlLeft
            };

            yield return new ToolStripMenuItem(
                Resources.ExpandAll,
                Resources.IcoExpandAll.ToBitmap(),
                (object sender, EventArgs e) => _trv.SelectedNodeExpandAll(true))
            {
                ShortcutKeyDisplayString = Resources.CtrlRight
            };

            cms.Opening += (object sender, CancelEventArgs e) => tsiAddNewSubdirectory.Visible = _manager
                .CanAddNewSubdirectory;
        }

        private IEnumerable<ToolStripItem> CreateDirectoryItems(ContextMenuStrip cms)
        {
            foreach (ToolStripItem item in CreateCutCopyPasteItems(cms))
                yield return item;

            yield return new ToolStripMenuItem(
                Resources.Delete,
                Resources.IcoDelete.ToBitmap(),
                (f, f2) => DeleteDirectory(f, f2))
            {
                ShortcutKeyDisplayString = Resources.Del
            };

            yield return new ToolStripMenuItem(
                Resources.Rename,
                Resources.IcoRename.ToBitmap(),
                (f, f2) => RenameItem(f, f2))
            {
                ShortcutKeyDisplayString = Resources.F2
            };

            yield return new ToolStripSeparator();
            yield return CreateOpenInExplorerItem();
            yield return CreateCopyPathToClipboardItem();
        }

        private IEnumerable<ToolStripItem> CreateProjectItems(ContextMenuStrip cms)
        {
            var tsiRemoveAllInputs = new ToolStripMenuItem(
                Resources.RemoveAllInputs,
                null,
                (f, f2) => RemoveAllInputs(f, f2));

            var tsiDeleteAllInputFiles = new ToolStripMenuItem(
                Resources.DeleteAllInputFiles,
                null,
                (f, f2) => DeleteAllFileInputs(f, f2));

            yield return new ToolStripMenuItem(Resources.Open, Resources.IcoOpen.ToBitmap(), (f, f2) => Open(f, f2))
            {
                ShortcutKeyDisplayString = Resources.F7
            };

            yield return new ToolStripSeparator();

            yield return new ToolStripMenuItem(
                Resources.NewItem + Resources.EllipsisStr,
                null,
                (object sender, EventArgs e) => _manager.AddNewInput())
            {
                ShortcutKeyDisplayString = Resources.CtrlShiftA
            };

            yield return new ToolStripSeparator();

            yield return new ToolStripMenuItem(
                Resources.NewInput + Resources.EllipsisStr,
                Resources.IcoInputNew.ToBitmap(),
                (f, f2) => LoadNewInput(f, f2));

            yield return new ToolStripMenuItem(
                Resources.NewFileInput + Resources.EllipsisStr,
                Resources.IcoFileNew.ToBitmap(),
                (f, f2) => LoadNewFileInput(f, f2));

            yield return new ToolStripMenuItem(
                Resources.ExistingFileInput + Resources.EllipsisStr,
                Resources.IcoFilePlus.ToBitmap(),
                (f, f2) => LoadNewInputs(f, f2))
            {
                ShortcutKeyDisplayString = Resources.CtrlD
            };

            yield return new ToolStripSeparator();

            foreach (ToolStripItem item in CreateCutCopyPasteItems(cms))
                yield return item;

            yield return new ToolStripMenuItem(
                Resources.Delete,
                Resources.IcoDelete.ToBitmap(),
                (f, f2) => DeleteProject(f, f2))
            {
                ShortcutKeyDisplayString = Resources.Del
            };

            yield return new ToolStripMenuItem(
                Resources.Rename,
                Resources.IcoRename.ToBitmap(),
                (f, f2) => RenameItem(f, f2))
            {
                ShortcutKeyDisplayString = Resources.F2
            };

            yield return new ToolStripSeparator();
            yield return tsiRemoveAllInputs;
            yield return tsiDeleteAllInputFiles;
            yield return new ToolStripSeparator();
#if DEBUG
            yield return new ToolStripMenuItem(Resources.OpenInNotepad, null, (f, f2) => OpenProjectInNotepad(f, f2));
#endif
            yield return CreateOpenInExplorerItem();
            yield return CreateCopyPathToClipboardItem();

            cms.Opening += (object sender, CancelEventArgs e) =>
            {
                var node = _trv.SelectedNode as ProjectNode;
                tsiRemoveAllInputs.Enabled = node?.Project.HasInputOrFileInput == true;
                tsiDeleteAllInputFiles.Enabled = node?.Project.FileInputs.Count > 0;
            };
        }

        private IEnumerable<ToolStripItem> CreateFileInputItems(ContextMenuStrip cms)
        {
            yield return new ToolStripMenuItem(Resources.Open, Resources.IcoOpen.ToBitmap(), (f, f2) => Open(f, f2))
            {
                ShortcutKeyDisplayString = Resources.F7
            };

            yield return new ToolStripSeparator();

            yield return new ToolStripMenuItem(Resources.RemoveFromProject, null, (f, f2) => RemoveFileInput(f, f2))
            {
                ShortcutKeyDisplayString = Resources.CtrlDel
            };

            yield return new ToolStripSeparator();

            foreach (ToolStripItem item in CreateCutCopyPasteItems(cms))
                yield return item;

            yield return new ToolStripMenuItem(
                Resources.Delete,
                Resources.IcoDelete.ToBitmap(),
                (f, f2) => DeleteFileInput(f, f2))
            {
                ShortcutKeyDisplayString = Resources.Del
            };

            yield return new ToolStripMenuItem(
                Resources.Rename,
                Resources.IcoRename.ToBitmap(),
                (f, f2) => RenameItem(f, f2))
            {
                ShortcutKeyDisplayString = Resources.F2
            };

            yield return new ToolStripSeparator();
#if DEBUG
            yield return new ToolStripMenuItem(Resources.OpenInNotepad, null, (f, f2) => OpenFileInputInNotepad(f, f2));
#endif
            yield return CreateOpenInExplorerItem();
            yield return CreateCopyPathToClipboardItem();
        }

        private IEnumerable<ToolStripItem> CreateInputItems(ContextMenuStrip cms)
        {
            yield return new ToolStripMenuItem(Resources.Open, Resources.IcoOpen.ToBitmap(), (f, f2) => Open(f, f2))
            {
                ShortcutKeyDisplayString = Resources.F7
            };

            yield return new ToolStripSeparator();

            foreach (ToolStripItem item in CreateCutCopyPasteItems(cms))
                yield return item;

            yield return new ToolStripMenuItem(
                Resources.Delete,
                Resources.IcoDelete.ToBitmap(),
                (f, f2) => DeleteInput(f, f2))

            {
                ShortcutKeyDisplayString = Resources.Del
            };

            yield return new ToolStripMenuItem(
                Resources.Rename,
                Resources.IcoRename.ToBitmap(),
                (f, f2) => RenameItem(f, f2))
            {
                ShortcutKeyDisplayString = Resources.F2
            };
        }

        private IEnumerable<ToolStripItem> CreateDefaultItems(ContextMenuStrip cms)
        {
            var tsiAddNewSubdirectory = new ToolStripMenuItem(
                Resources.NewFolder,
                Resources.IcoNewFolder.ToBitmap(),
                (object sender, EventArgs e) => _manager.AddNewSubdirectory())
            {
                ShortcutKeyDisplayString = Resources.F7
            };

            yield return new ToolStripMenuItem(
                Resources.NewItem + Resources.EllipsisStr,
                Resources.IcoNewItem.ToBitmap(),
                (object sender, EventArgs e) => _manager.AddNewProjectOrFolder());

            yield return new ToolStripSeparator();

            yield return new ToolStripMenuItem(
                Resources.NewProject + Resources.EllipsisStr,
                Resources.IcoCodeNew.ToBitmap(),
                (f, f2) => LoadNewProject(f, f2));

            yield return new ToolStripMenuItem(
                Resources.NewProjectWithInput + Resources.EllipsisStr,
                Resources.IcoNewProjectWithInput.ToBitmap(),
                (f, f2) => LoadNewProjectWithInput(f, f2));

            yield return new ToolStripMenuItem(
                Resources.NewProjectWithFileInput + Resources.EllipsisStr,
                Resources.IcoNewProjectWithFileInput.ToBitmap(),
                (f, f2) => LoadNewProjectWithFileInput(f, f2));

            yield return tsiAddNewSubdirectory;
            yield return new ToolStripSeparator();

            yield return new ToolStripMenuItem(
                Resources.Refresh,
                Resources.IcoReload.ToBitmap(),
                (f, f2) => ReloadDirectory(f, f2));

            yield return new ToolStripSeparator();

            yield return new ToolStripMenuItem(
                Resources.CollapseAll,
                Resources.IcoCollapseAll.ToBitmap(),
                (f, f2) => CollapseAll(f, f2));

            yield return new ToolStripMenuItem(
                Resources.ExpandAll,
                Resources.IcoExpandAll.ToBitmap(),
                (f, f2) => ExpandAll(f, f2));

            cms.Opening += (object sender, CancelEventArgs e)
                => tsiAddNewSubdirectory.Visible = _manager.CanAddNewSubdirectory;
        }

        private IEnumerable<ToolStripItem> CreateInvalidItems()
        {
            yield return new ToolStripMenuItem(Resources.Open, Resources.IcoOpen.ToBitmap(), (f, f2) => Open(f, f2))
            {
                ShortcutKeyDisplayString = Resources.F7
            };

            yield return new ToolStripSeparator();

            yield return new ToolStripMenuItem(Resources.Remove, null, (f, f2) => RemoveInvalid(f, f2))
            {
                ShortcutKeyDisplayString = Resources.CtrlDel
            };
        }

        private IEnumerable<ToolStripItem> CreateCutCopyPasteItems(ContextMenuStrip cms)
        {
#if DEBUG
            var tsiCut = new ToolStripMenuItem(
                Resources.Cut,
                Resources.IcoCut.ToBitmap(),
                (object sender, EventArgs e) => _manager.ClipboardManager.SetSelectedNode(ClipboardMode.Cut))
            {
                ShortcutKeyDisplayString = Resources.CtrlX
            };

            yield return tsiCut;

            var tsiCopy = new ToolStripMenuItem(
                Resources.Copy,
                Resources.IcoCopy.ToBitmap(),
                (object sender, EventArgs e) => _manager.ClipboardManager.SetSelectedNode(ClipboardMode.Copy))
            {
                ShortcutKeyDisplayString = Resources.CtrlC
            };

            yield return tsiCopy;

            var tsiPaste = new ToolStripMenuItem(
                Resources.Paste,
                Resources.IcoPaste.ToBitmap(),
                (object sender, EventArgs e) => _manager.ClipboardManager.Paste())
            {
                ShortcutKeyDisplayString = Resources.CtrlV
            };

            yield return tsiPaste;

            var separator = new ToolStripSeparator();
            yield return separator;

            cms.Opening += (object sender, CancelEventArgs e) =>
            {
                bool canCut = _manager.CanCut();
                bool canCopy = _manager.CanCopy();
                bool canPaste = _manager.CanBePasted();
                tsiCut.Visible = canCut;
                tsiCopy.Visible = canCopy;
                tsiPaste.Visible = canPaste;
                separator.Visible = canCut || canCopy || canPaste;
            };
#else
            yield break;
#endif
        }

        private void Open(object sender, EventArgs e)
        {
            TreeNode node = _trv.SelectedNode;

            if (node is ProjectNode projectNode)
            {
                _manager.LoadOrReloadProject(projectNode);
            }
            else if (node is FileInputNode fileNode)
            {
                _manager.LoadOrReloadFileInput(fileNode);
            }
            else if (node is InputNode inputNode)
            {
                _manager.LoadOrReloadInput(inputNode);
            }
            else if (node is InvalidNode invalidNode)
            {
                _manager.LoadInvalid(invalidNode);
            }
        }

        private void ReloadDirectory(object sender, EventArgs e)
        {
            _manager.ReloadDirectory();
        }

        private void LoadNewProject(object sender, EventArgs e)
        {
            _manager.LoadNewProject();
        }

        private void LoadNewProjectWithFileInput(object sender, EventArgs e)
        {
            _manager.LoadNewProject(InputKind.File);
        }

        private void LoadNewProjectWithInput(object sender, EventArgs e)
        {
            _manager.LoadNewProject(InputKind.Included);
        }

        private void LoadNewFileInput(object sender, EventArgs e)
        {
            _manager.LoadNewInput(InputKind.File);
        }

        private void LoadNewInput(object sender, EventArgs e)
        {
            _manager.LoadNewInput(InputKind.Included);
        }

        private void LoadNewInputs(object sender, EventArgs e)
        {
            if (_trv.SelectedNode is ProjectNode node)
                _manager.LoadNewFileInputs(node);
        }

        private void RenameItem(object sender, EventArgs e)
        {
            TreeNode node = _trv.SelectedNode;
            node?.BeginEdit();
        }

        private void DeleteDirectory(object sender, EventArgs e)
        {
            if (_trv.SelectedNode is DirectoryNode node)
                _manager.DeleteDirectory(node);
        }

        private void DeleteProject(object sender, EventArgs e)
        {
            if (_trv.SelectedNode is ProjectNode node)
                _manager.DeleteProject(node);
        }

        private void DeleteFileInput(object sender, EventArgs e)
        {
            if (_trv.SelectedNode is FileInputNode node)
                _manager.DeleteFileInput(node);
        }

        private void RemoveFileInput(object sender, EventArgs e)
        {
            if (_trv.SelectedNode is FileInputNode node)
                _manager.RemoveFileInput(node, _manager.ConfirmFileInputRemoval);
        }

        private void RemoveAllInputs(object sender, EventArgs e)
        {
            if (_trv.SelectedNode is ProjectNode node)
                _manager.RemoveAllInputs(node);
        }

        private void DeleteAllFileInputs(object sender, EventArgs e)
        {
            if (_trv.SelectedNode is ProjectNode node)
                _manager.DeleteAllFileInputs(node);
        }

        private void DeleteInput(object sender, EventArgs e)
        {
            if (_trv.SelectedNode is InputNode node)
                _manager.DeleteInput(node);
        }

        private void RemoveInvalid(object sender, EventArgs e)
        {
            if (_trv.SelectedNode is InvalidProjectNode invalidProjectNode)
            {
                _manager.RemoveNode(invalidProjectNode);
            }
            else if (_trv.SelectedNode is InvalidInputNode invalidInputNode)
            {
                _manager.RemoveInput(invalidInputNode);
            }
        }

#if DEBUG
        private void OpenProjectInNotepad(object sender, EventArgs e)
        {
            if (_trv.SelectedNode is ProjectNode node)
                Executor.StartProcess(() => Process.Start("notepad.exe", node.FullName));
        }

        private void OpenFileInputInNotepad(object sender, EventArgs e)
        {
            if (_trv.SelectedNode is FileInputNode node)
                Executor.StartProcess(() => Process.Start("notepad.exe", node.FullName));
        }
#endif

        private void OpenPathInExplorer(object sender, EventArgs e)
        {
            TreeNode node = _trv.SelectedNode;
            if (node != null)
                FileSystemUtility.OpenPathInExplorer((FileSystemNode)node);
        }

        private void CopyPathToClipboard(object sender, EventArgs e)
        {
            if (_trv.SelectedNode is FileSystemNode node)
                ClipboardManager.CopyPathToClipboard(node);
        }

        private void ContextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            _trv.ContextMenuStrip = null;
        }

        private void ExpandAll(object sender, EventArgs e)
        {
            _trv.ExpandAll(true);
            _trv.SelectedNode?.EnsureVisible();
        }

        private void CollapseAll(object sender, EventArgs e)
        {
            _trv.CollapseAll(true);
            _trv.SelectedNode?.EnsureVisible();
        }

        private ToolStripMenuItem CreateOpenInExplorerItem()
        {
            return new ToolStripMenuItem(Resources.OpenInExplorer, null, (f, f2) => OpenPathInExplorer(f, f2));
        }

        private ToolStripMenuItem CreateCopyPathToClipboardItem()
        {
            return new ToolStripMenuItem(Resources.CopyPath, null, (f, f2) => CopyPathToClipboard(f, f2));
        }

        public ContextMenuStrip DefaultMenu
        {
            get { return _cmsDefault ?? (_cmsDefault = CreateDefaultMenu()); }
        }

        private readonly FileSystemManager _manager;
        private readonly FileSystemTreeView _trv;

        private ContextMenuStrip _cmsRoot;
        private ContextMenuStrip _cmsProject;
        private ContextMenuStrip _cmsFileInput;
        private ContextMenuStrip _cmsInput;
        private ContextMenuStrip _cmsDefault;
        private ContextMenuStrip _cmsDirectory;
        private ContextMenuStrip _cmsInvalid;
    }
}