// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;
using System.Drawing;

namespace Regexator.Windows.Forms
{
    public class ExtendedTabControl : TabControl
    {
        private Rectangle _mouseDownRectangle;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            _mouseDownRectangle = Rectangle.Empty;

            if (e.Button == MouseButtons.Left
                && this.GetTabAt(e.Location) != null)
            {
                Size dragSize = SystemInformation.DragSize;
                _mouseDownRectangle = new Rectangle(
                    new Point(e.X - (int)(dragSize.Width / 2), e.Y - (int)(dragSize.Height / 2)),
                    dragSize);
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Button == MouseButtons.Left
                && _mouseDownRectangle != Rectangle.Empty
                && !_mouseDownRectangle.Contains(e.X, e.Y))
            {
                TabPage tbp = this.GetTabAt(e.Location);

                if (tbp != null)
                    DoDragDrop(tbp, DragDropEffects.Move);
            }

            base.OnMouseMove(e);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            ProcessDragEvent(drgevent);
            base.OnDragOver(drgevent);
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            ProcessDragEvent(drgevent);
            base.OnDragEnter(drgevent);
        }

        private void ProcessDragEvent(DragEventArgs drgevent)
        {
            if (drgevent == null)
                throw new ArgumentNullException(nameof(drgevent));

            TabPage source = GetSourceTab(drgevent);
            TabPage target = GetTargetTab(source, drgevent);

            drgevent.Effect = (source != null && target != null)
                ? DragDropEffects.Move
                : DragDropEffects.None;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (drgevent == null)
                throw new ArgumentNullException(nameof(drgevent));

            TabPage source = GetSourceTab(drgevent);
            TabPage target = GetTargetTab(source, drgevent);

            if (source != null && target != null)
                MoveTab(source, TabPages.IndexOf(target));

            base.OnDragDrop(drgevent);
        }

        protected static TabPage GetSourceTab(DragEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Data.GetDataPresent(typeof(TabPage)))
                return e.Data.GetData(typeof(TabPage)) as TabPage;

            return null;
        }

        private TabPage GetTargetTab(TabPage source, DragEventArgs e)
        {
            if (source != null)
            {
                TabPage target = this.GetTabAt(PointToClient(new Point(e.X, e.Y)));
                if (target != null
                    && !ReferenceEquals(source, target)
                    && (source.Parent == null || ReferenceEquals(this, source.GetTabControl())))
                {
                    return target;
                }
            }

            return null;
        }

        private void MoveTab(TabPage tabPage, int index)
        {
            MovingTab = true;
            TabPage selectedTab = SelectedTab;
            TabPages.Remove(tabPage);
            TabPages.Insert(index, tabPage);
            SelectedTab = selectedTab;
            MovingTab = false;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Button == MouseButtons.Right)
            {
                var p = new Point(e.X, e.Y);
                TabPage tbp = this.GetTabAt(p);
                if (tbp != null)
                {
                    SelectedTab = tbp;
                    ContextMenuStrip cms = tbp.ContextMenuStrip;

                    cms?.Show(this, p);
                }
            }

            base.OnMouseClick(e);
        }

        public bool MovingTab { get; private set; }
    }
}
