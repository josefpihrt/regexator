// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using Regexator.Windows.Forms;

namespace Regexator.FileSystem
{
    internal sealed class DragDrop : IDisposable
    {
        private readonly FileSystemManager _manager;
        private readonly FileSystemTreeView _trv;
        private readonly bool _rightMouseButton;
        private bool _disposed;

        public DragDrop(ItemDragEventArgs e, FileSystemManager manager)
        {
            _manager = manager;
            _trv = manager.TreeView;
            _trv.DragEnter += (f, f2) => TreeView_DragOverDragEnter(f, f2);
            _trv.DragOver += (f, f2) => TreeView_DragOverDragEnter(f, f2);
            _trv.DragDrop += (f, f2) => TreeView_DragDrop(f, f2);
            _rightMouseButton = (e.Button == MouseButtons.Right);
            if (e.Button == MouseButtons.Left || _rightMouseButton)
                DoDragDrop(e);
        }

        private void TreeView_DragDrop(object sender, DragEventArgs e)
        {
            FileSystemNode target = GetTargetNode(e);
            DragItem item = GetDragItem(e);

            if (item != null)
            {
                if (CheckNode(item, target))
                {
                    switch (item.Kind)
                    {
                        case NodeKind.Directory:
                            {
                                DropDirectoryNode((DirectoryNode)item.Node, target, e);
                                break;
                            }
                        case NodeKind.Project:
                            {
                                DropProjectNode((ProjectNode)item.Node, target, e);
                                break;
                            }
                        case NodeKind.Input:
                            {
                                DropInputNode((InputNode)item.Node, target as ProjectNode, e);
                                break;
                            }
                        case NodeKind.FileInput:
                            {
                                DropFileInputNode((FileInputNode)item.Node, GetProjectNode(target), e);
                                break;
                            }
                    }
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
                {
                    ProjectNode projectNode = GetProjectNode(target);
                    if (projectNode != null)
                        _manager.LoadFileInputsInternal(projectNode, paths);
                }
            }
        }

        private void TreeView_DragOverDragEnter(object sender, DragEventArgs e)
        {
            var effects = DragDropEffects.None;
            FileSystemNode target = GetTargetNode(e);
            if (target != null)
            {
                DragItem item = GetDragItem(e);
                if (item != null)
                {
                    if (CheckNode(item, target))
                        effects = DragDropEffects.Move;
                }
                else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    effects = DragDropEffects.Link;
                }
            }

            e.Effect = effects;
        }

