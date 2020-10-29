// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public static class DataGridViewColumnCollectionExtensions
    {
        public static DataGridViewColumn GetFirstVisibleNotFrozen(this DataGridViewColumnCollection collection)
        {
            return GetFirstVisibleNotFrozen(collection, 0);
        }

        public static DataGridViewColumn GetFirstVisibleNotFrozen(
            this DataGridViewColumnCollection collection,
            int startIndex)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            return collection
                .Cast<DataGridViewColumn>()
                .Skip(startIndex)
                .FirstOrDefault(f => f.Visible && !f.Frozen);
        }
    }
}
