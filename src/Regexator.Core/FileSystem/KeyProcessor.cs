// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Windows.Forms;
using Regexator.Windows.Forms;

namespace Regexator.FileSystem
{
    internal class KeyProcessor
    {
        public KeyProcessor(FileSystemManager manager)
        {
            _manager = manager;
            _trv = _manager.TreeView;
        }

        public void ProcessKeyDown(KeyEventArgs e)
        {
            ProcessKeys(e);
            if (_trv.SelectedNode != null)
            {
                if (_trv.SelectedNode is RootNode rootNode)
                {
                    ProcessKeys(e, rootNode);
                    return;
                }

                if (_trv.SelectedNode is DirectoryNode directoryNode)
                {
                    ProcessKeys(e, directoryNode);
                    return;
                }

                if (_trv.SelectedNode is ProjectNode projectNode)
                {
                    ProcessKeys(e, projectNode);
                    return;
                }

                if (_trv.SelectedNode is FileInputNode fileNode)
                {
                    ProcessKeys(e, fileNode);
                    return;
                }

                if (_trv.SelectedNode is InputNode inputNode)
                {
                    ProcessKeys(e, inputNode);
                    return;
                }

                if (_trv.SelectedNode is InvalidNode invalidNode)
                    ProcessKeys(e, invalidNode);
            }
        }

        private void ProcessKeys(KeyEventArgs e)
        {
            switch (e.Modifiers)
            {
                case Keys.None:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.F7:
                                {
                                    if (_trv.SelectedNode == null || _trv.SelectedNode is RootNode)
                                        _manager.AddNewSubdirectory();

                                    break;
                                }
                            case Keys.Back:
                                {
                                    _manager.LoadParentDirectory();
                                    break;
                                }
                        }

                        break;
                    }
                case Keys.Control:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.Left:
                                {
                                    _trv.SelectedNodeCollapseAll();
                                    break;
                                }
                            case Keys.Right:
                                {
                                    _trv.SelectedNodeExpandAll();
                                    break;
                                }
                            case Keys.Delete:
                                {
                                    if (_trv.SelectedNode is InvalidProjectNode invalidProjectNode)
                                    {
                                        _manager.RemoveNode(invalidProjectNode);
                                    }
                                    else if (_trv.SelectedNode is InvalidInputNode invalidInputNode)
                                    {
                                        _manager.RemoveInput(invalidInputNode);
                                    }

                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        private void ProcessKeys(KeyEventArgs e, RootNode node)
        {
            switch (e.Modifiers)
            {
                case Keys.None:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.Enter:
                                {
                                    _manager.LoadParentDirectory(node);
                                    break;
                                }
                        }

                        break;
                    }
                case (Keys.Control | Keys.Shift):
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.A:
                                {
                                    _manager.AddNewProjectOrFolder();
                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        private void ProcessKeys(KeyEventArgs e, DirectoryNode node)
        {
            switch (e.Modifiers)
            {
                case Keys.None:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.F2:
                                {
                                    node.BeginEdit();
                                    break;
                                }
                            case Keys.Enter:
                                {
                                    _manager.LoadDirectory(node.DirectoryInfo);
                                    break;
                                }
                            case Keys.Delete:
                                {
                                    _manager.DeleteDirectory(node);
                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        private void ProcessKeys(KeyEventArgs e, ProjectNode node)
        {
            switch (e.Modifiers)
            {
                case Keys.None:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.F2:
                                {
                                    node.BeginEdit();
                                    break;
                                }
                            case Keys.F7:
                            case Keys.Enter:
                                {
                                    _manager.LoadOrReloadProject(node);
                                    break;
                                }
                            case Keys.Delete:
                                {
                                    _manager.DeleteProject(node);
                                    break;
                                }
                        }

                        break;
                    }
                case Keys.Control:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.D:
                                {
                                    _manager.LoadNewFileInputs(node);
                                    break;
                                }
                        }

                        break;
                    }
                case (Keys.Control | Keys.Shift):
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.A:
                                {
                                    _manager.AddNewInput();
                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        private void ProcessKeys(KeyEventArgs e, FileInputNode node)
        {
            switch (e.Modifiers)
            {
                case Keys.None:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.F2:
                                {
                                    node.BeginEdit();
                                    break;
                                }
                            case Keys.F7:
                            case Keys.Enter:
                                {
                                    _manager.LoadOrReloadFileInput(node);
                                    break;
                                }
                            case Keys.Delete:
                                {
                                    _manager.DeleteFileInput(node);
                                    break;
                                }
                        }

                        break;
                    }
                case Keys.Control:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.Delete:
                                {
                                    _manager.RemoveFileInput(node, _manager.ConfirmFileInputRemoval);
                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        private void ProcessKeys(KeyEventArgs e, InputNode node)
        {
            switch (e.Modifiers)
            {
                case Keys.None:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.Enter:
                                {
                                    _manager.LoadOrReloadInput(node);
                                    break;
                                }
                            case Keys.F2:
                                {
                                    node.BeginEdit();
                                    break;
                                }
                            case Keys.F7:
                                {
                                    _manager.LoadOrReloadInput(node);
                                    break;
                                }
                            case Keys.Delete:
                                {
                                    _manager.DeleteInput(node);
                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        private void ProcessKeys(KeyEventArgs e, InvalidNode node)
        {
            switch (e.Modifiers)
            {
                case Keys.None:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.F7:
                            case Keys.Enter:
                                {
                                    _manager.LoadInvalid(node);
                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        private readonly FileSystemManager _manager;
        private readonly FileSystemTreeView _trv;
    }
}