        private void DoDragDrop(ItemDragEventArgs e)
        {
            DragItem item = DragItem.Create(e);
            if (item != null)
                _trv.DoDragDrop(item, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
        }

        private void DropDirectoryNode(DirectoryNode directoryNode, FileSystemNode target, DragEventArgs e)
        {
            string destDirPath = Path.Combine(GetDirectoryPath(target), directoryNode.FileName);
            DropNode(
                () =>
                {
                    Microsoft.VisualBasic.FileIO.FileSystem
                        .MoveDirectory(directoryNode.FullName, destDirPath, UIOption.AllDialogs);
                    _manager.RemoveNode(directoryNode);
                },
                () => Microsoft.VisualBasic.FileIO.FileSystem
                    .CopyDirectory(directoryNode.FullName, destDirPath, UIOption.AllDialogs),
                e);
        }

        private void DropProjectNode(ProjectNode projectNode, FileSystemNode target, DragEventArgs e)
        {
            string destPath = Path.Combine(GetDirectoryPath(target), projectNode.FileName);
            DropNode(
                () =>
                {
                    if (FileSystemUtility.CheckFileExists(destPath, _manager.UseRecycleBin))
                    {
                        projectNode.FileInfo.MoveTo(destPath);
                        projectNode.RemoveNode();
                    }
                },
                () =>
                {
                    using (FileStream stream = FileSystemUtility.CreateNewOrOverwrite(destPath))
                    {
                        if (stream != null)
                            projectNode.Project.Save(stream);
                    }
                },
                e);
        }

        private void DropInputNode(InputNode inputNode, ProjectNode projectNode, DragEventArgs e)
        {
            DropNode(
                () =>
                {
                    if (inputNode.MoveTo(projectNode))
                        _trv.SelectedNode = inputNode;
                },
                () =>
                {
                    InputNode node = inputNode.CopyTo(projectNode);
                    if (node != null)
                        _trv.SelectedNode = node;
                },
                e);
        }

        private void DropFileInputNode(FileInputNode fileNode, ProjectNode projectNode, DragEventArgs e)
        {
            DropNode(
                () =>
                {
                    if (fileNode.MoveTo(projectNode))
                        _trv.SelectedNode = fileNode;
                },
                () =>
                {
                    FileInputNode node = fileNode.CopyTo(projectNode);
                    if (node != null)
                        _trv.SelectedNode = node;
                },
                e);
        }

        private void DropNode(Action moveCommand, Action copyCommand, DragEventArgs e)
        {
            if (_rightMouseButton)
            {
                ShowMenu(new Point(e.X, e.Y), () => Move(), () => Copy());
            }
            else
            {
                Move();
            }

            void Move() => Executor.Execute(() => moveCommand(), Executor.BasicExceptions | Exceptions.InvalidOperation);

            void Copy() => Executor.Execute(() => copyCommand(), Executor.BasicExceptions | Exceptions.InvalidOperation);
        }

        private static void ShowMenu(Point location, Action moveCommand, Action copyCommand)
        {
            var cms = new ContextMenuStrip();
            cms.Items.Add(Resources.Move, null, (object sender, EventArgs e) => moveCommand());
            cms.Items.Add(Resources.Copy, null, (object sender, EventArgs e) => copyCommand());
            cms.Items.AddSeparator();
            cms.Items.Add(Resources.Cancel, null, (object sender, EventArgs e) => cms.Hide());
            cms.Show(location);
        }

        private static string GetDirectoryPath(FileSystemNode destNode)
        {
            switch (destNode.Kind)
            {
                case NodeKind.Root:
                    return destNode.DirectoryName;
                case NodeKind.Directory:
                    return destNode.FullName;
            }

            return null;
        }

        private static DragItem GetDragItem(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragItem)))
                return (DragItem)e.Data.GetData(typeof(DragItem));

            return null;
        }

        private FileSystemNode GetTargetNode(DragEventArgs e)
        {
            TreeNode target = _trv.GetNodeAt(_trv.PointToClient(new Point(e.X, e.Y)));
            if (target != null)
            {
                if (target is IInputContainer input)
                    return input.ProjectNode;

                return target as FileSystemNode;
            }

            return null;
        }

        private static ProjectNode GetProjectNode(TreeNode target)
        {
            return (target is FileInputNode inputNode) ? inputNode.ProjectNode : target as ProjectNode;
        }

        private static bool CheckNode(DragItem item, FileSystemNode target)
        {
            switch (item.Kind)
            {
                case NodeKind.Directory:
                    return target.Kind == NodeKind.Root
                        || (target.Kind == NodeKind.Directory && !ReferenceEquals(item.Node, target));
                case NodeKind.Project:
                    return target.Kind == NodeKind.Root || (target.Kind == NodeKind.Directory);
                case NodeKind.Input:
                    return target.Kind == NodeKind.Project && !ReferenceEquals(target, ((InputNode)item.Node).ProjectNode);
                case NodeKind.FileInput:
                    {
                        ProjectNode projectNode = GetProjectNode(target);
                        if (projectNode != null)
                            return !ReferenceEquals(projectNode, ((FileInputNode)item.Node).ProjectNode);

                        break;
                    }
            }

            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _trv.DragEnter -= (f, f2) => TreeView_DragOverDragEnter(f, f2);
                    _trv.DragOver -= (f, f2) => TreeView_DragOverDragEnter(f, f2);
                    _trv.DragDrop -= (f, f2) => TreeView_DragDrop(f, f2);
                }

                _disposed = true;
            }
        }
    }
}
