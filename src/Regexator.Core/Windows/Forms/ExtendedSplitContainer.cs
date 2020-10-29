// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public class ExtendedSplitContainer : SplitContainer
    {
        public ExtendedSplitContainer()
        {
            Dock = DockStyle.Fill;
            TabStop = false;
            DoubleBuffered = true;
        }
    }
}