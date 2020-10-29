// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Regexator.Text.RegularExpressions
{
    public class CaptureItemCollection : ReadOnlyCollection<CaptureItem>
    {
        internal CaptureItemCollection(IList<CaptureItem> list)
            : base(list)
        {
        }
    }
}
