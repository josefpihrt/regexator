// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Windows.Forms;

namespace Regexator.UI
{
    public class ProjectInfoPropertyGrid : PropertyGrid
    {
        public ProjectInfoPropertyGrid()
        {
            Dock = DockStyle.Fill;
            HelpVisible = false;
            ToolbarVisible = false;
            PropertySort = PropertySort.NoSort;
        }

        public ProjectInfo SelectedInfo
        {
            get { return SelectedObject as ProjectInfo; }
        }
    }
}

