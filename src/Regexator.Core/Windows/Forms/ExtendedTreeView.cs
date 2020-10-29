// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public class ExtendedTreeView : TreeView
    {
        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Button == MouseButtons.Right)
                SelectedNode = e.Node;

            base.OnNodeMouseClick(e);
        }
    }
}