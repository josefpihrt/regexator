// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public static class DataGridViewRowCollectionExtensions
    {
        public static DataGridViewRow GetFirstVisibleNotFrozen(this DataGridViewRowCollection collection)
        {
            return GetFirstVisibleNotFrozen(collection, 0);
        }

        public static DataGridViewRow GetFirstVisibleNotFrozen(this DataGridViewRowCollection collection, int startIndex)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            return collection
                .Cast<DataGridViewRow>()
                .Skip(startIndex)
                .FirstOrDefault(f => f.Visible && !f.Frozen);
        }
    }
}
