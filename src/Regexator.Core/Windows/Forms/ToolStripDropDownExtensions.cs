// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public static class ToolStripDropDownExtensions
    {
        public static ToolStripDropDown GetBaseDropDown(this ToolStripDropDown dropDown)
        {
            if (dropDown == null)
                throw new ArgumentNullException(nameof(dropDown));

            ToolStripDropDown result = dropDown;
            while (true)
            {
                ToolStripItem item = result.OwnerItem;

                if (item?.Owner != null && item.Owner is ToolStripDropDown)
                {
                    result = item.Owner as ToolStripDropDown;
                }
                else
                {
                    break;
                }
            }

            return result;
        }
    }
}
