// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Regexator.Windows.Forms;

namespace Regexator.FileSystem
{
    public class FileSystemTreeView : ExtendedTreeView
    {
        [DebuggerStepThrough]
        internal FileSystemTreeView(FileSystemManager manager)
        {
            _manager = manager;
            _cmsFactory = new ContextMenuStripFactory(this, manager);
            AllowDrop = true;
            HideSelection = false;
            Dock = DockStyle.Fill;
            ShowLines = false;
            FullRowSelect = true;
            BorderStyle = BorderStyle.FixedSingle;
            ShowNodeToolTips = false;
            LabelEdit = true;
            TreeViewNodeSorter = new NodeSorter();
            ImageList = new ImageList();
            ImageList.Images.AddRange(EnumerateImages().ToArray());
        }

        private static IEnumerable<Bitmap> EnumerateImages()
        {
            yield return Resources.IcoFolder.ToBitmap();
            yield return Resources.IcoFolderOpen.ToBitmap();
            yield return Resources.IcoCode.ToBitmap();
            yield return Resources.IcoCodeSelected.ToBitmap();
            yield return Resources.IcoCodeError.ToBitmap();
            yield return Resources.IcoFile.ToBitmap();
            yield return Resources.IcoFileSelected.ToBitmap();
            yield return Resources.IcoFileError.ToBitmap();
            yield return Resources.IcoInput.ToBitmap();
            yield return Resources.IcoInputSelected.ToBitmap();
        }

        private void OnBeforeCollapseOrExpand(TreeViewCancelEventArgs e)
        {
            if ((e.Node is ProjectNode || e.Node is RootNode)
                && (MouseButtons & MouseButtons.Left) == MouseButtons.Left
                && new Rectangle(DisplayRectangle.Left, e.Node.Bounds.Top, DisplayRectangle.Width, e.Node.Bounds.Height)
                    .Contains(PointToClient(MousePosition))
                && !this.IsMouseOnPlusMinus())
            {
                e.Cancel = true;
            }
        }

        protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            OnBeforeCollapseOrExpand(e);
            base.OnBeforeCollapse(e);
        }

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            OnBeforeCollapseOrExpand(e);
            base.OnBeforeExpand(e);
        }

        protected override void OnAfterCollapse(TreeViewEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Node is RootNode node)
                FileSystemManager.SetRootNodeImage(node);

            base.OnAfterCollapse(e);
        }

        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Node is RootNode node)
                FileSystemManager.SetRootNodeImage(node);

            base.OnAfterExpand(e);
        }

        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            ContextMenuStrip = null;

            if (e.Button == MouseButtons.Right && e.Clicks == 1)
                ContextMenuStrip = _cmsFactory.GetContextMenuStrip(e.Node);

            base.OnNodeMouseClick(e);
        }

        protected override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Button == MouseButtons.Left && !this.IsMouseOnPlusMinus())
            {
                if (e.Node is InvalidNode invalidNode)
                {
                    _manager.LoadInvalid(invalidNode);
                }
                else if (e.Node is ProjectNode projectNode)
                {
                    _manager.LoadOrReloadProject(projectNode);
                }
                else if (e.Node is FileInputNode fileNode)
                {
                    _manager.LoadOrReloadFileInput(fileNode);
                }
                else if (e.Node is InputNode inputNode)
                {
                    _manager.LoadOrReloadInput(inputNode);
                }
                else if (e.Node is RootNode rootNode)
                {
                    _manager.LoadParentDirectory(rootNode);
                }
                else if (e.Node is DirectoryNode directoryNode)
                {
                    _manager.LoadDirectory(directoryNode.DirectoryInfo);
                }
            }

            base.OnNodeMouseDoubleClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Button == MouseButtons.Right && GetNodeAt(e.Location) == null)
                ContextMenuStrip = _cmsFactory.DefaultMenu;

            base.OnMouseDown(e);
        }

        protected override void CreateHandle()
        {
            base.CreateHandle();
            NativeMethods.SetWindowTheme(Handle, "explorer", null);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            _manager.KeyProcessor.ProcessKeyDown(e);
            base.OnKeyDown(e);
        }

        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            e.CancelEdit = !(e.Node is ProjectNode
                || e.Node is FileInputNode
                || e.Node is DirectoryNode
                || e.Node is InputNode);
            base.OnBeforeLabelEdit(e);
        }

        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Label != null && !e.CancelEdit)
            {
                if (e.Node is FileSystemNode node
                    && (node.Kind == NodeKind.Directory || node.Kind == NodeKind.Project || node.Kind == NodeKind.FileInput))
                {
                    e.CancelEdit = !Executor.Execute(() => node.Rename(e.Label));
                }
                else if (e.Node is InputNode inputNode)
                {
                    e.CancelEdit = !Executor.Execute(() => inputNode.Rename(e.Label));
                }
            }

            base.OnAfterLabelEdit(e);
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            using (new FileSystem.DragDrop(e, _manager))
            {
            }

            base.OnItemDrag(e);
        }

        private bool IsSortRequired(TreeNode node)
        {
            if (TreeViewNodeSorter is NodeSorter sorter)
            {
                if (node.PrevNode != null
                    && sorter.Compare(node.PrevNode, node) > 0)
                {
                    return true;
                }

                if (node.NextNode != null
                    && sorter.Compare(node, node.NextNode) > 0)
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        internal void SortIfRequired(TreeNode node)
        {
            if (IsSortRequired(node))
            {
                TreeNode selectedNode = SelectedNode;
                Sort();
                if (selectedNode != null)
                {
                    SelectedNode = selectedNode;
                    selectedNode.EnsureVisible();
                }
            }
        }

        internal void RemoveNode(TreeNode node)
        {
            _manager.RemoveNode(node);
        }

        internal bool IsCurrentInputNode(TreeNode node)
        {
            return _manager.IsCurrentInputNode(node);
        }

        private readonly ContextMenuStripFactory _cmsFactory;
        private readonly FileSystemManager _manager;
    }
}